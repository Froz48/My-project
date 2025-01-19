using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new MonsterDatabase", menuName = "Databases/MonsterDatabase")]
public class MonsterDatabase : ScriptableObject, ISerializationCallbackReceiver
{
    public NPCData[] monsters;

    [ContextMenu("Update ID's")]
    private void UpdateID()
    {
        for (int i = 0; i < monsters.Length; i++)
        {
            if (monsters[i].Id != i)
                monsters[i].Id = i;
        }
    }
    public void OnAfterDeserialize()
    {
        UpdateID();
    }

    public void OnBeforeSerialize()
    {
        UpdateID();
    }
}
