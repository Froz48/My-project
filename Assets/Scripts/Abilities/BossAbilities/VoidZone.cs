using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class VoidZone : Ability
{
    AbilityData abilityDataProjectile;
    public GameObject activationPrefab;
    public GameObject warningPrefab;
    public float warningDuration = 2f; 

    [ServerRpc]
    public override void AbilityUseServerRpc(Vector2 startPosition, Vector2 targetPosition){
        GameObject warningGO = GameManager.Instantiate(warningPrefab, targetPosition, Quaternion.identity);
        var networkObj = warningGO.GetComponent<NetworkObject>();
        networkObj.Spawn();
        warningGO.AddComponent<Effect_DestroyAfterDelay>().Initialize(warningDuration); 
        GameManager.Instance.StartCoroutine(DelayedActivation(targetPosition));
    }

    private IEnumerator DelayedActivation(Vector2 targetPosition){
        yield return new WaitForSeconds(warningDuration);

        GameObject activationGO = GameManager.Instantiate(activationPrefab, targetPosition, Quaternion.identity);
        activationGO.AddComponent<Effect_DamageOnCollisionToPlayer>().Initialize(abilityData.power);
        activationGO.AddComponent<Effect_DestroyAfterDelay>().Initialize(abilityDataProjectile.lifetime);
        
        NetworkObject activationNetObj = activationGO.GetComponent<NetworkObject>();
        activationNetObj.Spawn();

    }
}
