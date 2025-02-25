
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class Chest : PlaceableObject, IPointerClickHandler
{
    private bool isOpened = false;
    public void Start(){

    }


    [ServerRpc]
    public void OpenChestServerRpc(){
        

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isOpened){
            GetComponentInChildren<InventoryUI>().makeUI(inventory, columns: 5);
            isOpened = true;
        }
        else {
            foreach (Transform child in GetComponentInChildren<InventoryUI>().transform){
                Destroy(child.gameObject);
            }
            isOpened = false;
        }
    }
}