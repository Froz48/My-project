using System;
using System.Collections.Generic;
using UnityEngine;
public enum EAttributes{
    Power, AttackSpeed, MovementSpeed, Armor, MaxHealth,
    Population, Housing, Gold, Defenses, StorageSpace, Fame, Infrastructure, Complexity
}//(int)EAttributes.MaxHealth].GetValue()

[Serializable]
public class Attribute
{
    #region main variables
    [SerializeField] public EAttributes name;
    [SerializeField] private float baseValue;
    [SerializeField] private float value;
    private List<AttributeModifier> modifiers = new List<AttributeModifier>();

    #endregion
    public event EventHandler onValueModified;
    public Attribute(EAttributes _type, float _baseValue = 0){
        name = _type;
        baseValue = _baseValue;
        value = _baseValue;
    }
    public Attribute(int _baseValue = 0)
    {   baseValue = _baseValue;
        value = _baseValue;
    }
    public float GetBaseValue(){
        return baseValue;
    }
    public float GetValue(){
        return value;
    }
    public EAttributes GetName(){
        return name;
    }

    public void UpdateModifiedValue()
    {
        value = baseValue;
        for (int i = 0; i < modifiers.Count; i++)
        {
            value += modifiers[i].addValue;
            // modifiers[i].AddValue(ref value);
        }
        onValueModified?.Invoke(this, EventArgs.Empty);
    }
    public void AddModifier(AttributeModifier _modifier)
    {
        modifiers.Add(_modifier);
        UpdateModifiedValue();
    }
    public void RemoveModifier(AttributeModifier _modifier)
    {
        modifiers.Remove(_modifier);
        UpdateModifiedValue();
    }
}