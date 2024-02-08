using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class pppp : BehaviorScript
{
	[Header("Distance")]
	[SerializeField]
	public DistanceConditionScript distance;

	[Header("Attack")]
	[SerializeField]
	public AttackStateScript attack;

	[Header("Hearing")]
	[SerializeField]
	public HearingConditionScript hearing;

	[Header("Chase")]
	[SerializeField]
	public ChaseStateScript chase;

	private void Start()
	{
		currentState = FSMStates.Chase;
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
}
