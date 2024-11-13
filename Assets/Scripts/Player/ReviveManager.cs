using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ReviveManager : NetworkBehaviour
{
    public void Kill(GameObject playerGameObject, float reviveTime){
        
        StartCoroutine(revive(playerGameObject, reviveTime));
        playerGameObject.GetComponent<Player>().Revive(); 
    }    

    public IEnumerator revive(GameObject gameObject, float time){
        gameObject.SetActive(false);
        yield return new WaitForSeconds(time);
        Debug.Log("Revive");
        gameObject.SetActive(true);
    }
}
