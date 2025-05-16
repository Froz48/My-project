

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[Serializable]
[CreateAssetMenu(fileName = "MonsterData", menuName = "NPC/MonsterData")]
public class NPCData : ScriptableObject{
    [SerializeField] public NPCBehaviour[] nPCBehaviour;
    [SerializeField] public Ability[] abilities;
    [SerializeField] public Transform Prefab;
    [SerializeField] public float detectionRadius;
    [SerializeField] public float attackDistance;
    [SerializeField] public float movementSpeed;
    [SerializeField] public float maxHealth;
    [SerializeField] public List<LootDropEntry> lootTable;
    public NPCData CreateInstance(){
        return (NPCData)this.MemberwiseClone();
    }

}


