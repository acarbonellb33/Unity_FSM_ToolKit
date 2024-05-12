namespace FSM.Nodes.States.StateScripts
{
    using UnityEngine;
    using Enumerations;
    // DistanceConditionScript class inherits from StateScript and implements ICondition interface
    public class DistanceConditionScript : StateScript, ICondition
    {
        private float _distanceToPlayer;
        public FsmOperands operand; // Public enum to define comparison operands
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
            _distanceToPlayer = Vector3.Distance(Player.transform.position, Agent.transform.position);

            // Switch statement to check the condition based on the operand
            switch (operand)
            {
                case FsmOperands.LessThan:
                    return _distanceToPlayer < distanceFromPlayer;
                case FsmOperands.GreaterThan:
                    return _distanceToPlayer > distanceFromPlayer;
                case FsmOperands.EqualTo:
                    return _distanceToPlayer.Equals(distanceFromPlayer);
                case FsmOperands.NotEqualTo:
                    return !_distanceToPlayer.Equals(distanceFromPlayer);
                default:
                    return false;
            }
        }
    }
}
