
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "Abilities/AbilityData")]
public class AbilityData : ScriptableObject {
    public enum AbilityType { Projectile, AOE, Buff }
    [SerializeField] public Sprite sprite;
    [SerializeField] public float power = 1f;
    [SerializeField] public float cooldown = 1f;
    public GameObject projectilePrefab; 
    [SerializeField]public float lifetime = 1f;
    public float projectileSpeed;

}