using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class attackTest : BehaviorScript
{
	[Header("Seeing 0")]
	[SerializeField]
	public SeeingConditionScript seeing0;

	[Header("Attack")]
	[SerializeField]
	public AttackStateScript attack;

	[Header("Hearing 0")]
	[SerializeField]
	public HearingConditionScript hearing0;

	[Header("Hearing 1")]
	[SerializeField]
	public HearingConditionScript hearing1;

	[Header("Chase")]
	[SerializeField]
	public ChaseStateScript chase;

	private void Start()
	{
		currentState = FSMStates.Chase;
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
		if(hearing1.Condition() )
		{
			ChangeChaseState();
		}
	}
	public void UpdateChaseState()
	{
		chase.Execute();
		if(seeing0.Condition() && hearing0.Condition() )
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
	private void OnFootstep() {}
}
