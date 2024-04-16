using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HearingConditionScript : StateScript
{
    private float distanceToPlayer;
    public FSMOperands operand;
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
        distanceToPlayer = Vector3.Distance(player.transform.position, agent.transform.position);
        switch (operand)
        {
            case FSMOperands.LessThan:
                return distanceToPlayer < hearingRange;
            case FSMOperands.GreaterThan:
                return distanceToPlayer > hearingRange;
            case FSMOperands.EqualTo:
                return distanceToPlayer.Equals(hearingRange) ;
            case FSMOperands.NotEqualTo:
                return !distanceToPlayer.Equals(hearingRange) ;
            default:
                return false;
        }
    }
}
