
using System.Collections;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "UseAbilityToPlayer", menuName = "Abilities/UseAbilityToPlayer")]
public class UseAbilityToPlayer : Ability
{
    public Ability ability;
    public int NumberOfTimes;
    public float delayBetweenSpam;
    public bool toEveryone;
    public override void AbilityUseServerRpc(Vector2 playerPosition, Vector2 targetPosition)
    {
        if (toEveryone)
        {
            foreach (var i in NetworkManager.Singleton.ConnectedClientsList)
            {
                ability.AbilityUseServerRpc(playerPosition, i.PlayerObject.transform.position);
            }
        }
        else
        {
            ability.AbilityUseServerRpc(playerPosition, NetworkManager.Singleton.ConnectedClientsList[Random.Range(0, NetworkManager.Singleton.ConnectedClientsList.Count)].PlayerObject.transform.position);
        }
    }
}