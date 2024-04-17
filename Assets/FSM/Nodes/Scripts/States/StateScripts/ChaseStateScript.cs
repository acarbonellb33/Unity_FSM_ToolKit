public class ChaseStateScript : StateScript, IAction
{
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
        agent.SetDestination(player.transform.position);
    }
}

