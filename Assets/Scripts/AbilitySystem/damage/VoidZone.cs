using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
[CreateAssetMenu(fileName = "VoidZone", menuName = "Abilities/VoidZone")]
public class VoidZone : Ability
{   [Header("Settings")]
    public float lifetime = 1f;
    public float warningDuration = 2f;
    public float effectRadius = 1;

    [Header("Prefabs")]
    public GameObject activationPrefab;
    public GameObject warningPrefab;

    [ServerRpc]
    public override void AbilityUseServerRpc(Vector2 startPosition, Vector2 targetPosition){
        GameObject warningGO = Instantiate(warningPrefab, targetPosition, Quaternion.identity);
        warningGO.transform.localScale = Vector3.one * effectRadius; 

        CircleWarning warningAnim = warningGO.GetComponent<CircleWarning>();
        warningAnim.SetDuration(warningDuration);

        warningGO.GetComponent<NetworkObject>().Spawn();

        warningGO.AddComponent<Effect_DestroyAfterDelay>().Initialize(warningDuration); 

        GameManager.Instance.StartCoroutine(DelayedActivation(targetPosition));
        nextUseTime = Time.time + cooldown;
    }

    private IEnumerator DelayedActivation(Vector2 targetPosition){
        yield return new WaitForSeconds(warningDuration);

        GameObject activationGO = Instantiate(activationPrefab, targetPosition, Quaternion.identity);
        activationGO.transform.localScale = Vector3.one * effectRadius;
        activationGO.AddComponent<Effect_DamageOnCollision>().Initialize(power, typeof(Player));
        activationGO.AddComponent<Effect_DestroyAfterDelay>().Initialize(lifetime);
        
        NetworkObject activationNetObj = activationGO.GetComponent<NetworkObject>();
        activationNetObj.Spawn();

    }
}
