#if UNITY_EDITOR
namespace FSM.Nodes.States
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;
    using UnityEngine.AI;

    // StateScript is an abstract class that inherits from MonoBehaviour
    // It provides functionalities related to managing states in the FSM
    [Serializable]
    public abstract class StateScript : MonoBehaviour
    {
        // Private variable to store the state name
        private string stateName;

        // Protected variables for player and agent references
        protected GameObject player;
        protected NavMeshAgent agent;

        // Called when the script instance is being loaded
        private void OnEnable()
        {
            // Get the NavMeshAgent component attached to the same GameObject as this script
            agent = GetComponent<NavMeshAgent>();

            // Find the "Player" GameObject in the scene and assign it to the player variable
            GameObject playerObject = GameObject.Find("Player");
            if (playerObject != null)
            {
                player = playerObject;
            }
        }

        // InspectVariables method inspects and returns a list of public variables and their values as strings
        public List<string> InspectVariables()
        {
            List<string> result = new List<string>();
            Type targetType = this.GetType();
            FieldInfo[] fields = targetType.GetFields(BindingFlags.Instance | BindingFlags.Public);

            foreach (FieldInfo field in fields)
            {
                // Check if the field is a List<GameObject>
                if (field.FieldType.ToString() == "System.Collections.Generic.List`1[UnityEngine.GameObject]")
                {
                    List<GameObject> list = (List<GameObject>)field.GetValue(this);
                    object newValue = "";
                    if (list != null && list.Count > 0)
                    {
                        // Concatenate GameObject names in the list
                        for (int i = 0; i < list.Count; i++)
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
                    object value = field.GetValue(this);
                    result.Add($"{field.Name},{field.FieldType},{value}");
                }
            }

            return result;
        }

        // GetVariables method returns a dictionary of all public variables and their values
        public Dictionary<string, object> GetVariables()
        {
            Dictionary<string, object> variables = new Dictionary<string, object>();
            Type type = GetType();
            FieldInfo[] fields = type.GetFields();

            foreach (FieldInfo field in fields)
            {
                // Get the value of the field and add it to the dictionary
                object value = field.GetValue(this);
                variables.Add(field.Name, value);
            }

            return variables;
        }

        // SetVariableValue method sets the value of a specified variable
        public void SetVariableValue(string variableName, object newValue)
        {
            System.Type type = GetType();
            System.Reflection.FieldInfo field = type.GetField(variableName);

            if (field != null)
            {
                // Check if the field is a List<GameObject>
                if (field.FieldType.ToString() == "System.Collections.Generic.List`1[UnityEngine.GameObject]")
                {
                    if (newValue.GetType().ToString() == "UnityEngine.GameObject")
                    {
                        // Cast newValue to List<GameObject> and add a new GameObject
                        List<GameObject> list = (List<GameObject>)field.GetValue(this);
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
                    // Set the value of the field to the new value
                    field.SetValue(this, newValue);
                }
            }
            else
            {
                // Log an error if the variable does not exist
                Debug.LogError($"{variableName} does not exist in the ScriptableObject.");
            }
        }

        // SetStateName method sets the state name
        public void SetStateName(string name)
        {
            stateName = name;
        }

        // GetStateName method returns the state name
        public string GetStateName()
        {
            return stateName;
        }
    }
}
#endif