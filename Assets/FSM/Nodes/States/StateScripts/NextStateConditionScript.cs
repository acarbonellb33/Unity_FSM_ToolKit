using UnityEngine;

public class NextStateStateScript : StateScript, ICondition
{
    //Add any properties specific to this state

     public NextStateStateScript()
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
