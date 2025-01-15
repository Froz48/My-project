

using System.Collections;
using Unity.Netcode;
using UnityEngine;
[CreateAssetMenu(fileName = "Flamestrike", menuName = "Abilities/Flamestrike")]
public class MeleeStrike : Ability{
    [SerializeField] private GameObject prefab;

    [ServerRpc]
    public override void AbilityUseServerRpc(Vector2 _position){
        GameObject effectObject = Instantiate(prefab, _position, Quaternion.identity);
        effectObject.AddComponent<MeleeStrikeEffect>().Initialize(damage, lifetime);
        
        var networkObj = effectObject.GetComponent<NetworkObject>();
        networkObj.Spawn();
    }


}