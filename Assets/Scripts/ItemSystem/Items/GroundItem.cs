using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GroundItem : NetworkBehaviour
{
    private Item item;
    [SerializeField] private static GameObject groundItemPrefab;

    [ClientRpc]
    public void SetItemClientRpc(int itemId){
        item = (Resources.Load(Config.DATABASE_ITEM_NAME) as Database).GetObjectById(itemId) as Item;
        GetComponent<SpriteRenderer>().sprite = item.sprite;
    }

    public Item GetItem(){
        return item;
    }
    public void OnAfterDeserialize(){}
    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
        GetComponent<SpriteRenderer>().sprite = item.sprite;
        EditorUtility.SetDirty(GetComponentInChildren<SpriteRenderer>());
#endif
    }
}
