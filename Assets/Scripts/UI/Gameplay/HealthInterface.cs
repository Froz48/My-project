using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthInterface : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image healthBar;
    Player player;
    
    public void MakeHealthUI(Player _player){
        player = _player;
        player.attributes[(int)EAttributes.MaxHealth].onValueModified += UpdateUi;
        player.OnHealthChanged += UpdateUi;
        UpdateUi(this, EventArgs.Empty);
    }

    public void Start()
    {   
        if (!IsOwner) {gameObject.SetActive(false); return;}
    }
    public void UpdateUi(object sender, System.EventArgs e){
        float currentHealth = Mathf.Floor(player.getCurrentHealth());
        float maxHealth = player.GetMaxHealth();
        // f0 = no decimal
        text.text = currentHealth.ToString("F0") + " / " +  maxHealth.ToString();
        healthBar.fillAmount = currentHealth / maxHealth;
    }   


}
