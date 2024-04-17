using UnityEngine;

// HearingConditionScript class inherits from StateScript and implements ICondition interface
public class HearingConditionScript : StateScript, ICondition
{
    private float distanceToPlayer;
    public FSMOperands operand;// Public enum to define comparison operands
    public float hearingRange = 10f;
    
    public HearingConditionScript()
    {
        // Set the state name to "Hearing" using the SetStateName method inherited from StateScript
        SetStateName("Hearing");
    }

    // Implementation of the Condition method from the ICondition interface
    public bool Condition()
    {
        // Calculate the distance between the player and the agent
        distanceToPlayer = Vector3.Distance(player.transform.position, agent.transform.position);

        // Switch statement to check the condition based on the operand
        switch (operand)
        {
            case FSMOperands.LessThan:
                return distanceToPlayer < hearingRange;
            case FSMOperands.GreaterThan:
                return distanceToPlayer > hearingRange;
            case FSMOperands.EqualTo:
                return distanceToPlayer.Equals(hearingRange);
            case FSMOperands.NotEqualTo:
                return !distanceToPlayer.Equals(hearingRange);
            default:
                return false;
        }
    }
}