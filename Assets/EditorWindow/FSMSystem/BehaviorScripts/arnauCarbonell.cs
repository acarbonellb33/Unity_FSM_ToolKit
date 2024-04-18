using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class arnauCarbonell : BehaviorScript
{
	[Header("Chase")]
	[SerializeField]
	public ChaseStateScript chase;

	[Header("Hearing")]
	[SerializeField]
	public HearingConditionScript hearing;
	

	[Header("Seeing")]
	[SerializeField]
	public SeeingConditionScript seeing;

	[Header("Attack")]
	[SerializeField]
	public AttackStateScript attack;

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
		if(hearing.Condition())
		{
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
