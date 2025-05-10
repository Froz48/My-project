using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FireballAbility : Ability
{
    [ServerRpc]
    public override void AbilityUseServerRpc(Vector2 playerPosition, Vector2 targetPosition)
    {
        GameObject fireball = GameManager.Instantiate(abilityData.projectilePrefab, playerPosition, Quaternion.identity);
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        fireball.AddComponent<Effect_DamageOnCollisionToMonster>().Initialize(abilityData.power);
        fireball.AddComponent<Effect_DestroyAfterDelay>().Initialize(abilityData.lifetime);
        Vector2 direction = (targetPosition - playerPosition).normalized;
        rb.velocity = direction * abilityData.projectileSpeed;
    }
}