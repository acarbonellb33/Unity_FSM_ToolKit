namespace FSM.Nodes.States.StateScripts
{
    using UnityEngine;

    // SeeingConditionScript class inherits from StateScript and implements ICondition interface
    public class SeeingConditionScript : StateScript, ICondition
    {
        // Public float variable to define the distance for raycasting
        public float distance = 10f;

        public SeeingConditionScript()
        {
            // Set the state name to "Seeing" using the SetStateName method inherited from StateScript
            SetStateName("Seeing");
        }

        // Implementation of the Condition method from the ICondition interface
        public bool Condition()
        {
            // Declare a RaycastHit variable to store information about the raycast hit
            RaycastHit hit;

            // Perform a raycast from the agent's first child's position in the forward direction
            if (Physics.Raycast(Agent.transform.GetChild(0).position, Agent.transform.GetChild(0).transform.forward,
                    out hit, distance))
            {
                // Check if the raycast hit a collider with the tag "Player"
                if (hit.collider.CompareTag("Player"))
                {
                    // Return true if the raycast hit the player
                    return true;
                }
            }

            return false;
        }
    }
}
