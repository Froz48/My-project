

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[Serializable]
[CreateAssetMenu(fileName = "MonsterData", menuName = "NPC/MonsterData")]
public class MonsterData : ScriptableObject{
    [SerializeField] public Transform Prefab;
    [SerializeField] public float detectionRadius;
    [SerializeField] public float attackDamage;
    [SerializeField] public float atackSpeed;
    [SerializeField] public float giveUpRadius;
    [SerializeField] public float movementSpeed;
    [SerializeField] public float maxHealth;
    [SerializeField] public List<LootDropEntry> lootDrop;
    public int Id = -1;

    public MonsterData CreateInstance(){
        return (MonsterData)this.MemberwiseClone();
    }

}

[System.Serializable]
public class LootDropEntry
{
    public ItemObject item;
    public float dropChance;
}
