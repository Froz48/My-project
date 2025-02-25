using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EquipmentInventory : InventoryBase
{   
    const int numberOfEquipmentSlots = 9;
    public EquipmentInventory():base(){
        Initialize(numberOfEquipmentSlots);
    }
    protected override void Initialize(int numberOfSlots = 1){
        base.Initialize(numberOfEquipmentSlots);
    }

    public void EquipItem(InventorySlot slot){
        EquipmentItem item = slot.item as EquipmentItem;
        SwapItems(slot, Slots[(int)item.eEquipmentSlot]);
    }



}
