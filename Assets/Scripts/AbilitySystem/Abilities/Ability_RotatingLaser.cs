// Файл: Ability_RotatingLaser.cs
using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "RotatingLaser", menuName = "Abilities/Boss/RotatingLaser")]
public class Ability_RotatingLaser : Ability
{
    [Header("Laser Settings")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private float duration = 10f; // Как долго луч будет существовать
    [SerializeField] private float rotationSpeed = 30f; // Градусов в секунду

    // Мы немного изменим сигнатуру, чтобы передавать самого "заклинателя" (босса)
    // Это гораздо гибче, чем просто передавать позицию
    public override void AbilityUse(Vector2 playerPosition, Vector2 targetPosition)
    {
        Debug.LogError("Используйте перегруженную версию AbilityUse(Transform caster, ...)");
    }

    public void AbilityUse(Transform caster)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        // Создаем префаб на позиции босса
        GameObject laserInstance = Instantiate(laserPrefab, caster.position, caster.rotation);
        
        // Привязываем луч к боссу, чтобы он двигался вместе с ним
        laserInstance.transform.SetParent(caster);

        // Спавним объект в сети
        NetworkObject netObj = laserInstance.GetComponent<NetworkObject>();
        netObj.Spawn(true);

        // Инициализируем параметры луча (длительность, скорость вращения)
        laserInstance.GetComponent<Effect_RotatingLaser>().Initialize(duration, rotationSpeed);
    }
}