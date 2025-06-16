using System;
using UnityEngine;

[CreateAssetMenu][Serializable]
public class District : ScriptableObject, IDatabaseObject
{
    int id;
    [SerializeField] public Recipe[] recipes; 
    [SerializeField] public Sprite sprite;
    [SerializeField] District[] upgradesInto;

    public void SetId(int id)
    {
        this.id = id;
    }
    public int GetId()
    {
        return id;
    }
}
