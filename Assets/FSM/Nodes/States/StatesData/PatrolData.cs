using System.Collections.Generic;
using UnityEngine;

public class PatrolData : StateScriptData
{
    public List<GameObject> patrolPoints;
    
    public PatrolData()
    {
        SetStateName("Patrol");
    }
}
