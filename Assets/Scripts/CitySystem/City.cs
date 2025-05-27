using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{
    private Attribute[] attributes;
    [SerializeField] private List<District> districts;
    [SerializeField] private Database districtDatabase;
    [SerializeField] private List<Item> tradeGoods;
    private void Start() {
        InitializeAttributes();
        districts = new List<District>();
        AddRandomDistrict();
        AddRandomDistrict();
    }
    private void InitializeAttributes(){
        attributes = new Attribute[3];
        attributes[0] = new Attribute(EAttributes.StorageSpace);
        attributes[1] = new Attribute(EAttributes.Infrastructure);
        attributes[2] = new Attribute(EAttributes.Complexity);
    }

    [ContextMenu("AddRandomDistrict")]
    public void AddRandomDistrict(){
        districts.Add(districtDatabase.GetRandomObject() as District);
    }

}
