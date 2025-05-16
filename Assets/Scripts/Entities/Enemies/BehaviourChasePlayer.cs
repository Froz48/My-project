using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Behaviours/BehaviourChasePlayer")]
public class BehaviourChasePlayer : NPCBehaviour {
    public override bool CheckConditions(NPCEntity npc) {
        return (FindNearestPlayer(npc).transform.position - npc.transform.position).magnitude < npc.monsterData.detectionRadius;
    }
    private float GetDistanceToPlayer(NPCEntity npc, Transform playerObject){
        return (playerObject.transform.position - npc.transform.position).magnitude; 
    }
    
    private Player FindNearestPlayer(NPCEntity npc){
        float minDistance = 5000;
        Player nearestPlayer = null;
        foreach (var player in NetworkManager.Singleton.ConnectedClientsList){
            float distance = GetDistanceToPlayer(npc, player.PlayerObject.transform);
            if (distance < minDistance){
                minDistance = distance;
                nearestPlayer = player.PlayerObject.GetComponent<Player>();
            }
        }
        return nearestPlayer;
    }


    public override void Act(NPCEntity npc, dynamic param = null) {
            Player player = FindNearestPlayer(npc);
            Vector2 newPosition = npc.transform.position + (player.transform.position - npc.transform.position).normalized * npc.monsterData.movementSpeed*Time.deltaTime;
            npc.GetComponent<Rigidbody2D>().MovePosition(newPosition);
            Debug.Log("Chasing player");
    }
}