
using UnityEngine;

public class EffectController : MonoBehaviour
{
    public static float GetPower(GameObject effectObject) // is 1 a magic number?
    {
        Effect_HasOwner ownerRef = effectObject.GetComponent<Effect_HasOwner>();
        if (ownerRef != null && ownerRef.owner != null)
        {
            if (ownerRef.owner.TryGetComponent(out Player player))
            {
                return player.GetPower();
            }
            else if (ownerRef.owner.TryGetComponent(out NPCEntity npc))
            {
                return 1f;
            }
            else if (ownerRef.owner.TryGetComponent(out BossData boss))
            {
                return 1f;
            }
        }
        return 1;
    }
    public void Initialize(Vector2 startPos, Vector2 targetPos)
    {
        if (TryGetComponent<Effect_MoveInDirection>(out var effect))
        {
            effect.direction = (targetPos - startPos).normalized;
        }
    }

    public static Vector2 GetOwnerPosition(GameObject effectObject)
    {
        Effect_HasOwner ownerRef = effectObject.GetComponent<Effect_HasOwner>();
        return (Vector2)ownerRef.transform.position;
    }
}