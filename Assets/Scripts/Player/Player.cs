using System;
using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : NetworkBehaviour, IDamageable
{
    #region Constants
    public const int MAX_ABILITIES = 4;
    #endregion
    
    #region Delegates
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
    [SerializeField] private PlayerInputController input;
    [SerializeField] private float reviveTime = 10;
    [SerializeField] private Vector3 spawnPosition = new Vector3(0,0,-1);
    [SerializeField] private Inventory inventory;
    [SerializeField] private EquipmentInventory equipment;
    [SerializeField] private NullAbility nullAbility;
    [SerializeField] private Ability meleeAbility;
    [SerializeField] private AttributeListSO baseAttributes;
    [SerializeField] private NetworkObject playerNetworkObject;
    #endregion

    #region Unity Methods
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            if (TryGetComponent(out PlayerInputController input)) input.enabled = false;
            if (TryGetComponent(out Camera cam)) cam.enabled = false;
            if (TryGetComponent(out MapGen mapgen)) mapgen.enabled = false;
            return;

        }
        transform.position = new Vector3(0, 0, -1);
        InitializePlayer();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
       if (other.TryGetComponent(out GroundItem groundItem) && inventory.CanPickupItem(groundItem.GetItem()))
        {
            PickupItemServerRpc(other.GetComponent<NetworkObject>().NetworkObjectId);
        }
    }

    private void InitializePlayer(){ // Fuck Start
        
        InitializeBaseValues();
        InitializeEvents();
        InitializeAbilities();
        input.onHotbarButton += () => UseHotbarSlot();
        MakeUIs();
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            Vector2 movement = input.GetMovementVectorNormalized().normalized;
            if (movement != Vector2.zero)
            {
                MoveInDirectionServerRpc(movement*(float)GetMovementSpeed()*Time.fixedDeltaTime);
            }

        }
    }

    [ServerRpc]
    private void MoveInDirectionServerRpc(Vector2 direction)
    {
        rb.MovePosition(rb.position + direction);
        MoveInDirectionClientRpc(direction);
    }
    [ClientRpc]
    private void MoveInDirectionClientRpc(Vector2 direction)
    {
        rb.position += direction;
    }
    [ServerRpc]
    private void UpdatePositionServerRpc(Vector2 position)
    {
        transform.position = new Vector3(position.x, position.y, -1);
        UpdatePositionClientRpc(position);
    }
    [ClientRpc]
    private void UpdatePositionClientRpc(Vector2 position)
    {
        transform.position = new Vector3(position.x, position.y, -1f);     
    }

    #endregion

    #region Initialization Methods
    private void InitializeEvents()
    {
        for (int i = 0; i < equipment.Slots.Length; i++)
        {
            int whyIsItAThing = i;
            equipment.Slots[i].OnAfterUpdate += (ctx1, ctx2) => ItemEquiped(equipment.Slots[whyIsItAThing]);
            equipment.Slots[i].OnBeforeUpdate += (ctx1, ctx2) => ItemUnequiped(equipment.Slots[whyIsItAThing]);
        }
    }

    private void InitializeBaseValues(){
        baseAttributes.SetAttributes(ref attributes);
        inventory = new Inventory(40);
        equipment = new EquipmentInventory();
        currentHealth = GetMaxHealth();
    }

    private void MakeUIs(){
        FindObjectOfType<AbilityCooldownUI>()?.MakeInterface(this);
        FindObjectOfType<HealthInterface>()?.MakeHealthUI(this);
        FindObjectOfType<StatsInterface>()?.makeUI(attributes);
        FindObjectOfType<InventoryUI>()?.makeUI(inventory);
        FindObjectOfType<EquipmentUI>()?.makeUI(equipment);
        OnAnyAbilityChanged?.Invoke(this, EventArgs.Empty);
    }

    private void InitializeAbilities(){
        abilities = new Ability[MAX_ABILITIES]; 
        for (int i = 0; i < abilities.Length; i++)
        {
            int bullshit = i;
            abilities[i] = nullAbility.CreateInstance();
            input.onAbilityUse[i] += () => UseAbilityOnPosition(bullshit);
        }
        ChangeAbilityInstance(0, meleeAbility);
    }
    #endregion

    #region Item Management
    public void ItemEquiped(InventorySlot _slot)
    {
        if (_slot.item == null)
            return;
        foreach (var i in _slot.item.attributeModifiers){
            attributes[(int)i.attribute].AddModifier(i);
        }
        if (_slot.item.ability != null){
            Debug.Log("Equiped an equipment item with an ability");
            ChangeAbilityInstance(_slot.item.GetAbilityPosition(), _slot.item.ability);
        }
        
    }
    public void ItemUnequiped(InventorySlot _slot)
    {
        if (_slot.item == null)
            return; 
        foreach (var i in _slot.item.attributeModifiers){
            attributes[(int)i.attribute].RemoveModifier(i);
        }
        if (_slot.item.ability != null){
            Debug.Log("Unequiped an equipment item with an ability");
            if (_slot.item.GetAbilityPosition() == 0){
                ChangeAbilityInstance(_slot.item.GetAbilityPosition(), meleeAbility);
            }
            else {
                ChangeAbilityInstance(_slot.item.GetAbilityPosition(), nullAbility);
            }
        }
    }

    
    #endregion

    #region Character Management
    public void RecountAttrubutes(){
        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i].UpdateModifiedValue();
        }
    }
    
    public void TakeDamage(float damage)
    {
        if (IsOwner)
        {
            currentHealth = Mathf.Max(0, currentHealth - damage);
            OnHealthChanged?.Invoke(this, EventArgs.Empty);
            if (currentHealth <= 0)
            {
                FindAnyObjectByType<ReviveManager>()?.KillPlayerServerRpc(GetComponent<NetworkObject>().NetworkObjectId, reviveTime);
            }
        }
    }

    public void Revive()
    {
        if (IsOwner)
        {
            currentHealth = GetMaxHealth();
            OnHealthChanged?.Invoke(this, EventArgs.Empty);
            UpdatePositionServerRpc(spawnPosition);
        }
        else Debug.LogError("Tried to revive other players");
    }
    #endregion

    #region Ability and Action Management
    private void UseHotbarSlot(){ // FIX THIS
        // Debug.Log("UseHotbar" + inventory.Slots[0].item.name);
        //inventory.Slots[0].item.UseItem(playerCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
        // inventory.Slots[0].RemoveAmount(1);
    }
    private void UseAbilityOnPosition(int index){
        if (abilities[index].IsReady())
        {
            Vector2 worldPosition = GetComponent<Camera>().ScreenToWorldPoint(Mouse.current.position.ReadValue());
            UseAbilityServerRpc(worldPosition, abilities[index].id);
            abilities[index].StartCooldown();
        }
    }

    [ServerRpc]
    private void UseAbilityServerRpc(Vector2 mousePosition, int abilityId){
        UseAbilityClientRpc(mousePosition, abilityId);
    }
    [ClientRpc]
    private void UseAbilityClientRpc(Vector2 mousePosition, int abilityId)
    {
        ((Resources.Load("AbilityDatabase") as Database).GetObjectById(abilityId) as Ability).AbilityUse(transform.position, mousePosition);
    }

    private void ChangeAbilityInstance(int index, Ability ability)
    {
        if (index == -1) return;
        abilities[index] = ability.CreateInstance();
        OnAnyAbilityChanged?.Invoke(this, EventArgs.Empty);
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
        inventory.AddItem(_item.GetComponent<GroundItem>().GetItem(), 1);
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

    internal void SetCurrentHealth(float health)
    {
        currentHealth = health;
    }
    #endregion
}
