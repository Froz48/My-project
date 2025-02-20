using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class ItemBase : ScriptableObject
{
    [SerializeField] public int id;
    [SerializeField] public Sprite uiDisplay;  
    [SerializeField] public int stackSize;
    [TextArea(15, 20)][SerializeField] public string description;
}
