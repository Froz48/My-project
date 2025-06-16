using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReviveManager : NetworkBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    public void KillPlayerServerRpc(ulong networkObjectId, float reviveTime)
    {
        KillPlayerClientRpc(networkObjectId, reviveTime);
    }

    [ClientRpc]
    public void KillPlayerClientRpc(ulong networkObjectId, float reviveTime)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
        {
            if (networkObject.IsOwner)
                StartCoroutine(reviveOwner(networkObject.gameObject, reviveTime));
            else
                StartCoroutine(reviveOthers(networkObject.gameObject, reviveTime));
        }

    }

    public IEnumerator reviveOwner(GameObject playerObject, float time)
    {
        SetPlayerObjectStateOwner(playerObject, false);
        yield return new WaitForSeconds(time);
        SetPlayerObjectStateOwner(playerObject, true);
        playerObject.GetComponent<Player>().Revive();
        // EnablePointerClickComponents(playerObject, true);
    }
    // private void EnablePointerClickComponents(GameObject playerObject, bool enable)
    // {
    //     // Включаем все компоненты, необходимые для обработки кликов
    //     var eventSystem = playerObject.GetComponent<EventSystem>();
    //     if (eventSystem != null) eventSystem.enabled = enable;
        
    //     var physicsRaycaster = playerObject.GetComponent<Physics2DRaycaster>();
    //     if (physicsRaycaster != null) physicsRaycaster.enabled = enable;
    // }
    public IEnumerator reviveOthers(GameObject gameObject, float time)
    {
        SetPlayerObjectStateOthers(gameObject, false);
        yield return new WaitForSeconds(time);
        SetPlayerObjectStateOthers(gameObject, true);
    }
    public void SetPlayerObjectStateOwner(GameObject go, bool state)
    {
        go.GetComponentInChildren<MapGen>().enabled = state;
        go.GetComponentInChildren<NetworkAnimator>().enabled = state;
        go.GetComponentInChildren<SpriteRenderer>().enabled = state;
        go.GetComponentInChildren<PlayerInputController>().enabled = state;
        go.GetComponentInChildren<Collider2D>().enabled = state;
        go.GetComponentInChildren<Animator>().enabled = state;
        go.GetComponentInChildren<Rigidbody2D>().simulated = state;
        go.GetComponent<Player>().enabled = state;
    }
    public void SetPlayerObjectStateOthers(GameObject go, bool state){
        go.GetComponentInChildren<NetworkAnimator>().enabled = state;
        go.GetComponentInChildren<SpriteRenderer>().enabled = state;
        go.GetComponentInChildren<Collider2D>().enabled = state;
        go.GetComponentInChildren<Animator>().enabled = state;
        go.GetComponentInChildren<Rigidbody2D>().simulated = state;
        go.GetComponent<Player>().enabled = state;
    }

}
