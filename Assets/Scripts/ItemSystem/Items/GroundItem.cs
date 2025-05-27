using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GroundItem : NetworkBehaviour
{
    private Item item;
    [SerializeField] private static GameObject groundItemPrefab;


    public void setItem(Item itemObject){
        item = itemObject;
        GetComponent<SpriteRenderer>().sprite = item.uiDisplay;
    }

    public Item getItem(){
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
