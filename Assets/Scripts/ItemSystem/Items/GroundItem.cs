using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GroundItem : NetworkBehaviour
{
    private ItemObject item;
    [SerializeField] private static GameObject groundItemPrefab;
    
   // private ItemObject itemBase;

    // public void CreateGroundItemInitialized(int itemId){
    //     item = ItemObject.CreateInstanceInitialized(itemId);
    // }
    // public void CreateGroundItemInitialized(ItemObject _itemBase){
    //     item = ItemObject.CreateInstanceInitialized(_itemBase);
       
    // }
    public void OnAfterDeserialize()
    {
    }

    // public static void CreateGroundItemNew(int itemId, Vector3 position){

    //     ItemObject item = ItemObject.CreateInstanceInitialized(itemId);
    //     GameObject _gameObject = new GameObject("GroundItem");
    //     _gameObject.transform.position = position;
    //     //GameObject _gameObject = Instantiate(groundItemPrefab, position, Quaternion.identity);
    //     _gameObject.AddComponent<GroundItem>().setItem(item);
    //     _gameObject.AddComponent<SpriteRenderer>().sprite = item.uiDisplay;
    //     _gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
    //     _gameObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    //     _gameObject.AddComponent<NetworkObject>().Spawn();
    // }


    public void setItem(ItemObject itemObject){
        item = itemObject;
    }

    public ItemObject getItem(){
        return item;
    }
    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
        GetComponent<SpriteRenderer>().sprite = item.uiDisplay;
        EditorUtility.SetDirty(GetComponentInChildren<SpriteRenderer>());
#endif
    }
}
