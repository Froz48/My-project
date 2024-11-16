using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldownUI : NetworkBehaviour
{
    private Player player;
    [SerializeField] private GameObject prefabSlot;
    private List<AbilityCooldownSlot> slotsOnInterface = new List<AbilityCooldownSlot>();
    private float spacing;

    private void Start()
    {
        if (!IsOwner)
        {
            gameObject.SetActive(false);
            return;
        }
    }

    public void MakeInterface(Player _player)
    {
        player = _player;
        CreateOrUpdateSlots();
        player.OnAbilityChanged += UpdateAbilities;
    }

    public void UpdateAbilities(object sender, EventArgs e)
    {
        CreateOrUpdateSlots();
    }

    private void CreateOrUpdateSlots()
    {
        foreach (var slot in slotsOnInterface)
        {
            Destroy(slot.gameObject);
        }
        slotsOnInterface.Clear();

        for (int i = 0; i < player.abilities.Length; i++)
        {
            Ability ability = player.abilities[i];
            GameObject slotObject = Instantiate(prefabSlot, transform);
            slotObject.name = "AbilitySlot" + i;
            slotObject.transform.localPosition = GetSlotPosition(i);

            AbilityCooldownSlot slot = slotObject.GetComponent<AbilityCooldownSlot>();
            slot.SetSprite(ability.sprite);
            slotsOnInterface.Add(slot);
        }
    }

    private void Update()
    {
        UpdateCooldown();
    }

    public void UpdateCooldown()
    {
        foreach (var slot in slotsOnInterface)
        {
            Ability ability = player.abilities[slotsOnInterface.IndexOf(slot)];
            float remainingCooldown = ability.GetRemainingCooldown();
            SetCooldownFill(slot, ability);
            slot.cooldownText.text = remainingCooldown > 0 ? ability.cooldown.ToString("F0") : "";
            slot.SetBackToAvailability(!(remainingCooldown > 0));
        }
    }

    public void SetCooldownFill(AbilityCooldownSlot slot, Ability ability)
    {
        slot.cooldownFill.fillAmount = ability.GetRemainingCooldown() / ability.cooldown;
    }

    private Vector3 GetSlotPosition(int i)
    {
        Rect rectItem = prefabSlot.GetComponent<RectTransform>().rect;
        float x = -i * (rectItem.width + spacing);
        return new Vector3(x, 0, 1);
    }
}
