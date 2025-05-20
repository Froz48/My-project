

using System.Collections;
using Unity.Netcode;
using UnityEngine;
[CreateAssetMenu(fileName = "Flamestrike", menuName = "Abilities/Flamestrike")]
public class Flamestrike : Ability{
    public GameObject prefab;
    public float lifetime = 1f;

    [ServerRpc]
    public override void AbilityUseServerRpc(Vector2 playerPosition, Vector2 targetPosition){
        GameObject flamestrikeObject = GameManager.Instantiate(prefab, targetPosition, Quaternion.identity);
        flamestrikeObject.AddComponent<Effect_DamageOnCollision>().Initialize(power, typeof(NPCEntity));
        flamestrikeObject.AddComponent<Effect_DamageOnCollision>().Initialize(power, typeof(BossEntity));
        flamestrikeObject.AddComponent<Effect_DestroyAfterDelay>().Initialize(lifetime);
        
        var networkObj = flamestrikeObject.GetComponent<NetworkObject>();
        networkObj.Spawn();
        nextUseTime = Time.time + cooldown;
    }
}