using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    private const int MAX_SPAWN_TRY = 30;
    [SerializeField] private int enemyMaxCount = 30;
    [SerializeField] private MonsterDatabase monsterDatabase;
    //private int maxEnemyCountForPlayer = 20; //20
    /// <summary>
    /// Every x seconds, spawn a new enemy. The more the rate, the more time between spawns.
    /// </summary>
    private float enemySpawnRate = 5f; 
    [SerializeField] private SpawnPool spawnPool;
    private float spawnMaxRadius = 30;
    private float spawnMinRadius = 15;
    public int spawnedEntityCounter = 0;


    public override void OnNetworkSpawn()
    {
        StartCoroutine(SpawnEnemiesCoroutine());
    }

    private IEnumerator SpawnEnemiesCoroutine()
    {
        while (true)
        {
            SpawnRandomEnemy();
            yield return new WaitForSeconds(enemySpawnRate);
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
    private int GetRandomMonsterId(){
        if (spawnPool.Pool.Count == 0){
            return -1;
        }
        int NPCid = Random.Range(0, spawnPool.Pool.Count);
        return NPCid;
    }

    
    private void SpawnRandomEnemy(){
        Vector2 spawnPos = GetRandomSpawnPosition();
        int NPCId = GetRandomMonsterId();
        if ( (spawnPos == new Vector2(0,0)) || (NPCId == -1) ){
            return;
        }
        SpawnEnemyServerRpc(spawnPos, NPCId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnEnemyServerRpc(Vector2 spawnPosition, int NPCId){
        if (spawnedEntityCounter >= enemyMaxCount){
            return;
        }
        spawnedEntityCounter++;
        
        NPCData nPCData = monsterDatabase.GetObjectById(NPCId) as NPCData;
        
        if (nPCData == null || nPCData.Prefab == null){
            Debug.Log($"ERROR! tried to spawn monster with id {NPCId}");
            return;
        }

        Transform enemyTransform = Instantiate(nPCData.Prefab, spawnPosition, Quaternion.identity, transform);
        enemyTransform.GetComponent<NetworkObject>().Spawn();
    }

}
