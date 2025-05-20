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
        Vector2 moveDirection = player.transform.position - npc.transform.position;
        if (moveDirection.magnitude < npc.monsterData.attackDistance) moveDirection = moveDirection * -1;
        Vector2 newPosition = (Vector2)npc.transform.position + (moveDirection.normalized * npc.monsterData.movementSpeed * Time.deltaTime);
        npc.GetComponent<Rigidbody2D>().MovePosition(newPosition);
        animator.SetFloat("MoveX", moveDirection.normalized.x);
        animator.SetFloat("MoveY", moveDirection.normalized.y);
    }
}