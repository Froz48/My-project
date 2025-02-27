

using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldownSlot : MonoBehaviour{
    [SerializeField] public TextMeshProUGUI cooldownText;
    [SerializeField] public Image cooldownFill;
    [SerializeField] public Image[] abilitySprites; // set in observer
    [SerializeField] public Image abilityBackground;

    public void SetSprite(Sprite sprite){
        for (int i = 0; i < abilitySprites.Length; i++){
            abilitySprites[i].sprite = sprite;
        }
    }

    public void SetBackToAvailability(bool isAvailable){
        abilityBackground.color = isAvailable? Color.green : Color.red;
    }
}