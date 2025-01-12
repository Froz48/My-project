using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentInventory : InventoryBase
{   
    const int numberOfEquipmentSlots = 9;
    public EEquipmentSlot eEquipmentSlot { get; private set;}

    public EquipmentInventory():base(){
        Initialize(numberOfEquipmentSlots);
    }
    protected override void Initialize(int numberOfSlots = 1){
        base.Initialize(numberOfEquipmentSlots);
    }
}
