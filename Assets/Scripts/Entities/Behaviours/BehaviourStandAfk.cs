using UnityEngine;

[CreateAssetMenu(menuName = "Behaviours/BehaviourStandAfk")]
public class BehaviourStandAfk : NPCBehaviour
{
    public override void Act(NPCEntity npc, Animator animator = null, dynamic param = null)
    {
        animator?.SetFloat("MoveX", 0);
        animator?.SetFloat("MoveY", 0);
    }

    public override bool CheckConditions(NPCEntity npc)
    {
        return true;
    }
}