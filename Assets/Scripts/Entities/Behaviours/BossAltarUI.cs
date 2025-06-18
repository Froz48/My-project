// BossAltarUI.cs
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
    
    // Открываем UI, когда игрок взаимодействует с алтарем
    public void Open(BossAltar altar, Player player)
    {
    currentAltar = altar;
    localPlayer = player;

    // ИЗМЕНЕНИЕ: Получаем информацию о предмете через метод
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
            // Кнопка активна, только если у игрока есть предмет
            startButton.interactable = localPlayer.GetInventory().IsHasItem(item);
        }
    }

    private void OnStartButtonPressed()
    {
        if (currentAltar != null && localPlayer != null)
        {
            currentAltar.TryStartRitual(localPlayer);
        }
        Close(); // Закрываем UI после нажатия
    }

    public void Close()
    {
        // Отписываемся от события, чтобы избежать утечек памяти
        if (localPlayer != null)
        {
            localPlayer.GetInventory().onItemUpdate -= OnInventoryChanged;
        }

        mainPanel.SetActive(false);
        currentAltar = null;
        localPlayer = null;
    }
}