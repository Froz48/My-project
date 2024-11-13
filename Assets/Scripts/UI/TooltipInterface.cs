using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public static class TooltipInterface
{
    static public GameObject tooltipPrefab = Resources.Load<GameObject>("TooltipPrefab");
    static private GameObject currentTooltip;
    public static void ShowTooltip(GameObject slot, ItemObject itemObject)
    {
            if (currentTooltip == null)  createTooltip();
            setPostition(slot);
            setText(itemObject);
            currentTooltip.SetActive(true);
        
    }
    private static void createTooltip(){
        currentTooltip = Object.Instantiate(tooltipPrefab, new Vector3(), Quaternion.identity);
    }

    private static void setText(ItemObject itemObject){
        currentTooltip.GetComponentInChildren<TextMeshProUGUI>().text = "";

        currentTooltip.GetComponentInChildren<TextMeshProUGUI>().text += 
            $"<align=center><size=30><color=yellow> {itemObject.Name} </color></size></align>\n";   

        string modifiersText = "";
        for (int i = 0; i < itemObject.buffs.Length; i++){
            modifiersText += itemObject.buffs[i].attribute + " " + itemObject.buffs[i].getValue() + "\n";
        }
        currentTooltip.GetComponentInChildren<TextMeshProUGUI>().text += 
        $"<size=25><color=red> {modifiersText} </color></size>";

        currentTooltip.GetComponentInChildren<TextMeshProUGUI>().text += $"<size=20> {itemObject.description}</size>\n";
    }

    private static void setPostition(GameObject slot){
        Vector3 pos = slot.transform.position;
            var rectMain = currentTooltip.transform.GetChild(0).GetComponentInChildren<RectTransform>();
            pos += new Vector3(
                rectMain.GetChild(0).GetComponentInChildren<RectTransform>().rect.width/2 + slot.GetComponent<RectTransform>().rect.width * 2, // no idea why i need to multiply by 2 but it works
               -rectMain.GetChild(0).GetComponentInChildren<RectTransform>().rect.height/2- slot.GetComponent<RectTransform>().rect.height * 2,
                0);
            
            if (pos.y < rectMain.GetChild(0).GetComponentInChildren<RectTransform>().rect.height/2){
                pos.y = rectMain.GetChild(0).GetComponentInChildren<RectTransform>().rect.height/2;
            }
            if (pos.x > Screen.width - rectMain.GetChild(0).GetComponentInChildren<RectTransform>().rect.width/2){
                pos.x = Screen.width - rectMain.GetChild(0).GetComponentInChildren<RectTransform>().rect.width/2;
            }
            rectMain.position = pos;
    }
    public static void HideTooltip()
    {
        if (currentTooltip != null)
        {
            currentTooltip.SetActive(false);
        }
    }
}
