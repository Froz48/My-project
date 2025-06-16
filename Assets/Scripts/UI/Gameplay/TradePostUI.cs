using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.Netcode;
using System.Collections;
using UnityEngine.EventSystems;

public class TradePostUI : MonoBehaviour
{
    [Header("Tabs")]
    [SerializeField] private Button craftTabButton;
    [SerializeField] private Button tradeTabButton;
    
    [Header("Panels")]
    [SerializeField] private GameObject craftPanel;
    [SerializeField] private GameObject tradePanel;
    [SerializeField] private GameObject mainCanvas;

    [Header("Craft References")]
    [SerializeField] private Transform districtsContainer;
    [SerializeField] private Transform recipesContainer;
    [SerializeField] private GameObject districtPrefab;
    [SerializeField] private GameObject consumedItemPrefab;
    [SerializeField] private GameObject recipePrefab;
    
    [Header("Trade References")] 
    [SerializeField] private Transform tradeItemsContainer;
    [SerializeField] private GameObject tradeItemPrefab;

    
    private City city;

    public void setCity(City newCity)
    {
        city = newCity;
    }

    private void Start()
    {
        // city = FindObjectOfType<City>();

        craftTabButton.onClick.AddListener(() => ShowCraftPanel());
        tradeTabButton.onClick.AddListener(() => ShowTradePanel());

        // ShowPanel(craftPanel);
        // RefreshCraftPanel();
        // RefreshTradePanel();
    }

    public void ShowPanel(GameObject panel)
    {
        mainCanvas.SetActive(true);
        craftPanel.SetActive(panel == craftPanel);
        tradePanel.SetActive(panel == tradePanel);
    }

    public void ShowTradePanel()
    {
        RefreshTradePanel();
        ShowPanel(tradePanel);
    }
    public void ShowCraftPanel()
    {
        CreateCraftPanel();
        ShowPanel(craftPanel);
    }
    public bool TryHide()
    {
        if (!mainCanvas.activeInHierarchy) return false;

        mainCanvas.SetActive(false);
        return true;
    }

    #region Craft Panel
    private void CreateCraftPanel()
    {
        ClearContainer(districtsContainer);
        ClearContainer(recipesContainer);
        foreach (District i in city.Districts)
        {
            Debug.Log(i);
            GameObject districtGO = Instantiate(districtPrefab, districtsContainer);
            districtGO.transform.Find("IconImage").GetComponent<Image>().sprite = i.sprite;
            
            Button buyButton = districtGO.GetComponent<Button>();
            buyButton.onClick.AddListener(() => ShowRecipesOfDistrict(i));
        }
    }
    private void ShowRecipesOfDistrict(District district) {
        ClearContainer(recipesContainer);
        foreach (Recipe i in district.recipes)
        {
            GameObject recipeGO = Instantiate(recipePrefab, recipesContainer);
            SetupRecipeUI(recipeGO, i);
        }
    }

    private void SetupRecipeUI(GameObject recipeGO, Recipe recipe)
    {
        recipeGO.GetComponentInChildren<Button>().onClick.AddListener(() => CraftRecipe(recipe));

        if (recipe.itemsCreated.Length > 0 && recipe.itemsCreated[0].item != null)
        {
            Transform craftedItemPanel = recipeGO.transform.Find("CraftedItemPanel");
            ItemAmountLine resultItem = recipe.itemsCreated[0];

            craftedItemPanel.Find("ItemName").GetComponent<TMP_Text>().text = resultItem.item.name;
            craftedItemPanel.Find("AmountText").GetComponent<TMP_Text>().text = $"x{resultItem.amount}";
            craftedItemPanel.Find("IconImage").GetComponent<Image>().sprite = resultItem.item.sprite;
        }

        Transform consumedItemsContainer = recipeGO.transform.Find("ConsumedItemsContainer");
        ClearContainer(consumedItemsContainer);

        var layoutGroup = consumedItemsContainer.GetComponent<HorizontalLayoutGroup>();

        foreach (ItemAmountLine consumedItem in recipe.itemsConsumed)
        {
            if (consumedItem.item == null) continue;

            GameObject itemDisplay = Instantiate(consumedItemPrefab, consumedItemsContainer);
            itemDisplay.transform.Find("IconImage").GetComponent<Image>().sprite = consumedItem.item.sprite;
            itemDisplay.transform.Find("AmountText").GetComponent<TMP_Text>().text = $"x{consumedItem.amount}";
        }
    }

    private void CraftRecipe(Recipe recipe)
    {
        if (city.CanCraft(recipe))
        {
            city.CraftRecipe(recipe);
        }
    }
    #endregion

    #region Trade Panel
    private void RefreshTradePanel()
    {
        ClearContainer(tradeItemsContainer);
        
        foreach (Item item in city.TradeGoods)
        {
            GameObject itemGO = Instantiate(tradeItemPrefab, tradeItemsContainer);
            itemGO.GetComponentInChildren<TMP_Text>().text = $"{item.name} - {item.price} g";
            itemGO.transform.Find("IconImage").GetComponent<Image>().sprite = item.sprite;
            
            Button buyButton = itemGO.GetComponentInChildren<Button>();
            buyButton.onClick.AddListener(() => BuyItem(item));
        }
    }

    private void BuyItem(Item item)
    {
        city.BuyItem(item);
        
    }
    #endregion

    private void ClearContainer(Transform container)
    {
        if (container == null)
        {
            Debug.Log("clearNull");
            return;
        }
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }
}