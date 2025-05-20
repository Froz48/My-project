using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
[CreateAssetMenu(fileName = "Projectile", menuName = "Abilities/Projectile")]
public class Projectile : Ability
{
    public GameObject prefab;
    public float projectileSpeed = 10f;
    public float lifetime = 3f;
    [ServerRpc]
    public override void AbilityUseServerRpc(Vector2 playerPosition, Vector2 targetPosition)
    {
        GameObject fireball = GameManager.Instantiate(prefab, playerPosition, Quaternion.identity);
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        fireball.AddComponent<Effect_DamageOnCollision>().Initialize(power, typeof(Player));
        fireball.AddComponent<Effect_DestroyAfterDelay>().Initialize(lifetime);
        Vector2 direction = (targetPosition - playerPosition).normalized;
        rb.velocity = direction * projectileSpeed;
        nextUseTime = Time.time + cooldown;
    }
}