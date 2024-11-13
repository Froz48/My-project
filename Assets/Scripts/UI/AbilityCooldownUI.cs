using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldownUI : NetworkBehaviour
{
    private Player player;
    [SerializeField] private GameObject prefabSlot;
    public Dictionary<GameObject, Ability> slotsOnInterface = new Dictionary<GameObject, Ability>();
    private float spacing;

    private void Start(){
        if (!IsOwner) {
            gameObject.SetActive(false);
            return;
        }
    }
    public void MakeInterface(Player _player){
        player = _player;
        for (int i = 0; i < player.abilities.Length; i++){
            Ability ability = player.abilities[i];
            GameObject slot = makeSlot(i);
            slot.GetComponent<AbilityCooldownSlot>().SetSprite(ability.sprite);
            slotsOnInterface.Add(slot, ability);
        }   
    }

    public void UpdateAbilities(){
        for (int i = 0; i < player.abilities.Length; i++){
            Ability ability = player.abilities[i];
            GameObject slot = makeSlot(i);
            slot.GetComponent<AbilityCooldownSlot>().SetSprite(ability.sprite);
            slotsOnInterface.Add(slot, ability);
        }   
    }

    private void Update()
    {
        UpdateCooldown();
    }
    public void UpdateCooldown()
    {
        foreach (var slot in slotsOnInterface){
            float remainingCooldown = slot.Value.GetRemainingCooldown();
            SetCooldownFill(slot);
            slot.Key.GetComponent<AbilityCooldownSlot>().cooldownText.text = remainingCooldown > 0 ? slot.Value.cooldown.ToString("F0") : "";
            slot.Key.GetComponent<AbilityCooldownSlot>().SetBackToAvailability(!(remainingCooldown > 0)); 
        }
        
    }

    public void SetCooldownFill(KeyValuePair<GameObject, Ability> slot){
        slot.Key.GetComponent<AbilityCooldownSlot>().cooldownFill.fillAmount = slot.Value.GetRemainingCooldown() / slot.Value.cooldown;
    }

    private GameObject makeSlot(int i, string _name = "AbilitySlot"){
        GameObject slot = Instantiate(prefabSlot, transform);
        slot.name = _name + i;
        slot.transform.SetParent(transform);
        slot.transform.localPosition = GetSlotPosition(i);
        return slot;
    }

    private Vector3 GetSlotPosition(int i){
        Rect rectItem = prefabSlot.GetComponent<RectTransform>().rect;
        float x = -i * (rectItem.width + spacing);
        return new Vector3(x, 0, 1);
    }
}
