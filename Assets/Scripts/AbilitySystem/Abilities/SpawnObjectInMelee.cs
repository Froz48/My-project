

using System.Collections;
using Unity.Netcode;
using UnityEngine;
[CreateAssetMenu(fileName = "MeleeStrike", menuName = "Abilities/MeleeStrike")]
public class SpawnObjectInMelee : Ability{
    public GameObject prefab;
    public override void AbilityUse(Vector2 playerPosition, Vector2 targetPosition)
    {
        Debug.Log("AbilityUseClientRpc");
        Vector2 direction = (targetPosition - playerPosition).normalized;
        Vector2 pos = playerPosition - (playerPosition - targetPosition).normalized;
        GameObject effectObject = Instantiate(prefab, pos, Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward));
        effectObject.GetComponent<EffectController>()?.Initialize(playerPosition, targetPosition);
        // effectObject.GetComponent<NetworkObject>().Spawn();
    }


}