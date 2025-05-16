using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
[CreateAssetMenu(fileName = "VoidZone", menuName = "Abilities/VoidZone")]
public class VoidZone : Ability
{
    public float lifetime = 1f;
    public GameObject activationPrefab;
    public GameObject warningPrefab;
    public float warningDuration = 2f; 

    [ServerRpc]
    public override void AbilityUseServerRpc(Vector2 startPosition, Vector2 targetPosition){
        GameObject warningGO = Instantiate(warningPrefab, targetPosition, Quaternion.identity);
        var networkObj = warningGO.GetComponent<NetworkObject>();
        networkObj.Spawn();
        warningGO.AddComponent<Effect_DestroyAfterDelay>().Initialize(warningDuration); 
        GameManager.Instance.StartCoroutine(DelayedActivation(targetPosition));
        nextUseTime = Time.time + cooldown;
    }

    private IEnumerator DelayedActivation(Vector2 targetPosition){
        yield return new WaitForSeconds(warningDuration);

        GameObject activationGO = Instantiate(activationPrefab, targetPosition, Quaternion.identity);
        activationGO.AddComponent<Effect_DamageOnCollisionToPlayer>().Initialize(power);
        activationGO.AddComponent<Effect_DestroyAfterDelay>().Initialize(lifetime);
        
        NetworkObject activationNetObj = activationGO.GetComponent<NetworkObject>();
        activationNetObj.Spawn();

    }
}
