using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class City : MonoBehaviour
{
    private Attribute[] attributes;
    [SerializeField] private List<District> districts;
    [SerializeField] private Database districtDatabase;
    [SerializeField] private List<TradeGoods> tradeGoods;
    [SerializeField] private List<ItemAmountLine> items;
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

    private void CalculateSupplyDemand(){
        
    }

    public void ProduceList(){
        Debug.Log("Producing...");
        foreach (District district in districts){
            if (IsEnoughResources(district)){
                Produce(district);
            }
        }
    }

    private int GetItemAmount(ItemBase itemBase){
        foreach (ItemAmountLine i in items){
            if (i.item == itemBase){
                return i.amount;
            }
        }
        return 0;
    }

    private void addItem(ItemBase itemBase, int amount){
        Debug.Log("Adding " + amount + " " + itemBase.name);
        foreach (ItemAmountLine i in items){
            if (i.item == itemBase){
                i.amount += amount;
                return;
            }
        }
        items.Add(new ItemAmountLine{item = itemBase, amount = amount});

    }

    public bool IsEnoughResources(District district){
        foreach (ItemAmountLine i in district.itemsConsumed){
            if (GetItemAmount(i.item) < i.amount){
                return false;
            }
        }
        return true;
    }

    public void Produce(District district){
        foreach (ItemAmountLine i in district.itemsConsumed){
            addItem(i.item, -i.amount);
        }
        foreach (ItemAmountLine i in district.itemsCreated){
            addItem(i.item, i.amount);
        }
    }

    [ContextMenu("AddRandomDistrict")]
    public void AddRandomDistrict(){
        districts.Add(districtDatabase.GetRandomObject() as District);
    }



}
