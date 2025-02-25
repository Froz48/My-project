using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BossData : ScriptableObject
{
    [field:SerializeField] public BossTimer[] timer {get; private set;}
    [field:SerializeField] public LootDropTable loot {get; private set;}
    [field:SerializeField] public float maxHealth {get; private set;}

}
