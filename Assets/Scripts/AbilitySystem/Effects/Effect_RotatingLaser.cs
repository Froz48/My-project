using UnityEngine;
using Unity.Netcode;

public class Effect_RotatingLaser : MonoBehaviour
{
    private float _rotationSpeed;
    private float _duration;

    public void Initialize(float duration, float rotationSpeed)
    {
        _duration = duration;
        _rotationSpeed = rotationSpeed;
        
        GetComponent<Effect_DestroyAfterDelay>().delay = _duration;
    }

    void Update()
    {

        transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime);
    }
}