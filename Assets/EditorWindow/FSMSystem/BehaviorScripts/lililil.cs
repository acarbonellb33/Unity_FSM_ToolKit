using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class lililil : BehaviorScript
{
	[Header("Chase")]
	[SerializeField]
	public ChaseStateScript chase;

	[Header("Seeing")]
	[SerializeField]
	public SeeingConditionScript seeing;

	[Header("Hearing")]
	[SerializeField]
	public HearingConditionScript hearing;

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
		if(seeing.Condition())
		{

		}
	}
	public void UpdateAttackState()
	{
		attack.Execute();

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
