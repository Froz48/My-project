

using System.Collections;
using Unity.Netcode;
using UnityEngine;
[CreateAssetMenu(fileName = "MeleeStrike", menuName = "Abilities/MeleeStrike")]
public class MeleeStrike : Ability{
    public GameObject prefab;
    public float lifetime = 1f;


    [ServerRpc]
    public override void AbilityUseServerRpc(Vector2 playerPosition, Vector2 targetPosition){
        Vector2 direction = (targetPosition - playerPosition).normalized;
        Vector2 pos = playerPosition - (playerPosition - targetPosition).normalized;
        GameObject effectObject = GameManager.Instantiate(prefab, pos, Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward));
        effectObject.AddComponent<Effect_DamageOnCollision>().Initialize(power, typeof(NPCEntity));
        effectObject.AddComponent<Effect_DamageOnCollision>().Initialize(power, typeof(BossEntity));
        effectObject.AddComponent<Effect_DestroyAfterDelay>().Initialize(lifetime);
        
        effectObject.GetComponent<NetworkObject>().Spawn();
        nextUseTime = Time.time + cooldown;
    }


}