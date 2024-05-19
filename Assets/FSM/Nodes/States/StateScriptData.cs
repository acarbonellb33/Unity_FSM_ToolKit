namespace FSM.Nodes.States
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;
    /// <summary>
    /// Data class for storing state script variables and metadata.
    /// </summary>
    public class StateScriptData
    {
        private string _stateName;
        /// <summary>
        /// Inspects public variables of the StateScriptData and returns their values.
        /// </summary>
        /// <returns>A list of strings representing the inspected variables.</returns>
        public List<string> InspectVariables()
        {
            var result = new List<string>();
            var targetType = this.GetType();
            var fields = targetType.GetFields(BindingFlags.Instance | BindingFlags.Public);

            foreach (var field in fields)
            {
                if (field.FieldType.ToString() == "System.Collections.Generic.List`1[System.String]")
                {
                    var list = (List<string>)field.GetValue(this);
                    var newValue = "";
                    if (list != null && list.Count > 0)
                    {
                        // Concatenate GameObject names in the list
                        for (var i = 0; i < list.Count; i++)
                        {
                            if (i + 1 == list.Count)
                                newValue += list[i];
                            else
                                newValue += list[i] + "/";
                        }

                        if (String.IsNullOrEmpty(newValue))
                            result.Add($"{field.Name},{field.FieldType},Null");
                        else
                            result.Add($"{field.Name},{field.FieldType},{newValue}");
                    }
                    else
                    {
                        result.Add($"{field.Name},{field.FieldType},");
                    }
                }
                else
                {
                    var value = field.GetValue(this);
                    result.Add($"{field.Name},{field.FieldType},{value}");
                }
            }

            return result;
        }
        /// <summary>
        /// Retrieves public variables of the StateScriptData and returns them in a dictionary.
        /// </summary>
        /// <returns>A dictionary containing variable names and their values.</returns>
        public Dictionary<string, object> GetVariables()
        {
            var variables = new Dictionary<string, object>();
            var type = GetType();
            var fields = type.GetFields();

            foreach (var field in fields)
            {
                // Get the value of the field and add it to the dictionary
                var value = field.GetValue(this);
                variables.Add(field.Name, value);
            }
            return variables;
        }
        /// <summary>
        /// Sets the value of a public variable in the StateScriptData.
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
                if (field.FieldType.ToString() == "System.Collections.Generic.List`1[System.String]")
                {
                    if (newValue.GetType().ToString() == "System.String")
                    {
                        // Cast newValue to List<GameObject> and add a new GameObject
                        var list = (List<string>)field.GetValue(this);
                        list.Add((string)newValue);
                        field.SetValue(this, list);
                    }
                    else
                    {
                        field.SetValue(this, (List<string>)newValue);
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
        /// Removes a variable from the StateScriptData.
        /// </summary>
        /// <param name="variableName">The name of the variable to remove.</param>
        /// <param name="pastValue">The value of the variable to remove.</param>
        public void RemoveVariable(string variableName, object pastValue)
        {
            var type = GetType();
            var field = type.GetField(variableName);

            if (field != null)
            {
                // Check if the field is a List<string>
                if (field.FieldType == typeof(List<string>))
                {
                    var list = (List<string>)field.GetValue(this);
                    list.Remove((string)pastValue);
                    field.SetValue(this, list);
                }
                else
                {
                    field.SetValue(this, null);
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
        /// <param name="name">The new name of the state.</param>
        protected void SetStateName(string name)
        {
            _stateName = name;
        }

        /// <summary>
        /// Gets the name of the state.
        /// </summary>
        /// <returns>The name of the state.</returns>
        public string GetStateName()
        {
            return _stateName;
        }
    }
}