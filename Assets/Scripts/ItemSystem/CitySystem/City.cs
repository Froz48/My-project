using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class City : NetworkBehaviour, IPointerClickHandler 
{
    private Attribute[] attributes;
    [SerializeField] private List<District> districts;
    [SerializeField] private Database districtDatabase;
    [SerializeField] private List<Item> tradeGoods;
    public List<District> Districts => districts;
    public List<Item> TradeGoods => tradeGoods;

    [SerializeField] public GameObject UIObject;
    Database itemDB;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked");
        if (UIObject == null)
        {
            UIObject = GameObject.Find("TradePostGO");
        }
        UIObject.GetComponent<TradePostUI>().setCity(this);
        UIObject.GetComponent<TradePostUI>().ShowTradePanel();

    }

    private void Start()
    {
        InitializeAttributes();
        districts = new List<District>();
        itemDB = Resources.Load("ItemDatabase") as Database;
        AddRandomDistrict();
        AddRandomItem();
        AddRandomItem();
        AddRandomItem();

    }
    private void AddRandomItem()
    {
        Item newItem = itemDB.GetRandomObject() as Item;
        int tryCount = 0;
        while (tradeGoods.Contains(newItem) && tryCount < 10)
        {
            tryCount++;
            newItem = itemDB.GetRandomObject() as Item;
        }
        tradeGoods.Add(newItem);

    }
    private void InitializeAttributes()
    {
        attributes = new Attribute[3];
        attributes[0] = new Attribute(EAttributes.StorageSpace);
        attributes[1] = new Attribute(EAttributes.Infrastructure);
        attributes[2] = new Attribute(EAttributes.Complexity);
    }

    [ContextMenu("AddRandomDistrict")]
    public void AddRandomDistrict(){
        districts.Add(districtDatabase.GetRandomObject() as District);
    }
    public void CraftRecipe(Recipe recipe)
    {
        Player player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
        foreach (var i in recipe.itemsConsumed)
        {
            player.GetInventory().AddItem(i.item, -i.amount);
        }
        foreach (var i in recipe.itemsCreated)
        {
            player.GetInventory().AddItem(i.item, i.amount);
        }
    }

    public void BuyItem(Item item)
    {
        Player player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
        if (player.GetInventory().IsHasItem(itemDB.GetObjectById(0) as Item, item.price))
        {
            player.GetInventory().AddItem(itemDB.GetObjectById(0) as Item, -item.price);
            player.GetInventory().AddItem(item, 1);
        }
    }


    public bool CanCraft(Recipe recipe)
    {
        Player player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
        foreach (var i in recipe.itemsConsumed)
        {
            if (!player.GetInventory().IsHasItem(i.item, i.amount))
            {
                return false;
            }
        }
        return true;
    }
}
