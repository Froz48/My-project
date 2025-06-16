using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class WorldBrowserMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform worldsContainer;
    [SerializeField] private GameObject worldEntryPrefab;

    private List<GameObject> currentWorldEntries = new List<GameObject>();
    [Header("New World UI")]
    [SerializeField] private TMP_InputField worldNameInput;
    [SerializeField] private TMP_InputField seedInput;
    [SerializeField] private Button createNewWorldButton;
    [SerializeField] private Button joinGameButton;
    [SerializeField] private RelayManager relayManager;

    private void Start()
    {
        createNewWorldButton.onClick.AddListener(CreateNewWorld);
        joinGameButton.onClick.AddListener(ConnectToGame);
        Debug.Log(createNewWorldButton.onClick);
    }
    public void CreateNewWorld()
    {
        string worldName = worldNameInput.text.Trim();
        string seed = seedInput.text.Trim();

        if (string.IsNullOrEmpty(worldName))
        {
            Debug.LogWarning("World name cannot be empty!");
            return;
        }

        string path = Path.Combine(Application.persistentDataPath, worldName + ".json");
        if (File.Exists(path))
        {
            Debug.LogWarning("World with this name already exists!");
            return;
        }
        WorldSaveData newWorldData = new WorldSaveData
        {
            worldName = worldName,
            seed = Convert.ToInt32(seedInput.text)
        };

        string json = JsonUtility.ToJson(newWorldData);
        File.WriteAllText(path, json);

        RefreshWorldList();
        worldNameInput.text = "";
        seedInput.text = "";
    }
    private void OnEnable()
    {
        RefreshWorldList();
    }

    public void ClearWorldList()
    {
        foreach (var entry in currentWorldEntries)
        {
            Destroy(entry);
        }
        currentWorldEntries.Clear();
    }
    public void RefreshWorldList()
    {

        ClearWorldList();

        var worldPaths = Directory.GetFiles(Application.persistentDataPath, "*.json")
                                .OrderByDescending(f => new FileInfo(f).LastWriteTime)
                                .ToList();

        foreach (var path in worldPaths)
        {
            string worldName = Path.GetFileNameWithoutExtension(path);
            CreateWorldEntry(worldName);
        }
    }

    private void CreateWorldEntry(string worldName)
    {
        GameObject entry = Instantiate(worldEntryPrefab, worldsContainer);
        currentWorldEntries.Add(entry);

        entry.transform.Find("World").GetComponentInChildren<TMP_Text>().text = worldName;

        Button selectButton = entry.transform.Find("SelectButton").GetComponent<Button>();
        selectButton.onClick.AddListener(() => OnWorldSelected(worldName));

        Button deleteButton = entry.transform.Find("DeleteButton").GetComponent<Button>();
        deleteButton.onClick.AddListener(() => OnWorldDeleted(worldName, entry));
    }

    private void OnWorldSelected(string worldName)
    {
        PlayerPrefs.SetString("CurrentWorld", worldName);
        string path = Path.Combine(Application.persistentDataPath, worldName + ".json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            WorldSaveData data = JsonUtility.FromJson<WorldSaveData>(json);
            PlayerPrefs.SetInt("CurrentSeed", data.seed);
        }
        SceneManager.LoadScene("Game");
        SceneManager.sceneLoaded += OnGameSceneLoadedHost;
    }

    private  void OnWorldDeleted(string worldName, GameObject entry)
    {
        string path = Path.Combine(Application.persistentDataPath, worldName + ".json");
        if (File.Exists(path))
        {
            File.Delete(path);
            currentWorldEntries.Remove(entry);
            Destroy(entry);
            Debug.Log($"Deleted world: {worldName}");
        }
    }
    public void ConnectToGame(){
        SceneManager.LoadScene("Game");
        SceneManager.sceneLoaded += OnGameSceneLoadedClient;
    }
    private void OnGameSceneLoadedClient(Scene scene, LoadSceneMode mode){
        if (scene.name == "Game")
        {
            SceneManager.sceneLoaded -= OnGameSceneLoadedClient;

            if (NetworkManager.Singleton != null)
            {
                // NetworkManager.Singleton.StartClient();
                relayManager.JoinRelay();
            }
            else
            {
                Debug.LogError("NetworkManager.Singleton is null!");
            }
        }
    }

    private void OnGameSceneLoadedHost(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            SceneManager.sceneLoaded -= OnGameSceneLoadedHost;

            if (NetworkManager.Singleton != null)
            {
                // NetworkManager.Singleton.StartHost();
                relayManager.CreateRelay();
                SaveManager.LoadWorld();
            }
            else
            {
                Debug.LogError("NetworkManager.Singleton is null!");
            }
        }
    }
}