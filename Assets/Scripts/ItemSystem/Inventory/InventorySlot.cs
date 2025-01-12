using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    // public ItemType[] AllowedItems = new ItemType[0];
    public event EventHandler OnAfterUpdate;
    public event EventHandler OnBeforeUpdate;
    public ItemBase item;
    public int amount;
    public InventorySlot(){}
    public InventorySlot(ItemBase _item, int _amount){
        UpdateSlot(_item, _amount);
    }

    public void UpdateSlot(ItemBase _item, int _amount)
    {
        OnBeforeUpdate?.Invoke(this, EventArgs.Empty);
        item = _item;
        amount = _amount;
        OnAfterUpdate?.Invoke(this, EventArgs.Empty);
    }
    public void RemoveItem()
    {
        UpdateSlot(null, 0);
    }
    public bool IsEmpty(){
        return item == null;
    }
    public void AddAmount(int value = 1){
        UpdateSlot(item, amount + value);
        if (amount <= 0){
            RemoveItem();
        }
    }

}