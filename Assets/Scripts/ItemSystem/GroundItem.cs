using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GroundItem : NetworkBehaviour
{
    private ItemBase item;
    [SerializeField] private static GameObject groundItemPrefab;


    public void setItem(ItemBase itemObject){
        item = itemObject;
    }

    public ItemBase getItem(){
        return item;
    }
    public void OnAfterDeserialize(){}
    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
        GetComponent<SpriteRenderer>().sprite = item.uiDisplay;
        EditorUtility.SetDirty(GetComponentInChildren<SpriteRenderer>());
#endif
    }
}
