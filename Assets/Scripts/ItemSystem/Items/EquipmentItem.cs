using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEquipmentSlot{
    Helmet, Chest, Neck, Gloves, Shoulders, Belt, Legs, MainHand, OffHand
}


[CreateAssetMenu]
public class EquipmentItem : ItemBase
{
    public EEquipmentSlot eEquipmentSlot;
    public AttributeModifier[] attributeModifiers;
    public Ability ability;

    internal int GetAbilityPosition()
    {
        switch (eEquipmentSlot)
        {
            case EEquipmentSlot.MainHand:
                return 0;
            case EEquipmentSlot.OffHand:
                return 1;
            case EEquipmentSlot.Helmet:
                return 2;
            case EEquipmentSlot.Legs:
                return 3;
        }
        return -1;
    }
}
