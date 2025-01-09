using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{
    private Attribute[] attributes;
    private List<District> districts;
    private Dictionary<ItemObject, int> items;

    private void Start() {
        InitializeAttributes();
        districts = new List<District>();
        
    }
    private void InitializeAttributes(){
        attributes = new Attribute[13];
        attributes[0] = new Attribute(EAttributes.Population);
        attributes[1] = new Attribute(EAttributes.Housing);
        attributes[2] = new Attribute(EAttributes.Gold);
        attributes[3] = new Attribute(EAttributes.Defenses);
        attributes[4] = new Attribute(EAttributes.StorageSpace);
        attributes[5] = new Attribute(EAttributes.Fame);
        attributes[6] = new Attribute(EAttributes.Infrastructure);
        attributes[7] = new Attribute(EAttributes.Complexity);
    }
}
