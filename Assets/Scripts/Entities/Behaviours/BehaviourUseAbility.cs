using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Behaviours/UseAbility")]
public class BehaviourUseAbility : NPCBehaviour
{
    public override bool CheckConditions(NPCEntity npc)
    {
        // Получаем текущую цель, которую выбрал сам NPC
        Player target = npc.GetCurrentTarget();
        
        if (npc == null || target == null || npc.MonsterData == null || npc._abilities == null)
            return false;

        float distance = Vector2.Distance(npc.transform.position, target.transform.position);
        if (distance > npc.MonsterData.attackDistance)
            return false;

        foreach (var ability in npc._abilities)
        {
            if (ability != null && ability.IsReady())
            {
                return true;
            }
        }
        return false;
    }

    public override void Act(NPCEntity npc, Animator animator = null, dynamic param = null)
    {
        if (npc == null || npc.MonsterData == null || npc._abilities == null) return;

        // Берем цель напрямую из NPC
        Player player = npc.GetCurrentTarget();
        if (player == null) return;

        for (int i = 0; i < npc._abilities.Length; i++)
        {
            if (npc._abilities[i] != null && npc._abilities[i].IsReady() &&
                Vector2.Distance(npc.transform.position, player.transform.position) < npc.MonsterData.attackDistance)
            {
                npc.UseAbilityServerRpc(player.transform.position, npc._abilities[i].id);
                npc._abilities[i].StartCooldown();

                if (animator != null)
                {
                    animator.SetTrigger("Action1");
                    animator.SetBool("IsMoving", false);
                }
            }
        }
    }
}