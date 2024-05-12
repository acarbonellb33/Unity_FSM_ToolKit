namespace FSM.Nodes.States.StateScripts
{
    using System.Reflection;
    public class CustomStateScript : StateScript, IAction
    {
        //Add any properties specific to this state
        public string selectedGameObject;
        public string selectedComponent;
        public string selectedFunction;

        public CustomStateScript()
        {
            // Set the state name to 'Custom' using the SetStateName method inherited from StateScript
            SetStateName("Custom");
        }

        // Override the Execute method from the base StateScript class
        public void Execute()
        {
            // Add the logic for this state
            // Make sure selectedGameObject and selectedComponent are not null
            if (selectedGameObject != null && selectedComponent != null)
            {
                // Get the method info of the selected function
                MethodInfo methodInfo = selectedComponent.GetType().GetMethod(selectedFunction);

                // Check if the method exists
                if (methodInfo != null)
                {
                    // Invoke the method on the selected component of the selected GameObject
                    methodInfo.Invoke(selectedComponent, null);
                }
            }
        }
    }
}
