
using Unity.Netcode;
using UnityEngine;

public abstract class Ability : ScriptableObject {

    [SerializeField] public Sprite sprite;
    [SerializeField]public float damage = 1f;
    [SerializeField]public float cooldown = 1f;
    [SerializeField]public float nextUseTime;
    [SerializeField]public float lifetime = 1f;

    [ServerRpc]
    public abstract void AbilityUseServerRpc(Vector2 playerPosition, Vector2 targetPosition);
    
    public float GetRemainingCooldown()
    {
        if (nextUseTime == float.NaN) return 0;
        return Mathf.Max(0, nextUseTime - Time.time);
    }
    public Ability CreateInstance(){
        return (Ability)this.MemberwiseClone();
    }

}