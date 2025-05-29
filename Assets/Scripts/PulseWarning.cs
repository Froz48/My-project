using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class PulseWarning : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform circleTransform;
    
    private float growDuration;
    public void SetDuration(float duration) => growDuration = duration;

    private void Start()
    {
        
        if (IsServer)
        {
            StartAnimationClientRpc(growDuration);
        }
        
        circleTransform.localScale = Vector3.zero;
    }

    [ClientRpc]
    private void StartAnimationClientRpc(float duration)
    {
        growDuration = duration;
        StartCoroutine(GrowAnimation());
    }

    private IEnumerator GrowAnimation()
    {
        float timer = 0f;
        while (timer < growDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / growDuration);
            
            // Масштабирование
            circleTransform.localScale = Vector3.one * progress;
    
            yield return null;
        }
    }
}