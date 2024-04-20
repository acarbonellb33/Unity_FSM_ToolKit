using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class lalaland : BehaviorScript
{
	[Header("Chase")]
	[SerializeField]
	public ChaseStateScript chase;

	[Header("Hearing 0")]
	[SerializeField]
	public HearingConditionScript hearing0;

	[Header("Attack")]
	[SerializeField]
	public AttackStateScript attack;

	[Header("Seeing")]
	[SerializeField]
	public SeeingConditionScript seeing;


	private void Start()
	{
		currentState = FSMStates.Chase;
		GetComponent<Animator>().SetFloat("Speed", GetComponent<NavMeshAgent>().speed);
	}
	void Update()
	{
		switch (currentState)
		{
			case FSMStates.Chase:
				UpdateChaseState();
				break;
			case FSMStates.Attack:
				UpdateAttackState();
				break;
		}
	}
	public void UpdateChaseState()
	{
		chase.Execute();
		if(seeing.Condition() && hearing0.Condition() )
		{
			ChangeChaseState();
		}
	}
	public void UpdateAttackState()
	{
		attack.Execute();
		if(hearing0.Condition() )
		{
			ChangeChaseState();
		}
	}
	private void ChangeChaseState()
	{
		currentState = FSMStates.Chase;
	}
	private void ChangeAttackState()
	{
		currentState = FSMStates.Attack;
	}
	private void OnFootstep() {}
}
