using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public abstract class InventoryBase
{
    public string savePath;
    public int slotCount { get; protected set; }
    public InventorySlot[] Slots {  get; protected set; }
    public event EventHandler onItemUpdate;
    //protected ItemDatabase database;
    protected virtual void Initialize(int numberOfSlots = 1)
    {
        Slots = new InventorySlot[numberOfSlots];
        for (int i = 0; i < numberOfSlots; i++)
        {
            Slots[i] = new InventorySlot();
            Slots[i].OnAfterUpdate += (sender, e) => onItemUpdate?.Invoke(this, EventArgs.Empty);
        }
        //database = Resources.Load<ItemDatabase>("ItemDatabase");
    }

    public InventorySlot FindItemOnInventory(ItemBase _item)
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            if(Slots[i].item?.id == _item.id)
            {
                return Slots[i];
            }
        }
        return null;
    }

    public bool IsItemInInventory(ItemBase _item){
        for (int i = 0; i < Slots.Length; i++)
        {
            if(Slots[i].item?.id == _item.id)
            {
                return true;
            }
        }
        return false;
    }
    internal void SwapItems(InventorySlot slot1, InventorySlot slot2)
    {
        InventorySlot temp = new InventorySlot(slot2.item, slot2.amount);
        slot2.UpdateSlot(slot1.item, slot1.amount);
        slot1.UpdateSlot(temp.item, temp.amount);
    }
#region Serialization
    [ContextMenu("Save")]
    public void Save()
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, Slots);
        stream.Close();
    }
    [ContextMenu("Load")]
    public void Load()
    {
        if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            InventorySlot[] newContainer = (InventorySlot[])formatter.Deserialize(stream);
            for (int i = 0; i < Slots.Length; i++)
            {
                Slots[i].UpdateSlot(newContainer[i].item, newContainer[i].amount);
            }
            stream.Close();
        }
    }
    [ContextMenu("Clear")]
    public void Clear()
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            Slots[i].RemoveItem();
        }
    }


    #endregion
}
