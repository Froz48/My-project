using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newSpawnPool", menuName = "SpawnPool")]
public class SpawnPool : ScriptableObject
{
    [SerializeField] public List<MonsterData> Pool = new List<MonsterData>();

    public void Add(MonsterData monsterData){
        Pool.Add(monsterData);
    }

    public void Add(SpawnPool spawnPool){
        Pool.AddRange(spawnPool.Pool);
    }

    public void Remove(MonsterData monsterData){
        Pool.Remove(monsterData);
    }

    public void Remove(SpawnPool spawnPool){
        Pool.RemoveAll(spawnPool.Pool.Contains);
    }

}
