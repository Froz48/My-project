
using System;
using UnityEngine;

public class EquipmentUI : UserInterface{
    int NUMBER_OF_COLUMN = 3;
    int spacing = 2;

    public override void makeUI(InventoryBase _equipment, int columns = 10, int _spacing = 2){
        base.makeUI(_equipment);
        for (int i = 0; i < inventory.Slots.Length; i++){
        slotsOnInterface.Add(makeSlot(i), inventory.Slots[i]);
           
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
        float x = rectItem.width; // x 1-го слота
        float y = rectItem.height; // y 1-го слота

        if (i == 0){
            x += (rectItem.width + spacing)*1;
            y += (rectItem.height + spacing)*3;
            return new Vector3(x, y, 0);
        } 
    
        y += (NUMBER_OF_COLUMN-1) * (rectItem.height + spacing); // слот chest

        x+= (i-1) % NUMBER_OF_COLUMN * (rectItem.width + spacing);
        y-= (i-1) / NUMBER_OF_COLUMN * (rectItem.height + spacing);
        if (i == 8){
            x += rectItem.width + spacing;
        }
        return new Vector3(x, y, 1);
    }
}