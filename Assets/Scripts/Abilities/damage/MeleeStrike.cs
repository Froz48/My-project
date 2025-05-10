

using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class MeleeStrike : Ability{
    [ServerRpc]
    public override void AbilityUseServerRpc(Vector2 playerPosition, Vector2 targetPosition){

        Vector2 pos = playerPosition - (playerPosition - targetPosition).normalized;
        GameObject effectObject = GameManager.Instantiate(abilityData.projectilePrefab, pos, Quaternion.identity);
        effectObject.AddComponent<Effect_DamageOnCollisionToMonster>().Initialize(abilityData.power);
        effectObject.AddComponent<Effect_DestroyAfterDelay>().Initialize(abilityData.lifetime);
        
        effectObject.GetComponent<NetworkObject>().Spawn();
        // networkObj.Spawn();`
    }


}