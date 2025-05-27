using System;
using UnityEngine;

[CreateAssetMenu][Serializable]
public class District : ScriptableObject
{
    [SerializeField] public Reciepe[] reciepes; 
    [SerializeField] float complexity;
    [SerializeField] District[] upgradesInto;
    
}
[Serializable]
public class Reciepe {
    [SerializeField] public ItemAmountLine[] itemsCreated;
    [SerializeField] public ItemAmountLine[] itemsConsumed;
}