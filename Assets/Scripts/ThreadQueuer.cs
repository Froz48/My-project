using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class ThreadQueuer
{
    public static List<Action> mainThreadActions = new List<Action>();
    public static void QueueMainThreadFunction(Action func){
        mainThreadActions.Add(func);
    }
}
