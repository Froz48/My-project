using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "RotatingLaser", menuName = "Abilities/Boss/RotatingLaser")]
public class Ability_RotatingLaser : Ability
{
    [Header("Laser Settings")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private float duration = 10f; 
    [SerializeField] private float rotationSpeed = 30f; // Градусов в секунду

    public override void AbilityUse(Vector2 playerPosition, Vector2 targetPosition)
    {
        Debug.LogError("Используйте перегруженную версию AbilityUse(Transform caster, ...)");
    }

    public void AbilityUse(Transform caster)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        GameObject laserInstance = Instantiate(laserPrefab, caster.position, caster.rotation);
        
        laserInstance.transform.SetParent(caster);

        NetworkObject netObj = laserInstance.GetComponent<NetworkObject>();
        netObj.Spawn(true);

        laserInstance.GetComponent<Effect_RotatingLaser>().Initialize(duration, rotationSpeed);
    }
}