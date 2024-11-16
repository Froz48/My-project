using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public enum EItemType 
{
        Helmet,
    Chest, Neck, Gloves,
    Shoulders, Belt, Legs,
    MainHand,       OffHand,


    Food,
    Default
}


[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/item")]
public class ItemObject : ScriptableObject
{
    public string Name = "";
    public int Id = -1;
    [SerializeField] bool isInitialized = false;

    public Ability ability;
    public ItemBuff[] buffs = new ItemBuff[0];
    public Sprite uiDisplay;
    public bool stackable;
    public EItemType type;
    [TextArea(15, 20)]
    public string description;
    public GameObject placeableObjectPrefab;

//---------------------------------- Public Methods ----------------------------------

    public bool IsEquipable(){
        return (
               type == EItemType.Helmet 
            || type == EItemType.Chest
            || type == EItemType.Neck
            || type == EItemType.Gloves
            || type == EItemType.Shoulders
            || type == EItemType.Belt
            || type == EItemType.Legs
            || type == EItemType.MainHand
            || type == EItemType.OffHand
            ); // can make list 
    } 
    public virtual void UseItem(Vector2 position){
        if (placeableObjectPrefab != null){
            Debug.Log("Using item " + Name);
            var a = Instantiate(placeableObjectPrefab, position, Quaternion.identity);
            a.GetComponent<NetworkObject>().Spawn();
        } else {
            Debug.Log("Tried to use item, but there's no placeableObjectPrefab");
        }
    }


    public void SpawnWorldItemCopy(Vector3 position){
        var _gameObject = Instantiate(Resources.Load("GroundItemPrefab"), position, quaternion.identity);
        _gameObject.GetComponent<GroundItem>().setItem(ItemObject.CreateInstanceInitialized(this));
        _gameObject.GetComponent<SpriteRenderer>().sprite = _gameObject.GetComponent<GroundItem>().getItem().uiDisplay;
        _gameObject.GetComponent<NetworkObject>().Spawn();
    }

    public static ItemObject CreateInstance()
    {
        var newItem = ScriptableObject.CreateInstance<ItemObject>();
        newItem.SetNullItem();
        return newItem;
    }
    public static ItemObject CreateInstanceInitialized(int itemId)
    {
        DatabaseItems database = Resources.Load<DatabaseItems>("ItemDatabase");
        return ItemObject.CreateInstanceInitialized(database.ItemObjects[itemId]);
    }
    public static ItemObject CreateInstanceInitialized(ItemObject itemObject)
    {
        Debug.Log(itemObject.GetInstanceID());
        var newItem = ScriptableObject.CreateInstance<ItemObject>();
        

        // this is disaster but i dont know how to fix it
        newItem.Name = itemObject.Name;
        newItem.Id = itemObject.Id;
        newItem.type = itemObject.type;
        newItem.description = itemObject.description;
        newItem.uiDisplay = itemObject.uiDisplay;
        newItem.stackable = itemObject.stackable;
        newItem.buffs = new ItemBuff[itemObject.buffs.Length];
        newItem.ability = itemObject.ability;
        newItem.placeableObjectPrefab = itemObject.placeableObjectPrefab;

        for (int i = 0; i < itemObject.buffs.Length; i++){
            newItem.buffs[i] = new ItemBuff(itemObject.buffs[i].attribute ,itemObject.buffs[i].addMin, itemObject.buffs[i].addMax);
        }
        newItem.Initialize();
        Debug.Log(newItem.GetInstanceID());
        return newItem;
    }
    public void Initialize(){
        if (isInitialized){
            Debug.Log("Trying to initialize an already initialized item");
            return;
        }
        for (int i = 0; i < buffs.Length; i++){
            buffs[i].GenerateValue();
        }
        isInitialized = true;
    }
    //----------------------------- Private Methods -------------------------

    private void SetNullItem(){
        Name = "";
        Id = -1;
        buffs = new ItemBuff[0];
    }

}


