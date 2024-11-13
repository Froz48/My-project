using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventorySlot : ScriptableObject
{
    public EItemType[] AllowedItems = new EItemType[0];
    public event EventHandler OnAfterUpdate;
    public event EventHandler OnBeforeUpdate;
    public ItemObject item;
    public int amount;

    // public InventorySlot()
    // {
    //     UpdateSlot(ItemObject.CreateInstance<ItemObject>(), 0);
    // }

    public static InventorySlot CreateInstance(){
        InventorySlot inventorySlot = ScriptableObject.CreateInstance<InventorySlot>();
        inventorySlot.Initialize();
        return inventorySlot;
    }
    private void Initialize(){
        item = ItemObject.CreateInstance<ItemObject>();
        AllowedItems = new EItemType[0];
        amount = 0;
    }

    // public void InventoryObject(){
    //     UpdateSlot(ItemObject.CreateInstance<ItemObject>(), 0);
    // }
    public void UpdateSlot(ItemObject _item, int _amount)
    {
        OnBeforeUpdate?.Invoke(this, EventArgs.Empty);
        item = _item;
        amount = _amount;
        OnAfterUpdate?.Invoke(this, EventArgs.Empty);
    }
    public void RemoveItem()
    {
        UpdateSlot(ItemObject.CreateInstance<ItemObject>(), 0);
    }
    public void RemoveAmount(int value = 1){
        UpdateSlot(item, amount -= value);
        if (amount <= 0){
            RemoveItem();
        }
    }
    public void AddAmount(int value)
    {
        UpdateSlot(item, amount += value);
    }
    public bool CanPlaceInSlot(ItemObject _itemObject)
    {
        if (AllowedItems.Length <= 0 || _itemObject == null || _itemObject.Id == -1)
            return true;
        for (int i = 0; i < AllowedItems.Length; i++)
        {
            if (_itemObject.type == AllowedItems[i])
                return true;
        }
        return false;
    }
}