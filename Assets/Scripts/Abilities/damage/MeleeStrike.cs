

using System.Collections;
using Unity.Netcode;
using UnityEngine;
[CreateAssetMenu(fileName = "MeleeStrike", menuName = "Abilities/MeleeStrike")]
public class MeleeStrike : Ability{
    public GameObject prefab;
    public float lifetime = 1f;


    [ServerRpc]
    public override void AbilityUseServerRpc(Vector2 playerPosition, Vector2 targetPosition){

        Vector2 pos = playerPosition - (playerPosition - targetPosition).normalized;
        GameObject effectObject = GameManager.Instantiate(prefab, pos, Quaternion.identity);
        effectObject.AddComponent<Effect_DamageOnCollisionToMonster>().Initialize(power);
        effectObject.AddComponent<Effect_DestroyAfterDelay>().Initialize(lifetime);
        
        effectObject.GetComponent<NetworkObject>().Spawn();
        // networkObj.Spawn();`
    }


}