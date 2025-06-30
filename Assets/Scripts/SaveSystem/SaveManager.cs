using System.IO;
using Unity.Netcode;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class SaveManager : NetworkBehaviour
{
    public static SaveManager Instance { get; private set; }
    private WorldSaveData _worldData;
    public static string CurrentWorldName => PlayerPrefs.GetString("CurrentWorld", "default_world");
    public static int CurrentSeed => PlayerPrefs.GetInt("CurrentSeed", 0);
    private static Database _itemDb;
    [SerializeField] private CharacterSelectionUI characterSelectionUI;
    public static Database itemDb
    {
        get
        {
            if (_itemDb == null)
            {
                _itemDb = Resources.Load<Database>("ItemDatabase");
                if (_itemDb == null)
                {
                    Debug.LogError("Failed to load ItemDatabase from Resources!");
                }
            }
            return _itemDb;
        }
    }
    public override void OnNetworkSpawn()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (IsServer)
        {
            LoadWorldDataFromFile();

            // NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoadedForAll;
        }
    }
    
    public override void OnNetworkDespawn()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnSceneLoadedForAll;
        }
        base.OnNetworkDespawn();
    }
    // private void OnClientConnected(ulong clientId)
    // {
    //     RequestCharacterChoiceClientRpc(JsonUtility.ToJson(_worldData), new ClientRpcParams
    //     {
    //         Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } }
    //     });
    // }
    private void OnSceneLoadedForAll(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        Debug.Log($"[SaveManager] OnSceneLoadedForAll triggered. Scene: {sceneName}, Clients completed: {clientsCompleted.Count}");
        if (sceneName != "Game")
        {
            Debug.Log($"[SaveManager] Scene is not 'Game', ignoring.");
            return;
        }
        if (characterSelectionUI == null)
            {

                characterSelectionUI = CharacterSelectionUI.Instance;
            }
        foreach (var clientId in clientsCompleted)
        {
            Debug.Log($"[SaveManager] Processing client ID: {clientId}. Sending RPC...");
            RequestCharacterChoiceClientRpc(JsonUtility.ToJson(_worldData), new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } }
            });
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void RefreshCharacterListServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        Debug.Log($"[SaveManager] Received character list request from client {clientId}.");

        RequestCharacterChoiceClientRpc(JsonUtility.ToJson(_worldData), new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } }
        });
    }

    [ClientRpc]
    public void RequestCharacterChoiceClientRpc(string worldDataJson, ClientRpcParams rpcParams = default)
    {
        Debug.Log($"[SaveManager] Client {NetworkManager.Singleton.LocalClientId} received RequestCharacterChoiceClientRpc.");
        
        WorldSaveData worldData = JsonUtility.FromJson<WorldSaveData>(worldDataJson);
        int characterCount = worldData?.players?.Length ?? 0;

        if (CharacterSelectionUI.Instance == null)
        {
            Debug.LogError("[SaveManager] CharacterSelectionUI.Instance is NULL. Cannot show UI.");
            return;
        }
        
        CharacterSelectionUI.Instance.Show(worldData.players);
    }

        [ServerRpc(RequireOwnership = false)]
    public void SelectCharacterServerRpc(string characterGuid, ServerRpcParams rpcParams = default)
    {
        Debug.Log("63");
        ulong clientId = rpcParams.Receive.SenderClientId;
        PlayerSaveData selectedData = _worldData.players?.FirstOrDefault(p => p.characterGuid == characterGuid);
        Debug.Log("66");
        Player playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<Player>();
        if (playerObject != null && selectedData != null)
        {
            Debug.Log("69");
             string playerDataJson = JsonUtility.ToJson(selectedData);
             LoadPlayerClientRpc(playerDataJson, new ClientRpcParams
             {
                 Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } }
             });
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CreateNewCharacterServerRpc(string characterName, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        PlayerSaveData newPlayerData = new PlayerSaveData();
        newPlayerData.ownerClientId = clientId;
        newPlayerData.characterName = characterName;

        var playerList = _worldData.players?.ToList() ?? new System.Collections.Generic.List<PlayerSaveData>();
        playerList.Add(newPlayerData);
        _worldData.players = playerList.ToArray();

        string playerDataJson = JsonUtility.ToJson(newPlayerData);

        LoadPlayerClientRpc(playerDataJson, new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } }
        });
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestSaveWorldServerRpc(bool andExit, ServerRpcParams rpcParams = default)
    {
        StartCoroutine(SaveRoutine(andExit));
    }

    private IEnumerator SaveRoutine(bool andExit)
    {
        _pendingClientSaves = NetworkManager.Singleton.ConnectedClientsIds.ToList();

        if (IsHost) _pendingClientSaves.Remove(NetworkManager.Singleton.LocalClientId);

        RequestPlayerDataClientRpc();

        if (IsHost)
        {
            Player hostPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
            UpdatePlayerData(JsonUtility.ToJson(new PlayerSaveData(hostPlayer)));
        }

        float timeout = 5f;
        while (_pendingClientSaves.Count > 0 && timeout > 0)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        if (timeout <= 0)
        {
            Debug.LogWarning("[SaveManager] Save timed out. Some clients did not respond.");
        }

        SaveWorld();

        if (andExit)
        {
            ShutdownClientRpc();
            yield return new WaitForSeconds(0.5f);
            NetworkManager.Singleton.Shutdown();
        }
    }
    [ClientRpc]
    private void ShutdownClientRpc()
    {
        if(!IsHost) NetworkManager.Singleton.Shutdown();
    }
    private List<ulong> _pendingClientSaves = new List<ulong>();
    [ServerRpc(RequireOwnership = false)]
    private void SubmitPlayerDataServerRpc(string playerDataJson, ServerRpcParams rpcParams = default)
    {
        UpdatePlayerData(playerDataJson);
        _pendingClientSaves.Remove(rpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    private void RequestPlayerDataClientRpc()
    {
        if (IsServer) return; 

        Player localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
        if (localPlayer != null && !string.IsNullOrEmpty(localPlayer.GetCharacterGuid()))
        {
            PlayerSaveData saveData = new PlayerSaveData(localPlayer);
            SubmitPlayerDataServerRpc(JsonUtility.ToJson(saveData));
        }
    }
    private void UpdatePlayerData(string playerDataJson)
    {
        PlayerSaveData receivedData = JsonUtility.FromJson<PlayerSaveData>(playerDataJson);
        if (receivedData == null || string.IsNullOrEmpty(receivedData.characterGuid)) return;

        PlayerSaveData saveDataToUpdate = _worldData.players.FirstOrDefault(p => p.characterGuid == receivedData.characterGuid);
        if (saveDataToUpdate != null)
        {
            saveDataToUpdate.position = receivedData.position;
            saveDataToUpdate.health = receivedData.health;
            saveDataToUpdate.ownerClientId = receivedData.ownerClientId;
            saveDataToUpdate.inventory = receivedData.inventory;
            saveDataToUpdate.equipment = receivedData.equipment;
            
            Debug.Log($"[SaveManager] Updated data for character {saveDataToUpdate.characterName} (Client: {saveDataToUpdate.ownerClientId})");
        }
    }
    private void SaveWorld()
    {
        if (!IsServer) return;
        
        string json = JsonUtility.ToJson(_worldData);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, CurrentWorldName + ".json"), json);
        Debug.Log($"[SaveManager] World '{CurrentWorldName}' saved to file.");
    }

    //     private void SaveWorld()
    // {
    //     if (!IsServer) return;

    //     foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
    //     {
    //         Player player = client.PlayerObject.GetComponent<Player>();
    //         if (player == null || string.IsNullOrEmpty(player.GetCharacterGuid())) continue;

    //         PlayerSaveData saveData = _worldData.players.FirstOrDefault(p => p.characterGuid == player.GetCharacterGuid());
    //         if (saveData != null)
    //         {
    //             saveData.position = player.transform.position;
    //             saveData.health = player.getCurrentHealth();
    //             saveData.ownerClientId = player.OwnerClientId;

    //             for (int i = 0; i < saveData.inventory.Length; i++)
    //             {
    //                 saveData.inventory[i] = new InventorySlotSaveData(player.GetInventory().Slots[i]);
    //             }
    //             for (int i = 0; i < saveData.equipment.Length; i++)
    //             {
    //                 saveData.equipment[i] = new InventorySlotSaveData(player.GetEquipment().Slots[i]);
    //             }
    //         }
    //     }

    //     string json = JsonUtility.ToJson(_worldData);
    //     File.WriteAllText(Path.Combine(Application.persistentDataPath, CurrentWorldName + ".json"), json);
    //     Debug.Log($"[SaveManager] World '{CurrentWorldName}' saved.");
    // }
    private void LoadWorldDataFromFile()
    {
        string path = Path.Combine(Application.persistentDataPath, CurrentWorldName + ".json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            _worldData = JsonUtility.FromJson<WorldSaveData>(json);
            Debug.Log($"[SaveManager] World data for '{CurrentWorldName}' loaded.");
        }
        else
        {
            _worldData = new WorldSaveData();
        }
    }
    private void LoadPlayerForClient(ulong clientId)
    {
        if (!IsServer || _worldData == null) return;

        PlayerSaveData playerData = _worldData.players?.FirstOrDefault(p => p.ownerClientId == clientId);

        if (playerData != null)
        {
            string playerDataJson = JsonUtility.ToJson(playerData);
            
            LoadPlayerClientRpc(playerDataJson, new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } }
            });
            Debug.Log($"[SaveManager] Found and sent save data to client {clientId}");
        }
        else
        {
             Debug.Log($"[SaveManager] No save data found for client {clientId}. They will start fresh.");
        }
    }
    [ClientRpc]
    private void LoadPlayerClientRpc(string playerDataJson, ClientRpcParams clientRpcParams = default)
    {
        PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(playerDataJson);
        Player localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
        
        if (localPlayer != null)
        {
            localPlayer.LoadData(data);
            Debug.Log($"[SaveManager] Client {OwnerClientId} loaded data for character '{data.characterName}'.");

            CharacterSelectionUI.Instance.Hide();
        }
    }
    public static void LoadToPlayer(Player player, PlayerSaveData data)
    {
        player.transform.position = data.position;
        player.SetCurrentHealth(data.health);


        for (int i = 0; i < data.inventory.Length; i++)
        {
            if (data.inventory[i].amount > 0)
            {
                Debug.Log(itemDb);
                Item item = itemDb.GetObjectById(data.inventory[i].id) as Item;
                player.GetInventory().Slots[i].UpdateSlot(item, data.inventory[i].amount);
            }
        }
        for (int i = 0; i < data.equipment.Length; i++)
        {
            if (data.equipment[i].amount > 0)
            {
                Item item = itemDb.GetObjectById(data.equipment[i].id) as Item;
                player.GetEquipment().Slots[i].UpdateSlot(item, data.equipment[i].amount);
            }
        }
    }

} 

