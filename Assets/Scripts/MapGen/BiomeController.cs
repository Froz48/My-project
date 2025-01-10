using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "new Biome", menuName = "Biome")]
public class Biome : ScriptableObject
{
    [SerializeField] public float biomeFrequency;
    [SerializeField] public string biomeName ;
    [SerializeField] public float temperatureLeftBorder;
    [SerializeField] public float temperatureRightBorder;
    [SerializeField] public float rainfallLeftBorder;
    [SerializeField] public float rainfallRightBorder;
    [SerializeField] public TileBase tile;
    [SerializeField] List<MonsterData> spawnPool;
    [SerializeField] List<LootDropEntry> lootTable;
}
