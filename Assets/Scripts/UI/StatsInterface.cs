using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;


public class StatsInterface : NetworkBehaviour
{
    Attribute[] attributes;
    [SerializeField] private TextMeshProUGUI text;
    public void makeUI(Attribute[] _attributes)
    {   
        attributes = _attributes;
        for (int i = 0; i < attributes.Length; i++){
            attributes[i].onValueModified += UpdateUI;
        }
        UpdateUI(this, EventArgs.Empty);
    }
    public void Start()
    {   
        if (!IsOwner) {gameObject.SetActive(false); return;}
    }
    public void UpdateUI(object sender, System.EventArgs e){
        text.text = "";
        for (int i = 0; i < System.Enum.GetValues(typeof(EAttributes)).Length; i++){
            text.text += Attribute.GetStatNameById(i) + ": " + attributes[i].GetValue() + "\n";
        }
    }


}
