using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
[CreateAssetMenu(fileName = "ObjectSpawnWithWarning", menuName = "Abilities/ObjectSpawnWithWarning")]
public class ObjectSpawnWithWarning : Ability
{
    public float warningDuration = 2f;
    public float effectRadius = 1;

    [Header("Prefabs")]
    public GameObject warningPrefab;
    public GameObject AbilityPrefab;

    [ServerRpc]
    public override void AbilityUseServerRpc(Vector2 startPosition, Vector2 targetPosition){
        nextUseTime = Time.time + cooldown;
        GameObject warningGO = Instantiate(warningPrefab, targetPosition, Quaternion.identity);
        warningGO.transform.localScale = Vector3.one * effectRadius; 

        warningGO.GetComponent<CircleWarning>().SetDuration(warningDuration);
        warningGO.GetComponent<Effect_DestroyAfterDelay>().delay = warningDuration;
        
        warningGO.GetComponent<NetworkObject>().Spawn();
 

        GameManager.Instance.StartCoroutine(DelayedActivation(startPosition, targetPosition));
    }

    private IEnumerator DelayedActivation(Vector2 startPosition, Vector2 targetPosition){
        yield return new WaitForSeconds(warningDuration);
        GameObject spawnedObject = Instantiate(AbilityPrefab, targetPosition, Quaternion.identity);
        spawnedObject.GetComponent<EffectController>().Initialize(startPosition, targetPosition);
        spawnedObject.GetComponent<NetworkObject>()?.Spawn();
        nextUseTime = Time.time + cooldown;
    }
}
