

using System.Collections;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class NPCEntity : NetworkBehaviour{

    float currentHealth;
    
    float despawnDistance = 40;
    [SerializeField] public NPCData monsterData;
    // [SerializeField] public Sprite sprite;
    Transform playerChased;
    private float nextAtackTime;
    

//---------------------------------------------------------------
    public override void OnNetworkSpawn(){
        if (IsServer){
            monsterData = monsterData.CreateInstance();
            // GetComponent<SpriteRenderer>().sprite = sprite;
            StartCoroutine(CheckForStateConditions());
            GetComponent<BoxCollider2D>().enabled = true; // huh?
            currentHealth = monsterData.maxHealth;
            StartCoroutine(DespawnCheck());
        }
    }

    private IEnumerator DespawnCheck(){
        while (true){
            if (GetDistanceToPlayer(FindNearestPlayer().transform) > despawnDistance){
                Die(false);
            }
            yield return new WaitForSeconds(5);
        }
    }
    private Player FindNearestPlayer(){
        float minDistance = 5000;
        Player nearestPlayer = null;
        foreach (var player in NetworkManager.Singleton.ConnectedClientsList){
            float distance = GetDistanceToPlayer(player.PlayerObject.transform);
            if (distance < minDistance){
                minDistance = distance;
                nearestPlayer = player.PlayerObject.GetComponent<Player>(); 
            }
        }
        return nearestPlayer;
    }
    
    private float GetDistanceToPlayer(Transform playerObject){
        return (playerObject.transform.position - transform.position).magnitude;
    }
    

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player player)){
            if (Time.time >= nextAtackTime)
                Atack(player);
        }
    }

    private void Atack(Player player){  // Change this later
        nextAtackTime = Time.time + monsterData.atackSpeed;
        player.GetDamage(monsterData.attackDamage);
    }
    public void FixedUpdate(){
        if (IsServer){
            monsterData.nPCBehaviour[activeStatePosition].Act(this);
            // Vector2 newPosition = transform.position + (playerChased.position - transform.position).normalized * monsterData.movementSpeed*Time.deltaTime;
            // GetComponent<Rigidbody2D>().MovePosition(newPosition);
        } 
    }
//---------------------------------------------------------------    
    // public void SetMonsterData(MonsterData _data){
    //     monsterData = _data;
    // }

   // [Rpc(SendTo.Server)]
    public void TakeDamageRpc(float damage){
        if (IsServer){
            currentHealth -= damage;
            if (currentHealth <= 0){
                Die();    
            }
        }
    }
//---------------------------------------------------------------
    private void Die(bool doDropLoot = true){
        if (doDropLoot)
            DropLoot();
        this.gameObject.GetComponent<NetworkObject>().Despawn();
        Destroy(this.gameObject);
        NPCSpawner enemySpawner = GameObject.FindObjectOfType<NPCSpawner>();
        enemySpawner.spawnedEnemyCount--;
    }
    
    private void DropLoot(){
        foreach(var i in monsterData.lootTable){
            if (i.dropChance - UnityEngine.Random.Range(0f, 1f) > 0){
                // i.item.SpawnWorldItemCopy(transform.position);
                GameObject groundItemPrefab = Resources.Load<GameObject>("GroundItemPrefab");
                var _gameObject = Instantiate(groundItemPrefab, transform.position, quaternion.identity);
                _gameObject.GetComponent<GroundItem>().setItem(i.item);
                _gameObject.GetComponent<SpriteRenderer>().sprite = _gameObject.GetComponent<GroundItem>().getItem().uiDisplay;
                _gameObject.GetComponent<NetworkObject>().Spawn();
            }  
        }
    }


    public int activeStatePosition = 0;

    private IEnumerator CheckForStateConditions(){
        while (true){
            for (int i = 0; i < monsterData.nPCBehaviour.Length; i++){
                if (i <= activeStatePosition) {
                    Debug.Log("activeStatePosition " + activeStatePosition + " is higher than " + i);
                    continue;
                }
                Debug.Log("Checking state " + i);
                if (monsterData.nPCBehaviour[i].CheckConditions(this)){
                    activeStatePosition = i;
                    Debug.Log("State changed to " + activeStatePosition);
                } else {
                    Debug.Log("State not changed");
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator Act(){
        while (true){
            
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator CheckForPlayerNearby()
    {
        while (true)
        {
            if (playerChased != null && (playerChased.position - transform.position).magnitude > monsterData.giveUpRadius){
                playerChased = null;
            }

            if (playerChased == null){   
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, monsterData.detectionRadius);
                foreach (Collider2D collider in colliders){
                    if (collider.GetComponent<Player>()!= null){
                        playerChased = collider.transform;
                        break;
                    }   
                }   
            }
            yield return new WaitForSeconds(0.5f); // Проверяем каждые 1 секунды
        }
    }
}