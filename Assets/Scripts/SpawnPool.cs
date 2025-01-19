using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newSpawnPool", menuName = "SpawnPool")]
public class SpawnPool : ScriptableObject
{
    [SerializeField] public List<NPCData> Pool = new List<NPCData>();

    public void Add(NPCData monsterData){
        Pool.Add(monsterData);
    }

    public void Add(SpawnPool spawnPool){
        Pool.AddRange(spawnPool.Pool);
    }

    public void Remove(NPCData monsterData){
        Pool.Remove(monsterData);
    }

    public void Remove(SpawnPool spawnPool){
        Pool.RemoveAll(spawnPool.Pool.Contains);
    }

}
