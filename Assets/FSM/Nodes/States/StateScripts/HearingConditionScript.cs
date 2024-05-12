#if UNITY_EDITOR
namespace FSM.Nodes.States.StateScripts
{
    using UnityEngine;

    // HearingConditionScript class inherits from StateScript and implements ICondition interface
    public class HearingConditionScript : StateScript, ICondition
    {
        // Public float variable to define the distance for checking hearing range
        public float hearingRange = 20f;

        // Public float variable to define the minimum speed threshold for the player to be heard
        public float minPlayerSpeed = 5f;

        public HearingConditionScript()
        {
            // Set the state name to "Hearing" using the SetStateName method inherited from StateScript
            SetStateName("Hearing");
        }

        // Implementation of the Condition method from the ICondition interface
        public bool Condition()
        {
            // Get the distance between the enemy and the player
            float distanceToPlayer = Vector3.Distance(agent.transform.position, player.transform.position);
            Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();

            // Check if the player is within the hearing range of the enemy
            if (distanceToPlayer <= hearingRange)
            {
                // Check if the player's speed is greater than the minimum threshold
                if (playerRigidbody.velocity.magnitude >= minPlayerSpeed)
                {
                    // Return true if the player is within the hearing range and moving at sufficient speed
                    return true;
                }
            }

            // Return false if the player is outside the hearing range or not moving fast enough
            return false;
        }
    }
}
#endif