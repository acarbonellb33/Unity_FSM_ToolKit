using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public abstract class StateScript : MonoBehaviour
{
    private string stateName;

    protected GameObject player;
    protected NavMeshAgent agent = null;

    private void OnEnable()
    {
        GameObject enemyObject = GameObject.Find("Enemy");
        if (enemyObject != null)
        {
            agent = enemyObject.GetComponent<NavMeshAgent>();
            
        }
        GameObject playerObject = GameObject.Find("Player");
        if (playerObject != null)
        {
            player = playerObject;
        }
    }

    public abstract void Execute();

    public List<string> InspectVariables()
    {
        List<string> result = new List<string>();
        Type targetType = this.GetType();
        FieldInfo[] fields = targetType.GetFields(BindingFlags.Instance | BindingFlags.Public);

        foreach (FieldInfo field in fields)
        {
            if (field.FieldType.ToString() == "System.Collections.Generic.List`1[UnityEngine.GameObject]")
            {
                List<GameObject> list = (List<GameObject>)field.GetValue(this);;
                object newValue = "";
                for (int i = 0; i < list.Count; i++)
                {
                    if(i+1 == list.Count)
                        newValue += list[i].name;
                    else
                        newValue += list[i].name+"/";
                }
                result.Add($"{field.Name},{field.FieldType},{newValue}");  
            }
            else
            {
              object value = field.GetValue(this);
              result.Add($"{field.Name},{field.FieldType},{value}");  
            }
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
            if (field.FieldType.ToString() == "System.Collections.Generic.List`1[UnityEngine.GameObject]")
            {
                if(newValue.GetType().ToString() == "UnityEngine.GameObject")
                {
                    List<GameObject> list = (List<GameObject>)field.GetValue(this);
                    list.Add((GameObject)newValue);
                    field.SetValue(this, list);
                }
                else
                {
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

    protected void SetStateName(string name)
    {
        stateName = name;
    }
    
    public string GetStateName()
    {
        return stateName;
    }
    
    public void SetAgent(NavMeshAgent agent)
    {
        this.agent = agent;
    }
}
