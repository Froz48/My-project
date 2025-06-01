using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class NPCSpawner : NetworkBehaviour
{
    private const int MAX_SPAWN_TRY = 30;
    [SerializeField] private int enemyMaxCount = 30;
    [SerializeField] private Database enemyDatabase;
    //private int maxEnemyCountForPlayer = 20; //20
    /// <summary>
    /// Every x seconds, spawn a new enemy. The more the rate, the more time between spawns.
    /// </summary>
    private float spawnRate = 2f; 
    [SerializeField] private SpawnPool spawnPool;
    private float spawnMaxRadius = 30;
    GameObject parentObject;
    private float spawnMinRadius = 15;
    public int spawnedEnemyCount = 0;
    [SerializeField] public GameObject npcPrefab;


    public override void OnNetworkSpawn()
    {
        if (parentObject == null) parentObject = GameObject.Find("NPCObjects");
        if (IsServer)
            StartCoroutine(SpawnEnemiesCoroutine());
    }

    private IEnumerator SpawnEnemiesCoroutine()
    {
        while (true)
        {
            if (parentObject == null) parentObject = GameObject.Find("NPCObjects");
            SpawnRandomEnemy();
            yield return new WaitForSeconds(spawnRate);
        }
    }
    private Vector2 GetRandomSpawnPosition(){
        Vector2 spawnPosition;
        for (int i = 0; i < MAX_SPAWN_TRY; i++){
            spawnPosition = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f,1f)).normalized * Random.Range(spawnMinRadius, spawnMaxRadius) + (Vector2)this.transform.position;
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
        GameObject enemyTransform = Instantiate(npcPrefab, spawnPosition, Quaternion.identity, parentObject.transform);
        enemyTransform.transform.SetParent(parentObject.transform);
        if (nPCData.spriteLibraryAsset) enemyTransform.GetComponent<SpriteLibrary>().spriteLibraryAsset = nPCData.spriteLibraryAsset;
        enemyTransform.GetComponent<NPCEntity>().setData(nPCData);
        enemyTransform.GetComponent<NetworkObject>().Spawn();
    }

}
