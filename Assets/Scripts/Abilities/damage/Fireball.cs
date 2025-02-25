using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "FireballAbility", menuName = "Abilities/Fireball")]
public class FireballAbility : Ability
{
    public GameObject fireballPrefab; // Префаб огненного шара
    public float fireballSpeed = 10f; // Скорость огненного шара

    [ServerRpc]
    public override void AbilityUseServerRpc(Vector2 playerPosition, Vector2 targetPosition)
    {
        GameObject fireball = Instantiate(fireballPrefab, playerPosition, Quaternion.identity);
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        fireball.AddComponent<Effect_DamageOnCollision>().Initialize(damage);
        fireball.AddComponent<Effect_DestroyAfterDelay>().Initialize(lifetime);
        Vector2 direction = (targetPosition - playerPosition).normalized;
        rb.velocity = direction * fireballSpeed;
    }
}