

using System.Collections;
using Unity.Netcode;
using UnityEngine;
[CreateAssetMenu(fileName = "Flamestrike", menuName = "Abilities/Flamestrike")]
public class MeleeStrike : Ability{
    [SerializeField] private GameObject prefab;

    [ServerRpc]
    public override void AbilityUseServerRpc(Vector2 playerPosition, Vector2 targetPosition){
        GameObject effectObject = Instantiate(prefab, targetPosition, Quaternion.identity);
        effectObject.AddComponent<MeleeStrikeEffect>().Initialize(damage, lifetime);
        
        var networkObj = effectObject.GetComponent<NetworkObject>();
        networkObj.Spawn();
    }


}