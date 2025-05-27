using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Behaviours/BehaviourKeepDistance")]
public class BehaviourKeepDistance : NPCBehaviour {
    public override bool CheckConditions(NPCEntity npc) {
        return MyMath.GetDistanceToNearestPlayer(npc.transform.position) < npc.monsterData.detectionRadius;
    }

    public override void Act(NPCEntity npc, Animator animator = null, dynamic param = null)
    {
        Player player = MyMath.GetNearestPlayer(npc.transform.position);
        Vector2 targetPosition = (npc.transform.position - player.transform.position).normalized * npc.monsterData.attackDistance + player.transform.position;
        Vector2 moveDirection = targetPosition - (Vector2)npc.transform.position;
        if (moveDirection.magnitude > 0.2)
        {
            Vector2 newPosition2 = (Vector2)npc.transform.position + (targetPosition - (Vector2)npc.transform.position).normalized * npc.monsterData.movementSpeed * Time.deltaTime;
            npc.GetComponent<Rigidbody2D>().MovePosition(newPosition2);
            animator.SetFloat("MoveX", moveDirection.normalized.x);
            animator.SetFloat("MoveY", moveDirection.normalized.y);
        }
    }
}