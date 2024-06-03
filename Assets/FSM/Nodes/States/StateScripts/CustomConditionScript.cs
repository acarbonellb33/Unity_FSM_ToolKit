namespace FSM.Nodes.States.StateScripts
{
    using System.Reflection;
    using Utilities;
    using UnityEngine;
    public class CustomConditionScript : StateScript, ICondition
    {
        //Add any properties specific to this state
        public string selectedGameObject;
        public string selectedComponent;
        public string selectedFunction;

        public CustomConditionScript()
        {
            // Set the state name to 'Custom' using the SetStateName method inherited from StateScript
            SetStateName("CustomCondition");
        }

        // Override the Execute method from the base StateScript class
        public bool Condition()
        {
            // Add the logic for this state
            // Make sure selectedGameObject and selectedComponent are not null
            if (selectedGameObject != null && selectedComponent != null)
            {
                var obj = FindGameObjectWithId<IDGenerator>(selectedGameObject);
                if (obj != null)
                {
                    Component component = obj.GetComponent(selectedComponent);
                    if (component != null)
                    {
                        MethodInfo methodInfo = component.GetType().GetMethod(selectedFunction);
                        if (methodInfo != null && methodInfo.ReturnType == typeof(bool))
                        {
                            // Invoke the method on the selected component of the selected GameObject
                            return (bool)methodInfo.Invoke(component, null);
                        }
                    }
                }
                
            }
            return true;
        }
        private GameObject FindGameObjectWithId<T>(string id) where T : MonoBehaviour
        {
            var components = FindObjectsOfType<T>();
            
            foreach (var component in components)
            {
                var idGenerator = component.GetComponent<IDGenerator>();
                if (idGenerator != null && idGenerator.GetUniqueID() == id)
                {
                    return component.gameObject;
                }
            }
            return null;
        }
    }
}
