using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeingConditionScript : StateScript
{
    public float distance = 10f;
    
    public SeeingConditionScript()
    {
        SetStateName("Seeing");
    }

    public override void Execute()
    {
        throw new System.NotImplementedException();
    }

    public bool Condition()
    {
        RaycastHit hit;
        if (Physics.Raycast(agent.transform.GetChild(0).position, agent.transform.GetChild(0).transform.forward, out hit, distance))
        {
            if (hit.collider.tag == "Player")
            {
                return true;
            }
        }
        return false;
    }
}
