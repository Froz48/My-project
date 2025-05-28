using System;
using UnityEngine;

[CreateAssetMenu][Serializable]
public class District : ScriptableObject, IDatabaseObject
{
    int id;
    [SerializeField] public Reciepe[] reciepes; 
    [SerializeField] float complexity;
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
[Serializable]
public class Reciepe {
    [SerializeField] public ItemAmountLine[] itemsCreated;
    [SerializeField] public ItemAmountLine[] itemsConsumed;
}