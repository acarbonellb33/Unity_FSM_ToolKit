using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class EnemyState : ScriptableObject
{
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
}
