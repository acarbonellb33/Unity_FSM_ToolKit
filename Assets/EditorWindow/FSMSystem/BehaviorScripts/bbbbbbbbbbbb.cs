using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class bbbbbbbbbbbb : BehaviorScript
{
	[Header("Attack")]
	[SerializeField]
	public AttackStateScript attack;

	[Header("Hearing 0")]
	[SerializeField]
	public HearingConditionScript hearing0;

	[Header("Chase")]
	[SerializeField]
	public ChaseStateScript chase;

	[Header("Seeing")]
	[SerializeField]
	public SeeingConditionScript seeing;

	[Header("Hearing")]
	[SerializeField]
	public HearingConditionScript hearing;

	private void Start()
	{
		currentState = FSMStates.Attack;
		GetComponent<Animator>().SetFloat("Speed", GetComponent<NavMeshAgent>().speed);
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
		if(hearing.Condition() && hearing.Condition())
		{
			ChangeAttackState();
		}
	}
	public void UpdateChaseState()
	{
		chase.Execute();
		if(seeing.Condition() && seeing.Condition() )
		{
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
	private void OnFootstep() {}
}
