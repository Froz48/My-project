using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

[Serializable]
public class InventoryUI : UserInterface {
    int NUMBER_OF_COLUMN;
    int spacing = 2;

    public override void makeUI(InventoryObject _inventory, int columns = 10, int _spacing = 2){
        NUMBER_OF_COLUMN = columns;
        spacing = _spacing;
        base.makeUI(_inventory);
        for (int i = 0; i < inventory.Container.Length; i++){
            slotsOnInterface.Add(makeSlot(i), inventory.Container[i]);
           
        }
        UpdateUI(this, EventArgs.Empty);
    }

    
    private GameObject makeSlot(int i, string _name = "InventorySlot"){
        GameObject slot = Instantiate(slotPrefab, transform);
        slot.name =  _name + i;
        slot.transform.SetParent(transform);
        slot.transform.localPosition = GetSlotPosition(i);
        AddEvents(slot);
        return slot;
    }

    private Vector3 GetSlotPosition(int i){
        Rect rectItem = slotPrefab.GetComponent<RectTransform>().rect;
        float x = (i % NUMBER_OF_COLUMN + 1) * (rectItem.width + spacing);
        float y= -(i / NUMBER_OF_COLUMN + 1) * (rectItem.height + spacing);

        return new Vector3(x, y, 1);
    }


}