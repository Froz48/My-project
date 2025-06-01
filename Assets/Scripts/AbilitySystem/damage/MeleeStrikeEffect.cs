using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class MeleeStrikeEffect : MonoBehaviour
{
     float damage;
    public void Initialize(float damage, float lifetime)
    {
        this.damage = damage;
        StartCoroutine(DestroyAfterDelay(lifetime));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out NPCEntity monsterWorld)){
            monsterWorld.TakeDamageRpc(damage);
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(this.gameObject);
    }
}