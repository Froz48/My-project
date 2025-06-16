using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class LineWarning : NetworkBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform outlineTransform; // Трансформ рамки
    [SerializeField] private Transform fillTransform;    // Трансформ заполнения
    private Vector2 size;

    private float _animationDuration;

    // Этот метод будет вызываться для запуска анимации
    public void StartWarning(float duration, Vector2 size)
    {
        this.size = size;
        _animationDuration = duration;
        GetComponent<Effect_DestroyAfterDelay>().delay = duration;

        if (outlineTransform != null)
        {
            outlineTransform.localScale = new Vector3(size.x, size.y, 1);
        }
        StartCoroutine(FillRoutine());

    }

    private IEnumerator FillRoutine()
    {
        float timer = 0f;
        while (timer < _animationDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / _animationDuration)* size.y;

            if (fillTransform != null)
            {
                fillTransform.localScale = new Vector3(size.x, progress, 1);
            }

            yield return null;
        }
    }
}