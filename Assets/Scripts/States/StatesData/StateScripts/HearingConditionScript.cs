using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HearingConditionScript : StateScript
{
    private float distanceToPlayer = 20f;
    public float hearingRange = 10f;
    
    public HearingConditionScript()
    {
        SetStateName("Hearing");
        //distanceToPlayer = Vector3.Distance(GameObject.FindWithTag("Player").transform.position, GameObject.FindWithTag("Enemy").transform.position);
    }
    
    public override void Execute()
    {
        throw new System.NotImplementedException();
    }

    public bool Condition()
    {
        if (distanceToPlayer <= hearingRange)
        {
            return true;
        }
        return false;
    }
}
