

using System.Collections;
using Unity.Netcode;
using UnityEngine;
[CreateAssetMenu(fileName = "Flamestrike", menuName = "Abilities/Flamestrike")]
public class Flamestrike : Ability{
    [SerializeField] private GameObject prefab;

    [ServerRpc]
    public override void AbilityUseServerRpc(Vector2 playerPosition, Vector2 targetPosition){
        Debug.Log("Flamestrike ServerRpc");
        GameObject flamestrikeObject = Instantiate(prefab, targetPosition, Quaternion.identity);
        flamestrikeObject.AddComponent<Effect_DamageOnCollision>().Initialize(damage);
        flamestrikeObject.AddComponent<Effect_DestroyAfterDelay>().Initialize(lifetime);
        
        var networkObj = flamestrikeObject.GetComponent<NetworkObject>();
        networkObj.Spawn();
    }
}