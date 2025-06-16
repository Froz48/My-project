using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public abstract class Ability : ScriptableObject, IDatabaseObject
{

    public int id;
    [SerializeField] private float nextUseTime;
    [SerializeField] public Sprite sprite;
    [SerializeField] public float power = 1f;
    [SerializeField] public float cooldown = 1f;
    public bool initialized = false;
    public abstract void AbilityUse(Vector2 playerPosition, Vector2 targetPosition);

    public void StartCooldown()
    {
        if (initialized)
            nextUseTime = Time.time + cooldown;
        else
            Debug.LogError("Trying to change base ability nextUseTime of " + name);
    }

    public float getCurrentCooldown()
    {
        return Time.time - nextUseTime;
    }
    public bool IsReady()
    {
        return getCurrentCooldown() >= 0;
    }
    public Ability CreateInstance()
    {
        Ability ability = (Ability)this.MemberwiseClone();
        ability.initialized = true;
        return ability;
    }

    public int GetId()
    {
        return id;
    }

    public float GetRemainingCooldown(){
        return Mathf.Max(0, nextUseTime - Time.time);
    }

    public void SetId(int id)
    {
        this.id = id;
    }
}