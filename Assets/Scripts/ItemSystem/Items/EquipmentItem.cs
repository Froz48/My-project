using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEquipmentSlot{
    Helmet, Chest, Neck, Gloves, Shoulders, Belt, Legs, MainHand, OffHand
}

public class EquipmentItem : ItemBase
{
    public EEquipmentSlot eEquipmentSlot { get; private set;}
    public AttributeModifier[] attributeModifiers { get; private set;}
    public Ability ability { get; private set;}

}
