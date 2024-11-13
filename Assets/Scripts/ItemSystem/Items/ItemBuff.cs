using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ItemBuff : IModifier
{ // add itemlvl buffs
    public EAttributes attribute;
    [SerializeField]
    private int valueToAdd;
    public int addMin;
    public int addMax;
    //private int valueToMultiply;
    //public int multMin;
    //public int multMax;

    public int getValue() => valueToAdd;
    public ItemBuff(EAttributes _attribute, int addMin = 0, int addMax = 0 /*, int multMin = 0, int multMax = 0 */)
    {
        attribute = _attribute;
        this.addMin = addMin;
        this.addMax = addMax;
        //this.multMin = multMin;
        //this.multMax = multMax;
    }

    public void AddValue(ref int value)
    {
        value += valueToAdd;
    }

    // public void MultiplyValue(ref int value){
    //     value *= valueToMultiply;
    // }

    public void GenerateValue()
    {
        int d = UnityEngine.Random.Range(addMin, addMax);
        valueToAdd = d;
        //valueToMultiply = UnityEngine.Random.Range(multMin, multMax);
    }
}
