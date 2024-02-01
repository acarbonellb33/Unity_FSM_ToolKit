using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHearingCondition", menuName = "States Conditions/Hearing Condition")]
public class HearingCondition : State
{
    private float distanceToPlayer = 20f;
    public float hearingRange = 10f;
    
    private HearingCondition()
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
