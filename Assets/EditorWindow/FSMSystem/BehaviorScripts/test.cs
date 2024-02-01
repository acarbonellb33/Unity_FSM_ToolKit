using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[Serializable]
public class test : BehaviorScript
{
	[Header("Distance")]
	[SerializeField]
	public DistanceCondition distance;

	[Header("Hearing")]
	[SerializeField]
	public HearingCondition hearing;

	[Header("Patrol")]
	[SerializeField]
	public PatrolState patrol;

	[Header("Attack")]
	[SerializeField]
	public AttackState attack;

	void Update()
	{
		switch (currentState)
		{
			case FSMStates.Patrol:
				UpdatePatrolState();
				break;
			case FSMStates.Attack:
				UpdateAttackState();
				break;
		}
	}
	public void UpdatePatrolState()
	{
		patrol.Execute();
		if(distance.Condition())
		{
			ChangePatrolState();
		}
	}
	public void UpdateAttackState()
	{
		attack.Execute();
		if(hearing.Condition())
		{
			ChangeAttackState();
		}
	}
	private void ChangePatrolState()
	{
		currentState = FSMStates.Patrol;
	}
	private void ChangeAttackState()
	{
		currentState = FSMStates.Attack;
	}
}
