using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BossData : ScriptableObject, IDatabaseObject
{
    int id;
    [SerializeField] GameObject prefab;
    [field: SerializeField] public BossTimer[] timer {get; private set;}
    [field:SerializeField] public LootDropTable loot {get; private set;}
    [field:SerializeField] public float maxHealth {get; private set;}

    public int GetId()
    {
        return id;
    }

    public void SetId(int id)
    {
        this.id = id;
    }
}
