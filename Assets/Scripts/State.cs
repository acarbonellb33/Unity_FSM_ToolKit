using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class State : ScriptableObject
{
    private string stateName;
    //public abstract void Execute();
    public abstract void Execute();

    public List<string> InspectVariables()
    {
        List<string> result = new List<string>();
        Type targetType = this.GetType();
        FieldInfo[] fields = targetType.GetFields(BindingFlags.Instance | BindingFlags.Public);

        foreach (FieldInfo field in fields)
        {
            object value = field.GetValue(this);
            result.Add($"{field.Name},{field.FieldType},{value}");
        }
        return result;
    }

    public List<object> GetVariablesValues()
    {
        List<object> values = new List<object>();
        Type type = GetType();
        FieldInfo[] fields = type.GetFields();

        foreach (FieldInfo field in fields)
        {
            object value = field.GetValue(this);
            values.Add(value);
        }
        return values;
    }
    
    public Dictionary<string, object> GetVariables()
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

    public void SetVariableValue(string variableName, object newValue)
    {
        // Use reflection to set the value of the variable with the given name
        System.Type type = GetType();
        System.Reflection.FieldInfo field = type.GetField(variableName);

        if (field != null)
        {
            field.SetValue(this, newValue);
        }
        else
        {
            Debug.LogError($"{variableName} does not exist in the ScriptableObject.");
        }
    }

    protected void SetStateName(string name)
    {
        stateName = name;
    }
    
    public string GetStateName()
    {
        return stateName;
    }
}
