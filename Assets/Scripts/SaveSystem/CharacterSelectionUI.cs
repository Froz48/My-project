// Файл: CharacterSelectionUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectionUI : MonoBehaviour
{
     public static CharacterSelectionUI Instance { get; private set; }

    [SerializeField] private GameObject mainPanel;
    [SerializeField] private Transform characterListContainer;
    [SerializeField] private GameObject characterEntryPrefab;
    [SerializeField] private Button createNewButton;
    [SerializeField] private TMP_InputField newCharacterNameInput;
    [SerializeField] private Button refreshButton;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        // Hide();
        createNewButton.onClick.AddListener(OnCreateNewCharacter);
        if (refreshButton != null)
        {
            refreshButton.onClick.AddListener(OnRefreshClicked);
        }
    }
    public void UpdateCharacterList(PlayerSaveData[] characters)
    {
        Debug.Log("[CharacterSelectionUI] Updating character list...");
        
        foreach (Transform child in characterListContainer)
        {
            Destroy(child.gameObject);
        }

        if (characters != null && characters.Length > 0)
        {
            foreach (var characterData in characters)
            {
                GameObject entry = Instantiate(characterEntryPrefab, characterListContainer);
                entry.GetComponentInChildren<TMP_Text>().text = characterData.characterName;
                entry.GetComponent<Button>().onClick.AddListener(() => OnCharacterSelected(characterData.characterGuid));
            }
        }
        else
        {
            Debug.Log("[CharacterSelectionUI] No characters to display.");
        }
    }
    private void OnRefreshClicked()
    {
        Debug.Log("[CharacterSelectionUI] Refresh button clicked. Requesting character list from server...");
        SaveManager.Instance.RefreshCharacterListServerRpc();
    }

    public void Show(PlayerSaveData[] characters)
    {
        Debug.Log("[CharacterSelectionUI] Show() called.");
        if (mainPanel == null)
        {
            Debug.LogError("[CharacterSelectionUI] MainPanel is not assigned in the inspector!");
            return;
        }
        mainPanel.SetActive(true);
        foreach (Transform child in characterListContainer)
        {
            Destroy(child.gameObject);
        }

        if (characters != null)
        {
            foreach (var characterData in characters)
            {
                GameObject entry = Instantiate(characterEntryPrefab, characterListContainer);
                entry.GetComponentInChildren<TMP_Text>().text = characterData.characterName;
                entry.GetComponentInChildren<Button>().onClick.AddListener(() => OnCharacterSelected(characterData.characterGuid));
            }
        }


    }

    private void OnCharacterSelected(string characterGuid)
    {
        Debug.Log("Clicked on select");
        SaveManager.Instance.SelectCharacterServerRpc(characterGuid);
    }

    private void OnCreateNewCharacter()
    {
        Debug.Log("OnCreateNewCharacter");
        string name = newCharacterNameInput.text;
        if (string.IsNullOrWhiteSpace(name)) return;
        SaveManager.Instance.CreateNewCharacterServerRpc(name);
    }
    
    public void Hide()
    {
        Debug.Log("hide");
        mainPanel.SetActive(false);
    }
}