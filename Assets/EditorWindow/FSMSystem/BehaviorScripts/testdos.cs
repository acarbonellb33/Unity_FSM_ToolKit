using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[Serializable]
public class testdos : BehaviorScript
{
	[Header("Attack")]
	[SerializeField]
	public AttackState attack;

	[Header("Chase")]
	[SerializeField]
	public ChaseState chase;

	[Header("Distance")]
	[SerializeField]
	public DistanceCondition distance;

	[Header("Hearing")]
	[SerializeField]
	public HearingCondition hearing;

	private void Start()
	{
		currentState = FSMStates.Attack;
	}
	void Update()
	{
		switch (currentState)
		{
			case FSMStates.Attack:
				UpdateAttackState();
				break;
			case FSMStates.Chase:
				UpdateChaseState();
				break;
		}
	}
	public void UpdateAttackState()
	{
		attack.Execute();
		if(hearing.Condition())
		{
			ChangeChaseState();
		}
	}
	public void UpdateChaseState()
	{
		chase.Execute();
		if(distance.Condition())
		{
			ChangeAttackState();
		}
	}
	private void ChangeAttackState()
	{
		currentState = FSMStates.Attack;
	}
	private void ChangeChaseState()
	{
		currentState = FSMStates.Chase;
	}
	public void AddObjectToList(GameObject obj)
	{
		//patrol.patrolPoints.Add(obj);
	}
}
