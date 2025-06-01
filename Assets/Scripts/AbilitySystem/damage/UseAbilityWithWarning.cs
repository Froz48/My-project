using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
[CreateAssetMenu(fileName = "UseAbilityWithWarning", menuName = "Abilities/UseAbilityWithWarning")]
public class UseAbilityWithWarning : Ability
{
    [Header("Settings")]
    public Ability ability;

    public void Start()
    {
        ability = ability.CreateInstance();
    }
    public float warningDuration = 2f;
    public float effectRadius = 1;

    [Header("Prefabs")]
    public GameObject warningPrefab;

    [ServerRpc]
    public override void AbilityUseServerRpc(Vector2 startPosition, Vector2 targetPosition){
        nextUseTime = Time.time + cooldown;
        GameObject warningGO = Instantiate(warningPrefab, targetPosition, Quaternion.identity);
        warningGO.transform.localScale = Vector3.one * effectRadius; 

        CircleWarning warningAnim = warningGO.GetComponent<CircleWarning>();
        warningAnim.SetDuration(warningDuration);

        warningGO.GetComponent<NetworkObject>().Spawn();

        warningGO.AddComponent<Effect_DestroyAfterDelay>().Initialize(warningDuration); 

        GameManager.Instance.StartCoroutine(DelayedActivation(startPosition, targetPosition));
    }

    private IEnumerator DelayedActivation(Vector2 startPosition, Vector2 targetPosition){
        yield return new WaitForSeconds(warningDuration);
        ability.AbilityUseServerRpc(startPosition, targetPosition);
    }
}
