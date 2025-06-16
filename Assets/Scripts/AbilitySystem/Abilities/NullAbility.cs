

using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "NullAbility", menuName = "Abilities/NullAbility")]
public class NullAbility : Ability{
    [ServerRpc]
    public override void AbilityUse(Vector2 playerPosition, Vector2 targetPosition)
    {
        
    }

}