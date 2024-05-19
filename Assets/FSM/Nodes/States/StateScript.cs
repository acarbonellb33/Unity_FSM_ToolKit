
namespace FSM.Nodes.States
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;
    using UnityEngine.AI;
    /// <summary>
    /// Abstract base class for state scripts.
    /// </summary>
    [Serializable]
    public abstract class StateScript : MonoBehaviour
    {
        private string _stateName;
        /// <summary>
        /// Reference to the player GameObject.
        /// </summary>
        protected GameObject Player;
        /// <summary>
        /// Reference to the NavMeshAgent component.
        /// </summary>
        protected NavMeshAgent Agent;
        
        private void Awake()
        {
            // Get the NavMeshAgent component attached to the same GameObject as this script
            Agent = GetComponent<NavMeshAgent>();

            // Find the "Player" GameObject in the scene and assign it to the player variable
            var playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                Player = playerObject;
            }
        }
        /// <summary>
        /// Inspects public variables of the StateScript and returns their values.
        /// </summary>
        /// <returns>A list of strings representing the inspected variables.</returns>
        public List<string> InspectVariables()
        {
            var result = new List<string>();
            var targetType = this.GetType();
            var fields = targetType.GetFields(BindingFlags.Instance | BindingFlags.Public);

            foreach (var field in fields)
            {
                // Check if the field is a List<GameObject>
                if (field.FieldType.ToString() == "System.Collections.Generic.List`1[UnityEngine.GameObject]")
                {
                    var list = (List<GameObject>)field.GetValue(this);
                    object newValue = "";
                    if (list != null && list.Count > 0)
                    {
                        // Concatenate GameObject names in the list
                        for (var i = 0; i < list.Count; i++)
                        {
                            if (i + 1 == list.Count)
                                newValue += list[i].name;
                            else
                                newValue += list[i].name + "/";
                        }

                        result.Add($"{field.Name},{field.FieldType},{newValue}");
                    }
                    else
                    {
                        result.Add($"{field.Name},{field.FieldType},");
                    }
                }
                else
                {
                    // Get the value of the field and add it to the result list
                    var value = field.GetValue(this);
                    result.Add($"{field.Name},{field.FieldType},{value}");
                }
            }

            return result;
        }
        /// <summary>
        /// Retrieves public variables of the StateScript and returns them in a dictionary.
        /// </summary>
        /// <returns>A dictionary containing variable names and their values.</returns>
        public Dictionary<string, object> GetVariables()
        {
            var variables = new Dictionary<string, object>();
            var type = GetType();
            var fields = type.GetFields();

            foreach (var field in fields)
            {
                var value = field.GetValue(this);
                variables.Add(field.Name, value);
            }

            return variables;
        }
        /// <summary>
        /// Sets the value of a public variable in the StateScript.
        /// </summary>
        /// <param name="variableName">The name of the variable to set.</param>
        /// <param name="newValue">The new value to assign to the variable.</param>
        public void SetVariableValue(string variableName, object newValue)
        {
            var type = GetType();
            var field = type.GetField(variableName);

            if (field != null)
            {
                // Check if the field is a List<GameObject>
                if (field.FieldType.ToString() == "System.Collections.Generic.List`1[UnityEngine.GameObject]")
                {
                    if (newValue.GetType().ToString() == "UnityEngine.GameObject")
                    {
                        // Cast newValue to List<GameObject> and add a new GameObject
                        var list = (List<GameObject>)field.GetValue(this);
                        list.Add((GameObject)newValue);
                        field.SetValue(this, list);
                    }
                    else
                    {
                        // Set the value of the field to the new List<GameObject>
                        field.SetValue(this, (List<GameObject>)newValue);
                    }
                }
                else
                {
                    field.SetValue(this, newValue);
                }
            }
            else
            {
                Debug.LogError($"{variableName} does not exist in the ScriptableObject.");
            }
        }
        
        /// <summary>
        /// Sets the name of the state.
        /// </summary>
        /// <param name="newName">The new name of the state.</param>
        public void SetStateName(string newName)
        {
            _stateName = newName;
        }

        /// <summary>
        /// Gets the name of the state.
        /// </summary>
        /// <returns>The name of the state.</returns>
        public string GetStateName()
        {
            return _stateName;
        }

        /// <summary>
        /// Hides the StateScript from the inspector.
        /// </summary>
        public void HideFlagsInspector()
        {
            hideFlags = HideFlags.HideInInspector;
        }
    }
}
