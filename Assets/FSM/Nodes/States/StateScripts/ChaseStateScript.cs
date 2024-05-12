namespace FSM.Nodes.States.StateScripts
{
    public class ChaseStateScript : StateScript, IAction
    {
        public float chaseSpeed = 5f;
        public float chaseRange = 10f;
        public float chaseCooldown = 2f;
        public bool canChase = true;

        public ChaseStateScript()
        {
            // Set the state name to "Chase" using the SetStateName method inherited from StateScript
            SetStateName("Chase");
        }

        // Override the Execute method from the base StateScript class
        public void Execute()
        {
            // Set the agent's destination to the player's current position
            // 'agent' and 'player' are inherited from the parent class StateScript
            Agent.SetDestination(Player.transform.position);
        }
    }
}