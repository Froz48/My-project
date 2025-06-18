using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class BossEntity : NetworkBehaviour, IDamageable
{
    [SerializeField] BossData data;
    float currentHealth;
    private int spawnerAltarId;
    float currentTimer;
    NetworkVariable<float> networkCurrentHealth = new NetworkVariable<float>();
    int timerPosition = 0;

    void Start()
    {

    }
    public void Initialize(BossData bossData, int altarId)
    {
        data = bossData;
        networkCurrentHealth.Value = data.maxHealth;
        spawnerAltarId = altarId;
    }

    void Update()
    {
        if (currentTimer >= data.timer[timerPosition].timer)
        {
            data.timer[timerPosition].ability.AbilityUse(this.gameObject.transform.position, new Vector2(0, 0));
            timerPosition++;
            if (timerPosition >= data.timer.Length)
            {
                timerPosition = 0;
                currentTimer = 0;
            }
        }
        currentTimer += Time.deltaTime;
        checkForAnyPlayerAlive();
    }

    void checkForAnyPlayerAlive()
    {
        bool yes = false;
        foreach (var i in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (i.PlayerObject.GetComponent<Player>().getCurrentHealth() > 0)
            {
                yes = true;
            }
        }
        if (yes == false)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }

    public void TakeDamage(float damage)
    {
        if (IsServer)
        {
            networkCurrentHealth.Value -= damage;
            VictoryCheck();
        }
    }
    void VictoryCheck()
    {
        if (networkCurrentHealth.Value <= 0)
        {
            BossAltarManager.Instance.UpdateAltarStateClientRpc(spawnerAltarId, false);

            DropLoot();
            GetComponent<NetworkObject>().Despawn();
        }
    }

    void DropLoot()
    {
        if (IsServer)
        {
            foreach (var loot in data.loot.lootDrops)
            {
                if (UnityEngine.Random.value <= loot.dropChance)
                {
                    SpawnLootItem(loot.item);
                }
            }
        }

    }
        private void SpawnLootItem(Item item)
    {
        var lootPrefab = Resources.Load<GameObject>("GroundItemPrefab");
        var loot = Instantiate(lootPrefab, transform.position, Quaternion.identity);
        loot.GetComponent<NetworkObject>().Spawn();
        loot.GetComponent<GroundItem>().SetItemClientRpc(item.id);
    }
}
