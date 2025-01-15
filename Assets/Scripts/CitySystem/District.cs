using System;
using UnityEngine;

[CreateAssetMenu][Serializable]
public class District : ScriptableObject
{  
    [SerializeField] public ItemAmountLine[] itemsCreated;
    [SerializeField] public ItemAmountLine[] itemsConsumed;
    [SerializeField] float complexity;
    [SerializeField] District[] upgradesInto;
    
}
