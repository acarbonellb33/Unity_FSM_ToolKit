using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeingConditionScript : StateScript, ICondition
{
    public float distance = 10f;
    
    public SeeingConditionScript()
    {
        SetStateName("Seeing");
    }

    public bool Condition()
    {
        RaycastHit hit;
        if (Physics.Raycast(agent.transform.GetChild(0).position, agent.transform.GetChild(0).transform.forward, out hit, distance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }
}
