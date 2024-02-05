using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDistanceCondition", menuName = "States Conditions/Distance Condition")]
public class DistanceCondition : State
{
    private float distanceToPlayer = 0f;
    public float distance = 10f;
    
    private DistanceCondition()
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
