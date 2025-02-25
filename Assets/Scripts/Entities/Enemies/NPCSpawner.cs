using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NPCSpawner : NetworkBehaviour
{
    private const int MAX_SPAWN_TRY = 30;
    [SerializeField] private int enemyMaxCount = 30;
    [SerializeField] private EnemyDatabase enemyDatabase;
    //private int maxEnemyCountForPlayer = 20; //20
    /// <summary>
    /// Every x seconds, spawn a new enemy. The more the rate, the more time between spawns.
    /// </summary>
    private float spawnRate = 5f; 
    [SerializeField] private SpawnPool spawnPool;
    private float spawnMaxRadius = 30;
    private float spawnMinRadius = 15;
    public int spawnedEnemyCount = 0;


    public override void OnNetworkSpawn()
    {
        StartCoroutine(SpawnEnemiesCoroutine());
    }

    private IEnumerator SpawnEnemiesCoroutine()
    {
        while (true)
        {
            SpawnRandomEnemy();
            yield return new WaitForSeconds(spawnRate);
        }
    }
    private Vector2 GetRandomSpawnPosition(){
        Vector2 spawnPosition;
        for (int i = 0; i < MAX_SPAWN_TRY; i++){
            spawnPosition = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f,1f)).normalized * Random.Range(spawnMinRadius, spawnMaxRadius);
            if (CheckIsValidSpawnPosition(spawnPosition)){
                return spawnPosition;
            }
        }
        return new Vector2(0,0);
    }
    private bool CheckIsValidSpawnPosition(Vector2 position){
        foreach (var P in NetworkManager.Singleton.ConnectedClientsList){
            if (Vector2.Distance(position, P.PlayerObject.transform.position) < spawnMinRadius){
                return false;
            }
        }
        return true;
    }

    private void SpawnRandomEnemy(){
        Vector2 spawnPos = GetRandomSpawnPosition();
        int NPCId = spawnPool.GetRandomMonsterIndex();
        if ( (spawnPos == new Vector2(0,0)) || (NPCId == -1) ){
            return;
        }
        SpawnEnemyServerRpc(spawnPos, NPCId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnEnemyServerRpc(Vector2 spawnPosition, int NPCId){
        if (spawnedEnemyCount >= enemyMaxCount){
            return;
        }
        spawnedEnemyCount++;
        
        NPCData nPCData = enemyDatabase.GetObjectById(NPCId) as NPCData;
        
        if (nPCData == null || nPCData.Prefab == null){
            Debug.Log($"ERROR! tried to spawn monster with id {NPCId}");
            return;
        }

        Transform enemyTransform = Instantiate(nPCData.Prefab, spawnPosition, Quaternion.identity, transform);
        enemyTransform.GetComponent<NetworkObject>().Spawn();
    }

}
