using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EEquipmentSlot{
    Helmet, Chest, Neck, Gloves, Shoulders, Belt, Legs, MainHand, OffHand
}
[Serializable]
[CreateAssetMenu(fileName = "Item", menuName = "ItemSystem/Item")]
public class Item : ScriptableObject, IDatabaseObject
{
    public int id;
    [SerializeField] public Sprite uiDisplay;
    [SerializeField] public bool isStackable;
    [TextArea(15, 20)][SerializeField] public string description;
    public EEquipmentSlot eEquipmentSlot;
    public int price;
    public AttributeModifier[] attributeModifiers;
    [SerializeReference] public Ability ability;

    public int GetId()
    {
        return id;
    }

    public void SetId(int id)
    {
        this.id = id;
    }

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
    
    GameObject CreateItem(){
        return null;
    }
}
