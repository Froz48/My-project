

using System;
using System.Collections;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class NPCEntity : NetworkBehaviour, IDamageable
{

    [NonSerialized] public Ability[] abilities;
    float currentHealth;
    float despawnDistance = 40;
    public int activeStatePosition = 0;
    Animator animator;
    [SerializeField] public NPCData monsterData;
    // [SerializeField] public Sprite sprite;


    //---------------------------------------------------------------
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // monsterData = monsterData.CreateInstance(); //doesnt need animore

        }
        animator = GetComponent<Animator>();
        Debug.Log("Spawned, animator: " + animator);
    }

    public void InitializeAbilities()
    {
        abilities = new Ability[monsterData.abilities.Length];
        for (int i = 0; i < monsterData.abilities.Length; i++)
        {
            abilities[i] = monsterData.abilities[i].CreateInstance();
        }
    }
    private IEnumerator DespawnCheck()
    {
        while (true)
        {
            if (GetDistanceToPlayer(FindNearestPlayer().transform) > despawnDistance)
            {
                Die(false);
            }
            yield return new WaitForSeconds(5);
        }
    }
    private Player FindNearestPlayer()
    {
        float minDistance = 5000;
        Player nearestPlayer = null;
        foreach (var player in NetworkManager.Singleton.ConnectedClientsList)
        {
            float distance = GetDistanceToPlayer(player.PlayerObject.transform);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPlayer = player.PlayerObject.GetComponent<Player>();
            }
        }
        return nearestPlayer;
    }

    private float GetDistanceToPlayer(Transform playerObject)
    {
        return (playerObject.transform.position - transform.position).magnitude;
    }

    public void FixedUpdate()
    {
        if (IsServer)
        {

            monsterData.nPCBehaviour[activeStatePosition].Act(this, animator);

        }
    }


    // [Rpc(SendTo.Server)]
    public void TakeDamageRpc(float damage)
    {
        if (IsServer)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    private void Die(bool doDropLoot = true)
    {
        if (doDropLoot)
            DropLoot();
        this.gameObject.GetComponent<NetworkObject>().Despawn();
        Destroy(this.gameObject);
        NPCSpawner enemySpawner = GameObject.FindObjectOfType<NPCSpawner>();
        enemySpawner.spawnedEnemyCount--;
    }

    private void DropLoot()
    {
        foreach (var i in monsterData.lootTable)
        {
            if (i.dropChance - UnityEngine.Random.Range(0f, 1f) > 0)
            {
                // i.item.SpawnWorldItemCopy(transform.position);
                GameObject groundItemPrefab = Resources.Load<GameObject>("GroundItemPrefab");
                var _gameObject = Instantiate(groundItemPrefab, transform.position, quaternion.identity);
                _gameObject.GetComponent<GroundItem>().setItem(i.item);
                _gameObject.GetComponent<SpriteRenderer>().sprite = _gameObject.GetComponent<GroundItem>().getItem().uiDisplay;
                _gameObject.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
    private IEnumerator CheckForStateConditions()
    {
        while (true)
        {
            for (int i = monsterData.nPCBehaviour.Length - 1; i >= 0; i--)
            {
                if (i <= activeStatePosition)
                {
                    if (i == activeStatePosition)
                    {
                        if (!monsterData.nPCBehaviour[i].CheckConditions(this))
                        {
                            activeStatePosition = 0;
                        }
                    }
                    continue;
                }
                if (monsterData.nPCBehaviour[i].CheckConditions(this))
                {
                    activeStatePosition = i;
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    public void setData(NPCData data)
    {
        monsterData = data;
        InitializeAbilities();
        StartCoroutine(CheckForStateConditions());
        GetComponent<BoxCollider2D>().enabled = true; // huh?
        currentHealth = monsterData.maxHealth;
        StartCoroutine(DespawnCheck());
    }

}