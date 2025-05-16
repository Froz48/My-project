using UnityEngine;

[CreateAssetMenu(menuName = "Behaviours/BehaviourStandAfk")]
public class BehaviourStandAfk : NPCBehaviour
{
    public override void Act(NPCEntity npc, dynamic param = null)
    {
        
    }

    public override bool CheckConditions(NPCEntity npc)
    {
        return true;
    }
}