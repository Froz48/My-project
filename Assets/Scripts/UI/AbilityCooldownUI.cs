using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldownUI : MonoBehaviour
{
    private Player player;
    [SerializeField] private GameObject prefabSlot;
    private List<AbilityCooldownSlot> slotsOnInterface = new List<AbilityCooldownSlot>();
    private float spacing;
    bool initialized = false;

    // private void Start()
    // {
    //     if (!IsOwner)
    //     {
    //         gameObject.SetActive(false);
    //         return;
    //     }
    // }

    public void MakeInterface(Player _player)
    {
        player = _player;
        CreateSlots();
        //CreateOrUpdateSlots();
        player.OnAnyAbilityChanged += UpdateAbilities;
        initialized = true;
        Debug.Log("AbilityCooldownUI initialized");
    }

    private void CreateSlots(){
        if (slotsOnInterface.Count >= 1){
            Debug.Log("Trying to create slots when they already exist");
            return;
        }
        for (int i = 0; i < player.abilities.Length; i++)
        {
            CreateSlot(i);
        }
    }

    private void CreateSlot(int i){
        //Ability ability = player.abilities[i];
        GameObject slotObject = Instantiate(prefabSlot, transform); // replace transform with getslotpossition
        slotObject.name = "AbilitySlot" + i;
        slotObject.transform.localPosition = GetSlotPosition(i);
        slotsOnInterface.Add(slotObject.GetComponent<AbilityCooldownSlot>());
        // slot.SetSprite(ability.sprite);
    }

    public void UpdateAbilities(object sender, EventArgs e)
    {
        //CreateOrUpdateSlots();
        for (int i = 0; i < player.abilities.Length; i++){
            slotsOnInterface[i].SetSprite(player.abilities[i].sprite);
        }
    }

    // private void CreateOrUpdateSlots()
    // {
    //     foreach (var slot in slotsOnInterface)
    //     {
    //         Destroy(slot.gameObject);
    //     }
    //     slotsOnInterface.Clear();
    //     for (int i = 0; i < player.abilities.Length; i++)
    //     {
    //         Ability ability = player.abilities[i];
    //         GameObject slotObject = Instantiate(prefabSlot, transform);
    //         slotObject.name = "AbilitySlot" + i;
    //         slotObject.transform.localPosition = GetSlotPosition(i);

    //         AbilityCooldownSlot slot = slotObject.GetComponent<AbilityCooldownSlot>();
    //         slot.SetSprite(ability.sprite);
    //         slotsOnInterface.Add(slot);
    //     }
    // }

    private void Update() // perfectly, nothing should be there
    {
        if (initialized) {
            UpdateCooldown();
            Debug.Log("Updating cooldown");
        }
        else {
            Debug.Log("Trying to update cooldown before initializing");
        }
    }

    public void UpdateCooldown()
    {
        for (int i = 0; i < player.abilities.Length; i++){
            Ability ability = player.abilities[i];
            SetCooldownFill(slotsOnInterface[i], ability);
            
        }

        // foreach (var slot in slotsOnInterface)
        // {
        //     Ability ability = player.abilities[slotsOnInterface.IndexOf(slot)];
        //     float remainingCooldown = ability.GetRemainingCooldown();
        //     SetCooldownFill(slot, ability);
        //     slot.cooldownText.text = remainingCooldown > 0 ? ability.cooldown.ToString("F0") : "";
        //     slot.SetBackToAvailability(!(remainingCooldown > 0));
        // }
    }

    public void SetCooldownFill(AbilityCooldownSlot slot, Ability ability)
    {
        float remainingCooldown = ability.GetRemainingCooldown();

        slot.cooldownFill.fillAmount = remainingCooldown / ability.cooldown;
        slot.cooldownText.text = remainingCooldown > 0 ? ability.cooldown.ToString("F0") : "";
        slot.SetBackToAvailability(!(remainingCooldown > 0));
    }

    private Vector3 GetSlotPosition(int i)
    {
        Rect rectItem = prefabSlot.GetComponent<RectTransform>().rect;
        float x = -i * (rectItem.width + spacing);
        return new Vector3(x, 0, 1);
    }
}
