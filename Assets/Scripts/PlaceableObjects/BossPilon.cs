using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class BossPilon : PlaceableObject, IPointerClickHandler
{   
    BossEntity boss;
    private int lvl;
    public void OnPointerClick(PointerEventData eventData)
    {
        CloseArenaServerRpc();
        StartFight();
    }

    public void StartFight(){
        
    }

    public void EndFight(bool isWon){
        CloseArenaServerRpc();
        if(isWon){
            lvl++;
            
        }

    }

    public void OpenArena(){

    }

    [ServerRpc]
    public void CloseArenaServerRpc(){
        
    }
}
