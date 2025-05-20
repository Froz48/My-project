

using UnityEngine;
[CreateAssetMenu(menuName = "Behaviours/UseAbility")]
public class BehaviourUseAbility : NPCBehaviour {
    public override bool CheckConditions(NPCEntity npc) {
        foreach (var a in npc.abilities){
            if ((a.nextUseTime < Time.time)&&(MyMath.GetDistanceToNearestPlayer(npc.transform.position) < npc.monsterData.attackDistance)){
                return true;
            }
        }
        return false;
    }

    public override void Act(NPCEntity npc, Animator animator = null, dynamic param = null)
    {
        foreach (var a in npc.abilities){
            if ((a.nextUseTime < Time.time)&&(MyMath.GetDistanceToNearestPlayer(npc.transform.position) < npc.monsterData.attackDistance)){
                a.AbilityUseServerRpc(npc.transform.position, MyMath.GetNearestPlayer(npc.transform.position).transform.position);
            }
        }
    }
}