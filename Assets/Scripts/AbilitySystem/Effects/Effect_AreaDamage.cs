
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_AreaDamage : MonoBehaviour
{
    [Header("Settings")]
    public float radius;
    public float frequency;
    public float damage;
    public string targetTypeString;
    public Type targetType;
    public void Start()
    {
        targetType = Type.GetType(targetTypeString);
        StartCoroutine(EAreaDamage());
    }
    private void AreaDamage()
    {
        //logic
        Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var i in collider2Ds)
        {
            var component = i.GetComponent(targetType);
            if ((component != null) && (component is IDamageable damageable))
            {
                damageable.TakeDamage(damage);
            }
        }

    }
    private IEnumerator EAreaDamage()
    {
        while (gameObject)
        {
            AreaDamage();
            yield return new WaitForSeconds(frequency);
        }
    }
    
}