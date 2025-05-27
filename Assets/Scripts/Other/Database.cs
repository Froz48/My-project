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

    // [ContextMenu("Update ID's")]
    // private void UpdateID()
    // {
    //     Debug.Log("UpdatingDatabaseItemIds");
    //     for (int i = 0; i < objects.Length; i++)
    //     {
    //         if (objects[i].id != i)
    //             objects[i].id = i;
    //     }
    // }
}
