using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

public abstract class BehaviorScript : MonoBehaviour
{
    public List<StateScript> options;
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
        variableName = variableName.Replace(" ","");
        System.Reflection.FieldInfo field = type.GetField(variableName);
        if (field != null)
        {
            if(options == null)
                options = new List<StateScript>();
            switch (((StateScript)newValue).GetType().ToString())
            {
                case "PatrolStateScript":
                    PatrolStateScript patrol = gameObject.AddComponent<PatrolStateScript>();
                    foreach (var variable in ((StateScript)newValue).GetVariables())
                    {
                        patrol.SetVariableValue(variable.Key, variable.Value);
                    }
                    field.SetValue(this, patrol);
                    options.Add(patrol);
                    break;
                case "ChaseStateScript":
                    ChaseStateScript chase = gameObject.AddComponent<ChaseStateScript>();
                    foreach (var variable in ((StateScript)newValue).GetVariables())
                    {
                        chase.SetVariableValue(variable.Key, variable.Value);
                    }
                    field.SetValue(this, chase);
                    options.Add(chase);
                    break;
                case "AttackStateScript":
                    AttackStateScript attack = gameObject.AddComponent<AttackStateScript>();
                    foreach (var variable in ((StateScript)newValue).GetVariables())
                    {
                        attack.SetVariableValue(variable.Key, variable.Value);
                    }
                    field.SetValue(this, attack);
                    options.Add(attack);
                    break;
                case "SearchStateScript":
                    SearchStateScript search = gameObject.AddComponent<SearchStateScript>();
                    foreach (var variable in ((StateScript)newValue).GetVariables())
                    {
                        search.SetVariableValue(variable.Key, variable.Value);
                    }
                    field.SetValue(this, search);
                    options.Add(search);
                    break;
                case "HearingConditionScript":
                    HearingConditionScript hearing = gameObject.AddComponent<HearingConditionScript>();
                    foreach (var variable in ((StateScript)newValue).GetVariables())
                    {
                        hearing.SetVariableValue(variable.Key, variable.Value);
                    }
                    field.SetValue(this, hearing);
                    options.Add(hearing);
                    break;
                case "SeeingConditionScript":
                    SeeingConditionScript seeing = gameObject.AddComponent<SeeingConditionScript>();
                    foreach (var variable in ((StateScript)newValue).GetVariables())
                    {
                        seeing.SetVariableValue(variable.Key, variable.Value);
                    }
                    field.SetValue(this, seeing);
                    options.Add(seeing);
                    break;
            }
        }
        else
        {
            Debug.LogError($"{variableName} does not exist in the ScriptableObject.");
        }
    }
}
