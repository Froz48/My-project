

using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "NullAbility", menuName = "Abilities/NullAbility")]
public class NullAbility : Ability{
    [ServerRpc]
    public override void AbilityUseServerRpc(Vector2 playerPosition, Vector2 targetPosition){
        Debug.Log("NullAbility ServerRpc");
        //null ability
    }

    // public NullAbility CreateInstance(){
    //     return (NullAbility)this.MemberwiseClone();
    // }
}