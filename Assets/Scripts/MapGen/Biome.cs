using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "new Biome", menuName = "Biome")]
public class Biome : ScriptableObject
{
    [SerializeField] public float biomeFrequency;
    [SerializeField] public TileBase tile;
    [SerializeField] List<NPCData> spawnPool;
    [SerializeField] List<LootDropEntry> lootTable;
}
