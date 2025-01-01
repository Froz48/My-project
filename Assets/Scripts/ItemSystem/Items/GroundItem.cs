using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GroundItem : NetworkBehaviour
{
    private ItemObject item;
    [SerializeField] private static GameObject groundItemPrefab;

    public void OnAfterDeserialize()
    {
    }
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
