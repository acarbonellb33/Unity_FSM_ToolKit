#if UNITY_EDITOR
namespace FSM.Nodes.States.StateScripts
{
    using UnityEngine;
    using Enumerations;
    // DistanceConditionScript class inherits from StateScript and implements ICondition interface
    public class DistanceConditionScript : StateScript, ICondition
    {
        private float distanceToPlayer;
        public FSMOperands operand; // Public enum to define comparison operands
        public float distanceFromPlayer = 10f;

        public DistanceConditionScript()
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
                    return distanceToPlayer < distanceFromPlayer;
                case FSMOperands.GreaterThan:
                    return distanceToPlayer > distanceFromPlayer;
                case FSMOperands.EqualTo:
                    return distanceToPlayer.Equals(distanceFromPlayer);
                case FSMOperands.NotEqualTo:
                    return !distanceToPlayer.Equals(distanceFromPlayer);
                default:
                    return false;
            }
        }
    }
}
#endif