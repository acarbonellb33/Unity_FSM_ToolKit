namespace FSM.Nodes.States.StateScripts
{
    using UnityEngine;
    public class NextStateConditionScript : StateScript, ICondition
    {
        //Add any properties specific to this state

        public NextStateConditionScript()
        {
            // Set the state name to 'NextState' using the SetStateName method inherited from StateScript
            SetStateName("NextState");
        }

        // Override the Execute method from the base StateScript class
        public bool Condition()
        {
            // Add the logic for this state
            return true; // Now returning true, replace with your logic
        }
    }
}
