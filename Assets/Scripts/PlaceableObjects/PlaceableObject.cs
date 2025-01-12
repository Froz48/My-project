using Unity.Netcode;
using UnityEngine;

public abstract class PlaceableObject : NetworkBehaviour{
    public bool canWalkThrough;
    public GameObject UI;
    public InventoryBase inventory;

    //public abstract PlaceableObject CreateInstance();



}