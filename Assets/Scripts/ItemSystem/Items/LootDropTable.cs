using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LootDropTable
{
    public List<LootDropEntry> lootDrops;

    public List<Item> generateLoot(){
        List<Item> loot = new List<Item>();
        foreach (LootDropEntry entry in lootDrops){
            float roll = UnityEngine.Random.value;
            if (roll < entry.dropChance){
                loot.Add(entry.item);
            }
        }
        return loot;
    }
}
