using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LootDropTable
{
    public List<LootDropEntry> lootDrops;

    public List<ItemBase> generateLoot(){
        List<ItemBase> loot = new List<ItemBase>();
        foreach (LootDropEntry entry in lootDrops){
            float roll = UnityEngine.Random.value;
            if (roll < entry.dropChance){
                loot.Add(entry.item);
            }
        }
        return loot;
    }
}
