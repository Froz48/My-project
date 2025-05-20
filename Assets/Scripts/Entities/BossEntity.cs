using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class BossEntity : NetworkBehaviour, IDamageable
{
    [SerializeField] BossData data;
    float currentHealth;
    float currentTimer;
    int timerPosition = 0;

    void Start()
    {

    }

    void Update()
    {
        if (currentTimer >= data.timer[timerPosition].timer)
        {
            data.timer[timerPosition].ability.AbilityUseServerRpc(this.gameObject.transform.position, new Vector2(0, 0));
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

    public void TakeDamageRpc(float damage)
    {
        currentHealth -= damage;
        VictoryCheck();
    }

    void VictoryCheck()
    {
        if (currentHealth <= 0)
        {
            GetComponent<NetworkObject>().Despawn();
            DropLoot();
        }
    }

    void DropLoot()
    {
        foreach (var i in data.loot.generateLoot())
        {
            GameObject groundItemPrefab = Resources.Load<GameObject>("GroundItemPrefab");
            var _gameObject = Instantiate(groundItemPrefab, transform.position, quaternion.identity);
            gameObject.GetComponent<GroundItem>().setItem(i);
            _gameObject.GetComponent<SpriteRenderer>().sprite = _gameObject.GetComponent<GroundItem>().getItem().uiDisplay;
            _gameObject.GetComponent<NetworkObject>().Spawn();
        }
    }

}
