using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    #region Constants
    public const int MAX_ABILITIES = 4;
    #endregion
    
    #region Delegates
    // public static event EventHandler OnAnyPlayerSpawned;
    public event EventHandler OnHealthChanged;
    public event EventHandler OnAnyAbilityChanged;
    #endregion

    #region Public Variables
    [SerializeField] public Attribute[] attributes;
    [SerializeField] public Ability[] abilities;
    #endregion

    #region Private Variables
    [SerializeField] private float currentHealth;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerInput input;
    [SerializeField] private float reviveTime = 10;
    [SerializeField] private Vector3 spawnPosition = new Vector3(0,0,-1);
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Inventory inventory;
    [SerializeField] private EquipmentInventory equipment;
    [SerializeField] private NullAbility nullAbility;
    [SerializeField] private AttributeListSO baseAttributes;
    #endregion

    #region Unity Methods
    public override void OnNetworkSpawn() {
        if (!IsOwner) {
            enabled = false;
            return;
            
        } 
        //NetworkObject.Spawn();
        transform.position = new Vector3(0, 0, -1);
        DoStartThings();
        // OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
       if (other.TryGetComponent(out GroundItem groundItem) && inventory.CanPickupItem(groundItem.getItem()))
        {
            PickupItemServerRpc(other.GetComponent<NetworkObject>().NetworkObjectId);
        }
    }

    private void DoStartThings(){ // Fuck Start
        
        InitializeBaseValues();
        InitializeEvents();
        InitializeAbilities();
        input.onHotbarButton += () => UseHotbarSlot();
        MakeUIs();
    }

    private void FixedUpdate()
    {       
        rb.MovePosition(rb.position + input.GetMovementVectorNormalized()*Time.fixedDeltaTime*(float)GetMovementSpeed());
    }
    #endregion

    #region Initialization Methods
    private void InitializeEvents(){
        for (int i = 0; i < equipment.Slots.Length; i++){
            int whyIsItAThing = i;
            equipment.Slots[i].OnAfterUpdate += (ctx1, ctx2) => ItemEquiped(equipment.Slots[whyIsItAThing]);
            equipment.Slots[i].OnBeforeUpdate += (ctx1, ctx2) => ItemUnequiped(equipment.Slots[whyIsItAThing]);
        }
    }

    private void InitializeBaseValues(){
        //attributes = Attribute.GetPlayerBaseValues();
        baseAttributes.SetAttributes(ref attributes);
        inventory = new Inventory(40);
        equipment = new EquipmentInventory();
        currentHealth = GetMaxHealth();
    }

    private void MakeUIs(){
        GetComponentInChildren<AbilityCooldownUI>()?.MakeInterface(this);    // this exact line took me a day   
        GetComponentInChildren<HealthInterface>()?.MakeHealthUI(this);
        GetComponentInChildren<StatsInterface>()?.makeUI(attributes);
        GetComponentInChildren<InventoryUI>()?.makeUI(inventory);
        GetComponentInChildren<EquipmentUI>()?.makeUI(equipment);
    }

    private void InitializeAbilities(){
        abilities = new Ability[MAX_ABILITIES]; 
        for (int i = 0; i < abilities.Length; i++)
        {
            int bullshit = i;
            abilities[i] = nullAbility.CreateInstance();
            input.onAbilityUse[i] += () => UseAbility(bullshit);
        }
    }
    #endregion

    #region Item Management
    public void ItemEquiped(InventorySlot _slot)
    {
        if (_slot.item == null)
            return;
        if (_slot.item is EquipmentItem equipmentItem){
            foreach (var i in equipmentItem.attributeModifiers){
                attributes[(int)i.attribute].AddModifier(i);
            }
        } else Debug.Log("How the f did you equip a non-equipment item?");
    }

    private int FindFreeSlot(){
        for (int i = 0; i < abilities.Length; i++)
            if (abilities[i] == nullAbility)
                return i;
        return -1;
    }
    private int GetAbilitySlotByEquipmentType(EItemType eItemType){
        switch (eItemType){
            case EItemType.Helmet:
                return 0;
            case EItemType.Legs:
                return 1;
            case EItemType.MainHand:
                return 2;
            case EItemType.OffHand:
                return 3;
            default: 
                return -1;
        }
    }

    public void ItemUnequiped(InventorySlot _slot)
    {
        if (_slot.item == null)
            return; 
        if (_slot.item is EquipmentItem equipmentItem){
            foreach (var i in equipmentItem.attributeModifiers){
                attributes[(int)i.attribute].RemoveModifier(i);
            }
        } else Debug.Log("Unequipped non-equipment item, good boy!");
    }

    
    #endregion

    #region Character Management
    public void RecountAttrubutes(){
        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i].UpdateModifiedValue();
        }
    }

    public void GetDamage(float damage){
        currentHealth -= damage;
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            FindAnyObjectByType<ReviveManager>().Kill(gameObject, reviveTime);
        }
    }

    public void Revive(){
        currentHealth = GetMaxHealth();
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
        transform.position = spawnPosition;
    }
    #endregion

    #region Ability and Action Management
    private void UseHotbarSlot(){ // FIX THIS
        // Debug.Log("UseHotbar" + inventory.Slots[0].item.name);
        //inventory.Slots[0].item.UseItem(playerCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
        // inventory.Slots[0].RemoveAmount(1);
    }

    private void UseAbility(int index){
        if (Time.time >= abilities[index].nextUseTime){
            abilities[index].nextUseTime = Time.time + abilities[index].cooldown;
            Vector2 worldPosition = playerCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Debug.Log("UseAbility" + index + " " + abilities[index].GetType());
            UseAbility(worldPosition, index);
        }
    }

    private void UseAbility(Vector2 mousePosition, int index){
        abilities[index]?.AbilityUseServerRpc(mousePosition);
    }

    private void ChangeAbilityInstance(int index, Ability ability){
        abilities[index] = ability.CreateInstance();
        OnAnyAbilityChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Atack(object sender, EventArgs e){
    }
    
    [ServerRpc] // every solutuion i found uses ServerRpc and not [Rpc(sendto.server)] :(
    private void PickupItemServerRpc(ulong itemNetworkObjectId, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        if (NetworkManager.ConnectedClients.ContainsKey(clientId)
            && NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(itemNetworkObjectId, out NetworkObject _item))
        {
            var client = NetworkManager.ConnectedClients[clientId];
            PickupItemClientRpc(itemNetworkObjectId, RpcTarget.Single(clientId, RpcTargetUse.Temp));
            _item.Despawn(); 
            // Probably there's a bug if 2 people will try to pickup the same item at the same time then item will duplicated and game crashes? Or server works in monothreaded mode?
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void PickupItemClientRpc(ulong itemNetworkObjectId, RpcParams rpcParams = default)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(itemNetworkObjectId, out NetworkObject _item);
        inventory.AddItem(_item.GetComponent<GroundItem>().getItem(), 1);
    }
    #endregion

    #region Getters
    public float GetAttributeByIndex(int index) => attributes[index].GetValue();
    public float GetMovementSpeed() => attributes[(int)EAttributes.MovementSpeed].GetValue();
    public float GetPower() => attributes[(int)EAttributes.Power].GetValue();
    public float GetMaxHealth() => attributes[(int)EAttributes.MaxHealth].GetValue();
    public float getCurrentHealth() => currentHealth;
    public Inventory GetInventory() => inventory;
    public EquipmentInventory GetEquipment() => equipment;
    #endregion
}
