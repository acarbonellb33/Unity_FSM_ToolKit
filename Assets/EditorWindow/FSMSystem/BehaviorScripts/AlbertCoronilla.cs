using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[Serializable]
public class AlbertCoronilla : MonoBehaviour
{
	[Header("Patrol")]
	[SerializeField]
	public PatrolState patrol;

	[Header("Attack")]
	[SerializeField]
	public AttackState attack;

	[Header("Hearing")]
	[SerializeField]
	public HearingCondition hearing;

	[Header("Distance")]
	[SerializeField]
	public DistanceCondition distance;

	public void Patrol()
	{
		patrol.Execute();
		if(distance.Condition())
		{
			attack.Execute();
		}
	}
	public void Attack()
	{
		attack.Execute();
		if(hearing.Condition())
		{
			patrol.Execute();
		}
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
}
