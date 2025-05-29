
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Effect_DestroyAfterDelay : MonoBehaviour
{
    public void Initialize(float lifetime)
    {
        StartCoroutine(DestroyAfterDelay(lifetime));
    }
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(this.gameObject);
    }
}