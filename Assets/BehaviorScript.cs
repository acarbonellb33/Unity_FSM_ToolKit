using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

public abstract class BehaviorScript : MonoBehaviour
{
    public List<State> options;
    public int selectedOptionIndex = 0;
    protected FSMStates currentState = FSMStates.Idle;
    
    public virtual Dictionary<string, object> GetVariables()
    {
        Dictionary<string, object> variables = new Dictionary<string, object>();
        Type type = GetType();
        FieldInfo[] fields = type.GetFields();

        foreach (FieldInfo field in fields)
        { 
            object value = field.GetValue(this);
            variables.Add(field.Name, value);
        }
        return variables;
    }

    public virtual void SetVariableValue(string variableName, object newValue)
    {
        System.Type type = GetType();
        System.Reflection.FieldInfo field = type.GetField(variableName);

        if (field != null)
        {
            field.SetValue(this, newValue);
            if(options == null)
                options = new List<State>();
            options.Add((State)newValue);
        }
        else
        {
            Debug.LogError($"{variableName} does not exist in the ScriptableObject.");
        }
    }
}
