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
    [SerializeField] private EAttributes name;
    [SerializeField] private int baseValue;
    [SerializeField] private int value;
    private List<IModifier> modifiers = new List<IModifier>();
    
    #endregion
    public event EventHandler onValueModified;
    public Attribute(EAttributes _type, int _baseValue = 0){
        name = _type;
        baseValue = _baseValue;
        value = _baseValue;
    }
    public Attribute(int _baseValue = 0)
    {   baseValue = _baseValue;
        value = _baseValue;
    }
    public static string GetStatNameById(int _value){
        return Enum.GetName(typeof(EAttributes), _value);
    }
    public void SetBaseValue(int _baseValue){
        baseValue = _baseValue;
        UpdateModifiedValue();
    }
    public int GetBaseValue(){
        return baseValue;
    }
    public int GetValue(){
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
            modifiers[i].AddValue(ref value);
        }
        onValueModified?.Invoke(this, EventArgs.Empty);
    }
    public void AddModifier(IModifier _modifier)
    {
        modifiers.Add(_modifier);
        UpdateModifiedValue();
    }
    public void RemoveModifier(IModifier _modifier)
    {
        modifiers.Remove(_modifier);
        UpdateModifiedValue();
    }
    public static Attribute[] GetPlayerBaseValues(){
        return CreateAttributes(new int[]{10, 10, 5, 0, 100});
    }

    public static Attribute[] GetMonsterBaseValues()
    {
        return CreateAttributes(new int[]{1, 1, 1, 0, 1});
    }

    public static Attribute[] CreateAttributes(int[] vals)
    {//Agility, Intellect, Strength, MovementSpeed, MaxHealth
        int attributeCount = Enum.GetValues(typeof(EAttributes)).Length;
        
        if (vals.Length != attributeCount){
            Debug.Log("Wrong attribute count! expected: " + attributeCount + " got: " + vals.Length);
            return null;
        }

        Attribute[] stats = new Attribute[attributeCount];
        for (int i = 0; i < attributeCount; i++)
        {
            stats[i] = new Attribute();
            stats[i].SetBaseValue(vals[i]);
        }
        return stats;
    }

}