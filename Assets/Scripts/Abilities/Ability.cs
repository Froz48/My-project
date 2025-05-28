using Unity.Netcode;
using UnityEngine;


public abstract class Ability : ScriptableObject, IDatabaseObject
{
    int id;
    public float nextUseTime;
    [SerializeField] public Sprite sprite;
    [SerializeField] public float power = 1f;
    [SerializeField] public float cooldown = 1f;

    [ServerRpc]
    public abstract void AbilityUseServerRpc(Vector2 playerPosition, Vector2 targetPosition);

    public Ability CreateInstance(){
         return (Ability)this.MemberwiseClone();
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