using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "new Biome", menuName = "Biome")]
public class Biome : ScriptableObject
{ //[Range(0f, 1f)]
    [SerializeField] public float biomeFrequency;
    [SerializeField] public TileBase tile;
    [field:SerializeField] public List<NPCData> SpawnPool { get; private set; }
    [SerializeField] List<LootDropEntry> lootPool;
}
