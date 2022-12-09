using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlockUnitStates : ScriptableObject
{
    public abstract void EnterState(FlockUnitController horse);
    public abstract FlockUnitStates DoState(FlockUnitController horse);
}
