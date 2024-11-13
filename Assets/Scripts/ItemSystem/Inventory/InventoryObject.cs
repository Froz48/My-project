using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System;
using Unity.Netcode;
using Unity.VisualScripting;
public enum EInventoryType
{
    Inventory,
    Equipment,
    Chest
}


[Serializable]
public class InventoryObject
{
    public string savePath;
    public DatabaseItems database;
    public EInventoryType type;
    public InventorySlot[] Container;

    public event EventHandler onItemUpdate;
    const int numberOfEquipmentSlots = 9;
    public int EmptySlotCount{ 
        get { 
            int counter = 0;
            for (int i = 0; i < Container.Length; i++){
                if (Container[i].item.Id == -1){
                    counter++;}
                }
            return counter; 
        } 
    }
    

    public static InventoryObject CreateInstance(EInventoryType interfaceType = EInventoryType.Inventory, int numberOfSlots = 0)
    {
        var instance = new InventoryObject();
        instance.Initialize(interfaceType, numberOfSlots);
        return instance;
    }

    public void Start(){
        database = Resources.Load<DatabaseItems>("ItemDatabase");
    }
    private void Initialize(EInventoryType interfaceType, int numberOfSlots = 0)
    {
        if (interfaceType == EInventoryType.Equipment){
            numberOfSlots = numberOfEquipmentSlots;
        }
        Container = new InventorySlot[numberOfSlots];
        for (int i = 0; i < numberOfSlots; i++)
        {
            Container[i] = InventorySlot.CreateInstance();//new InventorySlot();
            if (interfaceType == EInventoryType.Equipment){
                Container[i].AllowedItems = new EItemType[1];
                Container[i].AllowedItems[0] = (EItemType)Enum.GetValues(typeof(EItemType)).GetValue(i);
            }
            Container[i].OnAfterUpdate += ItemUpdate;
        }
        type = interfaceType;
        database = Resources.Load<DatabaseItems>("ItemDatabase");
    }
  
    public void ItemUpdate(object sender, EventArgs e){
        onItemUpdate?.Invoke(this, EventArgs.Empty);
    }
    
    // public InventoryObject(int numberOfSlots, InterfaceType interfaceType = InterfaceType.Inventory){
    //     Container = new InventorySlot[numberOfSlots];
    //     for (int i = 0; i < numberOfSlots; i++)
    //     {
    //         Container[i] = new InventorySlot();
    //     }
    //     type = interfaceType;
    //     database = Resources.Load<ItemDatabaseObject>("ItemDatabase");
    // }
    public bool AddItem(ItemObject _item, int _amount)
    {
        if (!CanPickupItem(_item)){
            
            return false;
        }

        InventorySlot slot = FindItemOnInventory(_item);
        if(!_item.stackable || slot == null)
        {// не стакается или предмет новый
            
            AddToEmptySlot(_item, _amount);
            //onItemUpdate?.Invoke(this, EventArgs.Empty);
            return true;
        }
        slot.AddAmount(_amount);
        
       // onItemUpdate?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public InventorySlot FindItemOnInventory(ItemObject _item)
    {
        for (int i = 0; i < Container.Length; i++)
        {
            if(Container[i].item.Id == _item.Id)
            {
                return Container[i];
            }
        }
        return null;
    }
    private InventorySlot AddToEmptySlot(ItemObject _item, int _amount)
    {
        for (int i = 0; i < Container.Length; i++)
        {
            if (Container[i].item.Id == -1)
            {   
                Container[i].UpdateSlot(_item, _amount);
                return Container[i];
            }
        }
        //set up functionality for full inventory
        return null;
    }
    public InventorySlot GetEmptySlot(){
        for (int i = 0; i < Container.Length; i++)
        {
            if (Container[i].item.Id == -1)
            {  
                return Container[i];
            }
        }
        return null;
    }
    public void SwapItems(InventorySlot slot1, InventorySlot slot2)
    {
        if(slot2.CanPlaceInSlot(slot1.item) && slot1.CanPlaceInSlot(slot2.item))
        {
            //InventorySlot temp = new InventorySlot( item2.item, item2.amount);
            //  why did i do this?..
            var tmpItem = slot2.item;
            var tmpAmount = slot2.amount;
            slot2.UpdateSlot(slot1.item, slot1.amount);
            slot1.UpdateSlot(tmpItem, tmpAmount);
        }
        //onItemUpdate?.Invoke(this, EventArgs.Empty);
    }
    

    [ContextMenu("Save")]
    public void Save()
    {
        //string saveData = JsonUtility.ToJson(this, true);
        //BinaryFormatter bf = new BinaryFormatter();
        //FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
        //bf.Serialize(file, saveData);
        //file.Close();

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, Container);
        stream.Close();
    }
    [ContextMenu("Load")]
    public void Load()
    {
        if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        {
            //BinaryFormatter bf = new BinaryFormatter();
            //FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
            //JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            //file.Close();

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            InventorySlot[] newContainer = (InventorySlot[])formatter.Deserialize(stream);
            for (int i = 0; i < Container.Length; i++)
            {
                Container[i].UpdateSlot(newContainer[i].item, newContainer[i].amount);
            }
            stream.Close();
        }
    }
    [ContextMenu("Clear")]
    public void Clear()
    {
        for (int i = 0; i < Container.Length; i++)
        {
            Container[i].RemoveItem();
        }
    }

    public bool CanPickupItem(ItemObject item)
    {
        
        if (EmptySlotCount > 0){
            return true;
        }
        if (item.stackable && FindItemOnInventory(item)){
            return true;
        }
        return false;
    }
}


