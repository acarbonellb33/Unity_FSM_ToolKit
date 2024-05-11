using System.Collections.Generic;
using UnityEngine;

public class PatrolData : StateScriptData
{
    public List<string> patrolPoints;
    
    public PatrolData()
    {
        SetStateName("Patrol");
    }
}
