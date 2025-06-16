
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Effect_DestroyAfterDelay : NetworkBehaviour
{
    public float delay;
    public void Start()
    {
        // if (IsServer)
        StartCoroutine(DestroyAfterDelay());
    }
    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        // NetworkObject.Despawn();
        Destroy(this.gameObject);
    }

}