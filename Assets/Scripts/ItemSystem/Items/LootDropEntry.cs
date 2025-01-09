using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class LootTable : ScriptableObject, IEnumerable<LootDropEntry>
// {
//     public List<LootDropEntry> lootTable;

//     public static implicit operator List<LootDropEntry>(LootTable v){
//         return v.lootTable;
//     }

//     public IEnumerator<LootDropEntry> GetEnumerator(){
//         return lootTable.GetEnumerator();
//     }

//     IEnumerator IEnumerable.GetEnumerator(){
//         return GetEnumerator();
//     }
// }

[System.Serializable]
public class LootDropEntry
{
    public ItemObject item;
    public float dropChance;
}