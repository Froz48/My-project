using UnityEngine;

[CreateAssetMenu(menuName = "Behaviours/BehaviourKeepDistance")]
public class BehaviourKeepDistance : NPCBehaviour 
{
    private float distanceTolerance = 0.1f;
    public override bool CheckConditions(NPCEntity npc)
    {
        // Получаем текущую цель
        Player target = npc.GetCurrentTarget();
        
        if (npc == null || target == null || npc.MonsterData == null) return false;

        float distance = Vector2.Distance(npc.transform.position, target.transform.position);
        
        return distance < npc.MonsterData.detectionRadius &&
               distance > npc.MonsterData.attackDistance;
    }

    public override void Act(NPCEntity npc, Animator animator = null, dynamic param = null)
    {
        if (npc == null || npc.MonsterData == null || animator == null) return;

        Player player = npc.GetCurrentTarget();
        if (player == null)
        {
            animator.SetBool("IsMoving", false);
            return;
        }

        Vector2 directionAwayFromPlayer = (npc.transform.position - player.transform.position).normalized;
        Vector2 idealPosition = (Vector2)player.transform.position + directionAwayFromPlayer * npc.MonsterData.attackDistance;

        Vector2 moveDirection = (idealPosition - (Vector2)npc.transform.position).normalized;
        float distanceToIdealPos = Vector2.Distance(npc.transform.position, idealPosition);

        if (distanceToIdealPos > distanceTolerance)
        {
            float moveSpeed = npc.MonsterData.movementSpeed * Time.fixedDeltaTime;

            Rigidbody2D rb = npc.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 newPosition = (Vector2)npc.transform.position + moveDirection * moveSpeed;
                rb.MovePosition(newPosition);

                // Обновляем аниматор
                animator.SetFloat("MoveX", moveDirection.x);
                animator.SetFloat("MoveY", moveDirection.y);
                animator.SetBool("IsMoving", true);
            }
        }
        else animator.SetBool("IsMoving", false);

    }
}