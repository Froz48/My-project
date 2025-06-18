using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private float spawnInterval;
    [SerializeField] private int maxEnemiesPerPlayer;
    [SerializeField] private float spawnRadius = 30f;
    [SerializeField] private float minPlayerDistance = 20f;
    [Header("Dependencies")]
    [SerializeField] private BiomeGenerator biomeGenerator;

    [Header("Despawn Settings")]
    [SerializeField] private float despawnRadius = 30f;
    [SerializeField] private float cleanupInterval = 5f;

    [Header("Prefabs")]
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private Database baseSpawnPool;

    private List<GameObject> _activeNPCs = new List<GameObject>();
    private Transform _npcsContainer;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        _npcsContainer = new GameObject("NPCs_Container").transform;
        StartCoroutine(SpawnCycle());
        StartCoroutine(CleanupCycle());
    }

    private IEnumerator SpawnCycle()
    {
        var wait = new WaitForSeconds(spawnInterval);
        
        while (true)
        {
            yield return wait;
            
            if (CanSpawnMoreNPCs())
            {
                TrySpawnNPC();
            }
        }
    }
    private IEnumerator CleanupCycle()
    {
        var wait = new WaitForSeconds(cleanupInterval);
        while (true)
        {
            yield return wait;
            CheckAndDespawnDistantNPCs();
        }
    }
     private void CheckAndDespawnDistantNPCs()
    {
        if (NetworkManager.Singleton.ConnectedClients.Count == 0)
        {
            foreach (var npc in _activeNPCs)
            {
                DespawnNPC(npc);
            }
            return;
        }

        for (int i = _activeNPCs.Count - 1; i >= 0; i--)
        {
            GameObject npc = _activeNPCs[i];

            if (npc == null)
            {
                _activeNPCs.RemoveAt(i);
                continue;
            }

            bool isAnyPlayerNear = false;
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (client.PlayerObject != null && Vector2.Distance(npc.transform.position, client.PlayerObject.transform.position) < despawnRadius)
                {
                    isAnyPlayerNear = true;
                    break;
                }
            }

            if (!isAnyPlayerNear)
            {
                DespawnNPC(npc);
            }
        }
    }

    private bool CanSpawnMoreNPCs()
    {
        if (NetworkManager.Singleton.ConnectedClients.Count == 0) return false;
        int npcCount = _activeNPCs.Count;
        int playerCount = NetworkManager.Singleton.ConnectedClients.Count;
        int allowedCount = playerCount * maxEnemiesPerPlayer;
        bool ans = npcCount < allowedCount;

        return ans;
    }

    private void TrySpawnNPC()
    {
        var spawnPosition = FindSpawnPosition();
        if (spawnPosition != Vector2.zero)
        {
            var npcData = GetRandomNPCData(spawnPosition);
            if (npcData != null)
            {
                SpawnNPC(spawnPosition, npcData);
            }
        }
    }

    private Vector2 FindSpawnPosition()
    {
        var player = GetRandomPlayer();
        if (player == null) return Vector2.zero;

        for (int i = 0; i < 5; i++)
        {
            var randomDir = Random.insideUnitCircle.normalized;
            var spawnPos = (Vector2)player.transform.position + 
                          randomDir * Random.Range(minPlayerDistance, spawnRadius);
            
            if (IsValidSpawnPosition(spawnPos))
            {
                return spawnPos;
            }
        }
        
        return Vector2.zero;
    }

    private bool IsValidSpawnPosition(Vector2 position)
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (Vector2.Distance(position, client.PlayerObject.transform.position) < minPlayerDistance)
            {
                return false;
            }
        }
        return true;
    }

    private void SpawnNPC(Vector2 position, NPCData npcData)
    {
        var npc = Instantiate(npcPrefab, position, Quaternion.identity, _npcsContainer);
        var netObj = npc.GetComponent<NetworkObject>();
        var npcEntity = npc.GetComponent<NPCEntity>();
        
        netObj.Spawn();
        
        npcEntity.InitializeWithDataServerRpc(npcData.GetId());
        _activeNPCs.Add(npc);
    }

        private void DespawnNPC(GameObject npc)
    {
        if (npc == null) return;

        NetworkObject netObj = npc.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Despawn(); 
        }
        
        _activeNPCs.Remove(npc);
    }
    private NPCData GetRandomNPCData(Vector2 spawnPosition)
    {
        // 1. Создаем итоговый список, который будет содержать всех возможных для спавна NPC
        List<NPCData> finalSpawnPool = new List<NPCData>();

        // 2. Добавляем всех NPC из базового (общего) пула
        if (baseSpawnPool is Database npcDatabase)
        {
            foreach(var obj in npcDatabase.GetAllObjects())
            {
                if (obj is NPCData npcData)
                {
                    finalSpawnPool.Add(npcData);
                }
            }
        }

        // 3. Определяем биом в точке спавна
        Biome currentBiome = biomeGenerator.GetBiomeAt(spawnPosition);

        // 4. Если биом определен и у него есть свой уникальный пул, ДОБАВЛЯЕМ его содержимое в общий список
        if (currentBiome != null && currentBiome.SpawnPool != null && currentBiome.SpawnPool.Count > 0)
        {
            finalSpawnPool.AddRange(currentBiome.SpawnPool);
        }
        
        // 5. Если после всех проверок итоговый пул пуст, то спавнить некого.
        if (finalSpawnPool.Count == 0)
        {
            Debug.LogWarning($"No NPC data found for biome at {spawnPosition} and the base pool is also empty.");
            return null;
        }

        // 6. Выбираем случайного NPC из объединенного пула
        NPCData data = finalSpawnPool[Random.Range(0, finalSpawnPool.Count)];
        
        // Возвращаем копию, чтобы не изменять оригинальный ScriptableObject
        return data != null ? Instantiate(data) : null;
    }
    private Player GetRandomPlayer()
    {
        var clients = NetworkManager.Singleton.ConnectedClientsList;
        return clients.Count > 0 ? 
            clients[Random.Range(0, clients.Count)].PlayerObject.GetComponent<Player>() : 
            null;
    }

    public void OnEnemyDied(GameObject npc)
    {
        if (_activeNPCs.Contains(npc))
        {
            _activeNPCs.Remove(npc);
        }
    }
}