using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
[CreateAssetMenu(fileName = "ObjectSpawn", menuName = "Abilities/ObjectSpawn")]
public class ObjectSpawn : Ability
{
    public GameObject objectPrefab;
    public bool spawnOnCaster = false;

    [ServerRpc]
    public override void AbilityUseServerRpc(Vector2 playerPosition, Vector2 targetPosition)
    {
        Vector2 startingPos;
        if (spawnOnCaster)
        {
            startingPos = playerPosition;
        }
        else
        {
            startingPos = targetPosition;
        }

        GameObject spawnedObject = Instantiate(objectPrefab, startingPos, Quaternion.identity);
        spawnedObject.GetComponent<EffectController>()?.Initialize(playerPosition, targetPosition);
        spawnedObject.GetComponent<NetworkObject>()?.Spawn();
        nextUseTime = Time.time + cooldown;
    }
}