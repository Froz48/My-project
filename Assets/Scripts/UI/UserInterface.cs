

using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour{
    public Dictionary<GameObject, InventorySlot> slotsOnInterface;
    public GameObject slotPrefab;
    public InventoryBase inventory;

    public virtual void makeUI(InventoryBase _inventory, int columns = 10, int _spacing = 2){
        inventory = _inventory;
        slotsOnInterface = new Dictionary<GameObject, InventorySlot>();
        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });
        inventory.onItemUpdate += UpdateUI;
    }

    // public override void OnNetworkSpawn()
    // {
    //     if (!IsOwner){
    //         gameObject.SetActive(false);
    //     }
    // }
    private void EquipItem(InventorySlot slot)
    {
        if (slot.item is EquipmentItem)
        {
            Player player = GetComponentInParent<Player>();
            inventory.SwapItems(slot, player.GetInventory().GetEmptySlot());
        }
    }
    

    protected void UpdateUI(object sender, EventArgs e){
        foreach (var slot in slotsOnInterface){
            if (slot.Value.item == null){
                slot.Key.transform.GetChild(0).GetComponent<Image>().sprite = null;
                slot.Key.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
                slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = "";
            } 
            else
            {
                slot.Key.transform.GetChild(0).GetComponent<Image>().sprite = slot.Value.item.uiDisplay;
                slot.Key.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = slot.Value.amount.ToString("n0");
            }
        }
    }
    public void OnDrag(GameObject obj)
    {
        if (MouseData.tempItemBeingDragged != null)
            MouseData.tempItemBeingDragged.GetComponent<RectTransform>().position = Input.mousePosition;
    }
    public void OnDragStart(GameObject obj)
    {
        MouseData.tempItemBeingDragged = CreateTempItem(obj);
        TooltipInterface.HideTooltip();
    }
    public void OnExit(GameObject obj)
    {
        MouseData.slotHoveredOver = null;
        TooltipInterface.HideTooltip();
    }
    public GameObject CreateTempItem(GameObject obj)
    {
        GameObject tempItem = null;
        if(!slotsOnInterface[obj].IsEmpty())
        {
            tempItem = new GameObject("DraggedItem");
            var rt = tempItem.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(slotPrefab.GetComponent<RectTransform>().rect.width*4, slotPrefab.GetComponent<RectTransform>().rect.height*4);
           // Debug.Log(transform.GetInstanceID());
            tempItem.transform.SetParent(transform.parent.transform, false);
            var img = tempItem.AddComponent<Image>();
            img.sprite = slotsOnInterface[obj].item.uiDisplay;
            img.raycastTarget = false;
        }
        return tempItem;
    }
    public void OnEnterInterface(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = obj.GetComponent<UserInterface>();
    }
    public void OnExitInterface(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = null;
    }
    public void OnEnter(GameObject obj)
    {
        MouseData.slotHoveredOver = obj;
        if (!slotsOnInterface[obj].IsEmpty()){
            TooltipInterface.ShowTooltip(obj, slotsOnInterface[obj].item);
        }
    }
    public void OnRightClick(GameObject obj)
    {
        if (!slotsOnInterface[obj].IsEmpty())
        {
        EquipItem(slotsOnInterface[obj]);
        TooltipInterface.HideTooltip();
        }
    }
    public void OnDragEnd(GameObject obj)
    {
        Destroy(MouseData.tempItemBeingDragged);
        // if (MouseData.interfaceMouseIsOver == null)
        // {
        //     Debug.Log("MouseData.interfaceMouseIsOver == null");
        //     slotsOnInterface[obj].RemoveItem();
        //     return;
        // }
        if (MouseData.slotHoveredOver)
        {
            InventorySlot mouseHoverSlotData = MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoveredOver];
            inventory.SwapItems(slotsOnInterface[obj], mouseHoverSlotData);

        }
    }

    public void AddEvents(GameObject child){
        AddEvent(child, EventTriggerType.PointerEnter, delegate { OnEnter(child); });
        AddEvent(child, EventTriggerType.PointerExit, delegate { OnExit(child); });
        AddEvent(child, EventTriggerType.BeginDrag, delegate { OnDragStart(child); });
        AddEvent(child, EventTriggerType.EndDrag, delegate { OnDragEnd(child); });
        AddEvent(child, EventTriggerType.Drag, delegate { OnDrag(child); });
        AddEvent(child, EventTriggerType.PointerClick, (data) => 
            {
                PointerEventData pointerData = (PointerEventData)data;
                if (pointerData.button == PointerEventData.InputButton.Right)
                    {OnRightClick(child);}
            }); // gpt solved, no idea what does this mean
    }
    protected void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }
}