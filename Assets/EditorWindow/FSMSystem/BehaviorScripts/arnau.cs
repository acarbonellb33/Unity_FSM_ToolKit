using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[Serializable]
public class arnau : BehaviorScript
{
	[Header("Attack")]
	[SerializeField]
	public AttackState attack;

	[Header("Hearing")]
	[SerializeField]
	public HearingCondition hearing;

	[Header("Patrol")]
	[SerializeField]
	public PatrolState patrol;

	[Header("Distance")]
	[SerializeField]
	public DistanceCondition distance;

	void Update()
	{
		switch (currentState)
		{
			case FSMStates.Attack:
				UpdateAttackState();
				break;
			case FSMStates.Patrol:
				UpdatePatrolState();
				break;
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
	public void UpdatePatrolState()
	{
		patrol.Execute();
		if(distance.Condition())
		{
			ChangePatrolState();
		}
	}
	private void ChangeAttackState()
	{
		currentState = FSMStates.Attack;
	}
	private void ChangePatrolState()
	{
		currentState = FSMStates.Patrol;
	}
}
