using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootDropEntry
{
    public Item item;
    public int minAmount;
    public int maxAmount;
    public float dropChance;
}