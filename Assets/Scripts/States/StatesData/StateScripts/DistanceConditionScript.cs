using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceConditionScript : StateScript
{
    private float distanceToPlayer = 0f;
    public float distance = 10f;
    
    public DistanceConditionScript()
    {
        SetStateName("Distance");
    }

    public override void Execute()
    {
        throw new System.NotImplementedException();
    }

    public bool Condition()
    {
        distanceToPlayer = Vector3.Distance(player.transform.position, agent.transform.position);
        if (distanceToPlayer <= distance)
        {
            return true;
        }
        return false;
    }
}
