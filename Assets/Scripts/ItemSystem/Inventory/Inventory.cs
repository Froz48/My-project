using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EItemType 
{
    Helmet, Chest, Neck, Gloves, Shoulders, Belt, Legs, MainHand, OffHand,
    Food, Default
}
public class Inventory : InventoryBase
{
    public int GetEmptySlotCount(){
        int counter = 0;
        for (int i = 0; i < base.Slots.Length; i++){
            if (base.Slots[i].amount == 0){
                counter++;}
            }
        return counter; 
    }

    public Inventory(int slotCount) : base(){
        Initialize(slotCount);
    }

    public InventorySlot GetEmptySlot(){
        for (int i = 0; i < base.Slots.Length; i++){
            if (base.Slots[i].item == null){
                return base.Slots[i];
            }
        }
        return null;
    }

    internal bool CanPickupItem(ItemBase itemInstance)
    {
        if (GetEmptySlotCount() > 0 || IsItemInInventory(itemInstance)){
            return true;
        }
        else return false;
    }

    internal void AddItem(ItemBase itemInstance, int v)
    {
        GetEmptySlot().UpdateSlot(itemInstance, v);
    }



    /*
public bool AddItem(ItemInstance _item, int _amount)
{
   if (!CanPickupItem(_item)){
       return false;
   }

   InventorySlot slot = FindItemOnInventory(_item);
   if(slot == null)
   {// не стакается или предмет новый
       AddToEmptySlot(_item, _amount);
       //onItemUpdate?.Invoke(this, EventArgs.Empty);
       return true;
   }
   slot.AddAmount(_amount);

  // onItemUpdate?.Invoke(this, EventArgs.Empty);
   return true;
}
private InventorySlot AddToEmptySlot(ItemInstance _item, int _amount)
{
   for (int i = 0; i < Slots.Length; i++)
   {
       if (Slots[i].item.itemBase.Id == -1)
       {   
           Slots[i].UpdateSlot(_item, _amount);
           return Slots[i];
       }
   }
   //set up functionality for full inventory
   return null;
}
public InventorySlot GetEmptySlot(){
   for (int i = 0; i < Slots.Length; i++)
   {
       if (Slots[i].item.itemBase.Id == -1)
       {  
           return Slots[i];
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
public bool CanPickupItem(ItemInstance item)
{
   if (GetEmptySlotCount() > 0){
       return true;
   }
   if (FindItemOnInventory(item)){
       return true;
   }
   return false;
}
*/
}
