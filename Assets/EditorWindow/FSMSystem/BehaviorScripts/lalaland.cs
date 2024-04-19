using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class lalaland : BehaviorScript
{
	[Header("Hearing")]
	[SerializeField]
	public HearingConditionScript hearing;

	[Header("Attack")]
	[SerializeField]
	public AttackStateScript attack;

	[Header("Search")]
	[SerializeField]
	public SearchStateScript search;

	[Header("Chase")]
	[SerializeField]
	public ChaseStateScript chase;

	[Header("Seeing 0")]
	[SerializeField]
	public SeeingConditionScript seeing0;

	[Header("Seeing 1")]
	[SerializeField]
	public SeeingConditionScript seeing1;

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
			case FSMStates.Search:
				UpdateSearchState();
				break;
			case FSMStates.Chase:
				UpdateChaseState();
				break;
		}
	}
	public void UpdateAttackState()
	{
		attack.Execute();
		if(hearing.Condition() )
		{
			ChangeSearchState();
		}
		else if(!hearing.Condition() && seeing0.Condition() )
		{
			ChangeChaseState();
		}
	}
	public void UpdateSearchState()
	{
		search.Execute();
		if(seeing0.Condition() )
		{
			ChangeChaseState();
		}
	}
	public void UpdateChaseState()
	{
		chase.Execute();
		if(seeing1.Condition() )
		{
			ChangeAttackState();
		}
	}
	private void ChangeAttackState()
	{
		currentState = FSMStates.Attack;
	}
	private void ChangeSearchState()
	{
		currentState = FSMStates.Search;
	}
	private void ChangeChaseState()
	{
		currentState = FSMStates.Chase;
	}
	private void OnFootstep() {}
}
