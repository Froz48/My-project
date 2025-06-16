using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Behaviours/UseAbility")]
public class BehaviourUseAbility : NPCBehaviour
{
    public override bool CheckConditions(NPCEntity npc)
    {
        if (npc == null || npc.MonsterData == null || npc._abilities == null)
            return false;

        float distance = MyMath.GetDistanceToNearestPlayer(npc.transform.position);
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

        Player player = MyMath.GetNearestPlayer(npc.transform.position);
        if (player == null) return;

        for (int i = 0; i < npc._abilities.Length; i++)
        {
            if (npc._abilities[i] != null &&
                npc._abilities[i].IsReady() &&
                MyMath.GetDistanceToNearestPlayer(npc.transform.position) < npc.MonsterData.attackDistance)
            {
                npc.UseAbilityServerRpc(player.transform.position, npc._abilities[i].id);
                npc._abilities[i].StartCooldown();

                // Обновляем аниматор
                if (animator != null)
                {
                    animator.SetTrigger("Action1");
                    animator.SetBool("IsMoving", false);
                }
            }
        }
    }


}