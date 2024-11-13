using Unity.Netcode;
using UnityEngine;

public abstract class PlaceableObject : NetworkBehaviour{
    public bool canWalkThrough;
    public GameObject UI;
    public InventoryObject inventory;

    //public abstract PlaceableObject CreateInstance();



}