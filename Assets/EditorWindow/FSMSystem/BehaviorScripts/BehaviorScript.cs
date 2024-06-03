namespace EditorWindow.FSMSystem.BehaviorScripts
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using FSM.Enumerations;
    using FSM.Nodes.States;
    using FSM.Nodes.States.StateScripts;
    using UnityEngine;
    /// <summary>
    /// Represents an abstract behavior script for managing state options.
    /// </summary>
    public abstract class BehaviorScript : MonoBehaviour
    {
        public List<StateScript> options;
        public int selectedOptionIndex = 0;
        protected FsmStates CurrentState = FsmStates.Idle;
        
        /// <summary>
        /// Retrieves variables and their values from the behavior script.
        /// </summary>
        /// <returns>A dictionary containing variable names and their values.</returns>
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
        
        /// <summary>
        /// Sets the value of a specified variable in the behavior script.
        /// </summary>
        /// <param name="variableName">The name of the variable to set.</param>
        /// <param name="newValue">The new value to assign to the variable.</param>
        public virtual void SetVariableValue(string variableName, object newValue)
        {
            var type = GetType();
            variableName = variableName.Replace(" ", "");
            var field = type.GetField(variableName);
            if (field != null)
            {
                var lastParenIndex = newValue.ToString().LastIndexOf('.');
                var nameType = newValue.ToString().Substring(lastParenIndex + 1);
                if (options == null) options = new List<StateScript>();
                switch (nameType)
                {
                    case "PatrolData":
                        PatrolStateScript patrol = gameObject.AddComponent<PatrolStateScript>();
                        patrol.HideFlagsInspector();
                        foreach (var variable in ((StateScriptData)newValue).GetVariables())
                        {
                            patrol.SetVariableValue(variable.Key, variable.Value);
                        }
                        field.SetValue(this, patrol);
                        options.Add(patrol);
                        break;
                    case "IdleData":
                        IdleStateScript idle = gameObject.AddComponent<IdleStateScript>();
                        idle.HideFlagsInspector();
                        foreach (var variable in ((StateScriptData)newValue).GetVariables())
                        {
                            idle.SetVariableValue(variable.Key, variable.Value);
                        }
                        field.SetValue(this, idle);
                        options.Add(idle);
                        break;
                    case "ChaseData":
                        ChaseStateScript chase = gameObject.AddComponent<ChaseStateScript>();
                        chase.HideFlagsInspector();
                        foreach (var variable in ((StateScriptData)newValue).GetVariables())
                        {
                            chase.SetVariableValue(variable.Key, variable.Value);
                        }
                        field.SetValue(this, chase);
                        options.Add(chase);
                        break;
                    case "FleeData":
                        FleeStateScript flee = gameObject.AddComponent<FleeStateScript>();
                        flee.HideFlagsInspector();
                        foreach (var variable in ((StateScriptData)newValue).GetVariables())
                        {
                            flee.SetVariableValue(variable.Key, variable.Value);
                        }
                        field.SetValue(this, flee);
                        options.Add(flee);
                        break;
                    case "AttackData":
                        AttackStateScript attack = gameObject.AddComponent<AttackStateScript>();
                        attack.HideFlagsInspector();
                        foreach (var variable in ((StateScriptData)newValue).GetVariables())
                        {
                            attack.SetVariableValue(variable.Key, variable.Value);
                        }
                        field.SetValue(this, attack);
                        options.Add(attack);
                        break;
                    case "SearchData":
                        SearchStateScript search = gameObject.AddComponent<SearchStateScript>();
                        search.HideFlagsInspector();
                        foreach (var variable in ((StateScriptData)newValue).GetVariables())
                        {
                            search.SetVariableValue(variable.Key, variable.Value);
                        }
                        field.SetValue(this, search);
                        options.Add(search);
                        break;
                    case "HearingData":
                        HearingConditionScript hearing = gameObject.AddComponent<HearingConditionScript>();
                        hearing.HideFlagsInspector();
                        foreach (var variable in ((StateScriptData)newValue).GetVariables())
                        {
                            hearing.SetVariableValue(variable.Key, variable.Value);
                        }
                        field.SetValue(this, hearing);
                        options.Add(hearing);
                        break;
                    case "DistanceData":
                        DistanceConditionScript distance = gameObject.AddComponent<DistanceConditionScript>();
                        distance.HideFlagsInspector();
                        foreach (var variable in ((StateScriptData)newValue).GetVariables())
                        {
                            distance.SetVariableValue(variable.Key, variable.Value);
                        }
                        field.SetValue(this, distance);
                        options.Add(distance);
                        break;
                    case "SeeingData":
                        SeeingConditionScript seeing = gameObject.AddComponent<SeeingConditionScript>();
                        seeing.HideFlagsInspector();
                        foreach (var variable in ((StateScriptData)newValue).GetVariables())
                        {
                            seeing.SetVariableValue(variable.Key, variable.Value);
                        }
                        field.SetValue(this, seeing);
                        options.Add(seeing);
                        break;
                    case "HealthData":
                        HealthConditionScript health = gameObject.AddComponent<HealthConditionScript>();
                        health.HideFlagsInspector();
                        foreach (var variable in ((StateScriptData)newValue).GetVariables())
                        {
                            health.SetVariableValue(variable.Key, variable.Value);
                        }
                        field.SetValue(this, health);
                        options.Add(health);
                        break;
                    case "NextStateData":
                        NextStateConditionScript next = gameObject.AddComponent<NextStateConditionScript>();
                        next.HideFlagsInspector();
                        foreach (var variable in ((StateScriptData)newValue).GetVariables())
                        {
                            next.SetVariableValue(variable.Key, variable.Value);
                        }
                        field.SetValue(this, next);
                        options.Add(next);
                        break;
                    case "CustomData":
                        CustomStateScript custom = gameObject.AddComponent<CustomStateScript>();
                        custom.HideFlagsInspector();
                        foreach (var variable in ((StateScriptData)newValue).GetVariables())
                        {
                            custom.SetVariableValue(variable.Key, variable.Value);
                        }
                        field.SetValue(this, custom);
                        options.Add(custom);
                        break;
                    case "CustomConditionData":
                        CustomConditionScript customCondition = gameObject.AddComponent<CustomConditionScript>();
                        customCondition.HideFlagsInspector();
                        foreach (var variable in ((StateScriptData)newValue).GetVariables())
                        {
                            customCondition.SetVariableValue(variable.Key, variable.Value);
                        }
                        field.SetValue(this, customCondition);
                        options.Add(customCondition);
                        break;
                    case "VariableData":
                        VariableConditionScript variableCondition = gameObject.AddComponent<VariableConditionScript>();
                        variableCondition.HideFlagsInspector();
                        foreach (var variable in ((StateScriptData)newValue).GetVariables())
                        {
                            variableCondition.SetVariableValue(variable.Key, variable.Value);
                        }
                        field.SetValue(this, variableCondition);
                        options.Add(variableCondition);
                        break;
                }
            }
            else
            {
                Debug.LogError($"{variableName} does not exist in the ScriptableObject.");
            }
        }
    }
}