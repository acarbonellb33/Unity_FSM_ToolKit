using UnityEngine;

namespace FSM.Nodes.States.StateScripts
{
    public class ChaseStateScript : StateScript, IAction
    {
        public float chaseSpeed = 5f;
        public float chaseRange = 10f;

        private Vector3 _initialPosition = Vector3.zero;
        public ChaseStateScript()
        {
            // Set the state name to "Chase" using the SetStateName method inherited from StateScript
            SetStateName("Chase");
        }

        // Override the Execute method from the base StateScript class
        public void Execute()
        {
            if(_initialPosition == Vector3.zero)
            {
                _initialPosition = Agent.transform.position;
            }
            // Set the agent's destination to the player's current position
            // 'agent' and 'player' are inherited from the parent class StateScript
            if (Vector3.Distance(Player.transform.position, Agent.transform.position) <= chaseRange)
            {
                Agent.speed = chaseSpeed;
                Agent.SetDestination(Player.transform.position);
            }
            else
            {
                if (Agent.remainingDistance > 0.1f)
                {
                    Agent.SetDestination(_initialPosition);
                }
            }
        }
    }
}