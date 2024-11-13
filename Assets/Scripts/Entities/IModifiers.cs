using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModifier
{
    void AddValue(ref int baseValue);

    //void MultiplyValue(ref int baseValue);
}
