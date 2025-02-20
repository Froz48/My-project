using UnityEngine;

[CreateAssetMenu]
public class Database : ScriptableObject
{
    [SerializeField] private ScriptableObject[] objects;
    public ScriptableObject GetObjectById(int id){
        return objects[id];
    }

    public ScriptableObject GetRandomObject(){
        return objects[Random.Range(0, objects.Length)];
    }
}
