using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossAltarUI : MonoBehaviour
{
    public static BossAltarUI Instance { get; private set; }
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private Image requiredItemIcon;
    [SerializeField] private TextMeshProUGUI requiredItemName;
    [SerializeField] private Button startButton;
    [SerializeField] private Button closeButton;

    private BossAltar currentAltar;
    private Player localPlayer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        startButton.onClick.AddListener(OnStartButtonPressed);
        closeButton.onClick.AddListener(Close);
        mainPanel.SetActive(false);
    }
    
    public void Open(BossAltar altar, Player player)
    {
    currentAltar = altar;
    localPlayer = player;

    Item item = currentAltar.GetRequiredItem();
    if (item == null)
    {
        Debug.LogError("У алтаря не назначен предмет для призыва!");
        Close();
        return;
    }
    
    requiredItemIcon.sprite = item.sprite;
    requiredItemName.text = item.name;

    UpdateStartButtonState();
    
    mainPanel.SetActive(true);
    
    localPlayer.GetInventory().onItemUpdate += OnInventoryChanged;
}

    private void OnInventoryChanged(object sender, System.EventArgs e)
    {
        UpdateStartButtonState();
    }
    
    private void UpdateStartButtonState()
    {
        if (localPlayer != null && currentAltar != null)
        {
            var item = currentAltar.GetComponent<BossAltar>().requiredItem;
            startButton.interactable = localPlayer.GetInventory().IsHasItem(item);
        }
    }

    private void OnStartButtonPressed()
    {
        if (currentAltar != null && localPlayer != null)
        {
            currentAltar.TryStartRitual(localPlayer);
        }
        Close(); 
    }

    public void Close()
    {
        if (localPlayer != null)
        {
            localPlayer.GetInventory().onItemUpdate -= OnInventoryChanged;
        }

        mainPanel.SetActive(false);
        currentAltar = null;
        localPlayer = null;
    }
}