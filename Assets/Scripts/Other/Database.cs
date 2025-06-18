using UnityEngine;

[CreateAssetMenu]
public class Database : ScriptableObject
{
    [SerializeField] private ScriptableObject[] objects;
    public ScriptableObject GetObjectById(int id)
    {
        if (id > objects.Length) return objects[0];
        return objects[id];
    }

    public ScriptableObject GetRandomObject()
    {
        return objects[Random.Range(0, objects.Length)];
    }

    [ContextMenu("Update ID's")]
    public void UpdateID()
    {
        Debug.Log("UpdatingDatabaseItemIds");
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] is IDatabaseObject item)
            {
                item.SetId(i);
            }
        }
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
    public ScriptableObject[] GetAllObjects()
    {
        return objects;
    }
}

public interface IDatabaseObject
{
    public void SetId(int id);
    public int GetId();
}
