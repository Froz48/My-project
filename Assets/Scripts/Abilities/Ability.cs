using Unity.Netcode;
using UnityEngine;


public abstract class Ability : ScriptableObject
{
    public float nextUseTime;
    [SerializeField] public Sprite sprite;
    [SerializeField] public float power = 1f;
    [SerializeField] public float cooldown = 1f;

    [ServerRpc]
    public abstract void AbilityUseServerRpc(Vector2 playerPosition, Vector2 targetPosition);

    public Ability CreateInstance(){
         return (Ability)this.MemberwiseClone();
    }

    public float GetRemainingCooldown(){
        return Mathf.Max(0, nextUseTime - Time.time);
    }
}