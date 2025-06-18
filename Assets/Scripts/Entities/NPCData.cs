

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D.Animation;
[Serializable]
[CreateAssetMenu(fileName = "MonsterData", menuName = "NPC/MonsterData")]
public class NPCData : ScriptableObject, IDatabaseObject
{
    [SerializeField] private int id;
    [SerializeField] public NPCBehaviour[] nPCBehaviour;
    [SerializeField] public Ability[] abilities;
    [SerializeField] public float detectionRadius;
    [SerializeField] public float attackDistance;
    [SerializeField] public float movementSpeed;
    [SerializeField] public SpriteLibraryAsset spriteLibraryAsset;
    [SerializeField] public float maxHealth;
    [SerializeField] public List<LootDropEntry> lootTable;
    [SerializeField] public float sizeScale = 1f;
    public NPCData CreateInstance(){
        NPCData instance = Instantiate(this);
        instance.name = this.name + " Instance";
        return instance;
    }

    public int GetId()
    {
        return id;
    }

    public void SetId(int id)
    {
        this.id = id;
        Debug.Log("Setting id " + id + " to " + this.name);
    }
}


