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
    public string targetTypeString; 
    public float damage;
    public float powerCoefficient = 1;
    Type targetType;
    public void Initialize(float damage, Type targetType)
    {
        this.damage = damage;
        this.targetType = targetType;
    }
    public void Start()
    {
        if (targetType == null)
        {
            targetType = Type.GetType(targetTypeString);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IDamageable damageable) && damageable.GetType() == targetType)
        {
            damageable.TakeDamageRpc(damage * EffectController.GetPower(gameObject) * powerCoefficient);
        }
        // var component = collision.GetComponent(targetType);
        // if ((component != null) && component is IDamageable damageable)
        // {
        //     damageable.TakeDamageRpc(damage * EffectController.GetPower(gameObject) * powerCoefficient);
        // }
    }
}
