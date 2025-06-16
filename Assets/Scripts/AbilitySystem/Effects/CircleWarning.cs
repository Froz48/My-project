using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class Effect_CircleWarning : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform circleTransform;
    
    private float growDuration;
    public void SetDuration(float duration) => growDuration = duration;

    private void Start()
    {
        StartAnimation(growDuration);
        circleTransform.localScale = Vector3.zero;
    }

    private void StartAnimation(float duration)
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
            
            circleTransform.localScale = Vector3.one * progress;
    
            yield return null;
        }
    }
}