using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ReviveManager : NetworkBehaviour
{
    public void Kill(GameObject playerGameObject, float reviveTime){
        
        StartCoroutine(revive(playerGameObject, reviveTime));
    }    

    public IEnumerator revive(GameObject gameObject, float time){
        SetPlayerObjectState(gameObject, false);
        // gameObject.SetActive(false);
        yield return new WaitForSeconds(time);
        SetPlayerObjectState(gameObject, true);
        Debug.Log("Revive");
        gameObject.GetComponent<Player>().Revive(); 
    }

    public void SetPlayerObjectState(GameObject go, bool state){
        go.GetComponentInChildren<SpriteRenderer>().enabled = state;
        go.GetComponentInChildren<Collider2D>().enabled = state;
        go.GetComponentInChildren<Animator>().enabled = state;
        go.GetComponentInChildren<Rigidbody2D>().simulated = state;
        go.GetComponent<Player>().enabled = state;
    }

}
