

using System.Collections;
using Unity.Netcode;
using UnityEngine;
public class Flamestrike : Ability{

    [ServerRpc]
    public override void AbilityUseServerRpc(Vector2 playerPosition, Vector2 targetPosition){
        GameObject flamestrikeObject = GameManager.Instantiate(abilityData.projectilePrefab, targetPosition, Quaternion.identity);
        flamestrikeObject.AddComponent<Effect_DamageOnCollisionToMonster>().Initialize(abilityData.power);
        flamestrikeObject.AddComponent<Effect_DestroyAfterDelay>().Initialize(abilityData.lifetime);
        
        var networkObj = flamestrikeObject.GetComponent<NetworkObject>();
        networkObj.Spawn();
    }
}