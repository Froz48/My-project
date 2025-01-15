using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] public ItemBase[] ItemObjects;

    [ContextMenu("Update ID's")]
    private void UpdateID()
    {
        Debug.Log("UpdatingDatabaseItemIds");
        for (int i = 0; i < ItemObjects.Length; i++)
        {
            if (ItemObjects[i].Id != i)
                ItemObjects[i].Id = i;
        }
    }

    public ItemBase GetItem(int id){
        return ItemObjects[id];
    }
}