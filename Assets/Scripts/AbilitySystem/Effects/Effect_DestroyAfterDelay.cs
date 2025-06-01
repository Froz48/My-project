
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Effect_DestroyAfterDelay : MonoBehaviour
{
    public float delay;
    public void Start()
    {
        StartCoroutine(DestroyAfterDelay());
    }
    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        Destroy(this.gameObject);
    }
    public void Initialize(float any)
    {
        Debug.Log(gameObject.name + "is outdated, replace it");
    }
}