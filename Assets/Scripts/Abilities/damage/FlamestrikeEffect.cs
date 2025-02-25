using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Effect_DamageOnCollision : MonoBehaviour
{
    float damage;
    public void Initialize(float damage)
    {
        this.damage = damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out NPCEntity monsterWorld)){
            monsterWorld.TakeDamageRpc(damage);
        }
    }
}