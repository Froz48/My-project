using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
public interface IDamageable
{
    void TakeDamageRpc(float damage);
}

public class Effect_DamageOnCollision : MonoBehaviour
{
    float damage;
    Type targetType;
    public void Initialize(float damage, Type targetType)
    {
        this.damage = damage;
        this.targetType = targetType;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var component = collision.GetComponent(targetType);
        if ((component != null) && component is IDamageable damageable)
        {
            damageable.TakeDamageRpc(damage);
        }
    }
}
