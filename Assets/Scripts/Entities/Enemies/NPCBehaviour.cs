using UnityEngine;

public abstract class NPCBehaviour : ScriptableObject {
    public abstract bool CheckConditions(NPCEntity npc);
    public abstract void Act(NPCEntity npc, Animator animator = null, dynamic param = null);
}

