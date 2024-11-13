

using System.Collections;
using Unity.Netcode;
using UnityEngine;
[CreateAssetMenu(fileName = "Flamestrike", menuName = "Abilities/Flamestrike")]
public class Flamestrike : Ability{
    [SerializeField] private GameObject prefab;

    [ServerRpc]
    public override void AbilityUseServerRpc(Vector2 _position){
        GameObject flamestrikeObject = Instantiate(prefab, _position, Quaternion.identity);
        flamestrikeObject.AddComponent<FlamestrikeEffect>().Initialize(damage, lifetime);
        
        var networkObj = flamestrikeObject.GetComponent<NetworkObject>();
        networkObj.Spawn();
    }


}