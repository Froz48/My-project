using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    private int enemyCountPerPlayer = 20; //20
    private float spawnCooldown = 5f; // 5
    [SerializeField] private Transform enemyPrefab;
    [SerializeField] private MonsterData[] monsterData;
    private float spawnMaxRadius = 30;
    private float spawnMinRadius = 15;
    public int spawnedEntityCounter = 0;
    public override void OnNetworkSpawn()
    {
        if (!IsServer){
            // Debug.Log("EnemySpawner is not the server");
            gameObject.SetActive(false);
            return;
        }
        StartCoroutine(SpawnEnemies());
        // Debug.Log("EnemySpawner started coroutine");
    }

    private void EnemySpawnTry(){
        int playerCount = NetworkManager.Singleton.ConnectedClients.Count;
        if (playerCount == 0){
            // Debug.Log("EnemySpawnTry havent found any players");
            return;
        }
        // Debug.Log("enemyCountPerPlayer = " + enemyCountPerPlayer);
        // Debug.Log("playerCount: " + playerCount);
        // Debug.Log("spawnCount <= playerCount*enemyCountPerPlayer : " + spawnedEntityCounter + " <= " + playerCount*enemyCountPerPlayer);
        if (spawnedEntityCounter <= playerCount*enemyCountPerPlayer){
            Transform player = NetworkManager.Singleton.ConnectedClientsList[Random.Range(0,playerCount)].PlayerObject.gameObject.transform;
            Vector2 spawnPosition = player.position;
            spawnPosition += GetRandomSpawnPosition();
            if (CheckIsValidSpawnPosition(spawnPosition)){
                SpawnRandomEnemy(spawnPosition);
            }
        }
    }

    

    private bool CheckIsValidSpawnPosition(Vector2 position){
        foreach (var collider in Physics2D.OverlapCircleAll(position, spawnMinRadius)){
            if (collider.GetComponent<Player>() != null){
                return false;
            }
        }
        return true;
    }

    private Vector2 GetRandomSpawnPosition(){
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f,1f)).normalized * Random.Range(spawnMinRadius, spawnMaxRadius);
    }

    private MonsterData GetRandomMonster(){
        if (monsterData.Length == 0){
            // Debug.Log("No monster data");
            return null;
        }
        int i = Random.Range(0, monsterData.Length);
        return monsterData[i];
    }



    private void SpawnRandomEnemy(Vector2 spawnPos){
            // Debug.Log("Spawning enemy");
            Transform enemyTransform = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, transform);
            enemyTransform.GetComponent<MonsterWorld>().SetMonsterData(GetRandomMonster());
            enemyTransform.SetParent(gameObject.transform);
            spawnedEntityCounter++;
            enemyTransform.GetComponent<NetworkObject>().Spawn();
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            EnemySpawnTry();
            // Vector2 spawnPos = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f,1f)).normalized;
            // spawnPos *= Random.Range(spawnMinRadius, spawnMaxRadius);
            yield return new WaitForSeconds(spawnCooldown);
        }
    }

}
