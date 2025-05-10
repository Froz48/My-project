using Unity.Netcode;
using UnityEngine;

public abstract class Ability
{

    public AbilityData abilityData;
    public float nextUseTime;

    [ServerRpc]
    public abstract void AbilityUseServerRpc(Vector2 playerPosition, Vector2 targetPosition);

    public Ability CreateInstance(){
         return (Ability)this.MemberwiseClone();
    }

    public float GetRemainingCooldown(){
        return Mathf.Max(0, nextUseTime - Time.time);
    }
}