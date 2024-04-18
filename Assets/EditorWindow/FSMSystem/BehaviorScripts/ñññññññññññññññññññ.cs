using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class ñññññññññññññññññññ : BehaviorScript
{
	[Header("Attack")]
	[SerializeField]
	public AttackStateScript attack;

	[Header("Hearing")]
	[SerializeField]
	public HearingConditionScript hearing;

	[Header("Hearing 0")]
	[SerializeField]
	public HearingConditionScript hearing0;

	[Header("Hearing 1")]
	[SerializeField]
	public HearingConditionScript hearing1;

	[Header("Search")]
	[SerializeField]
	public SearchStateScript search;

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
		}
	}
	public void UpdateAttackState()
	{
		attack.Execute();
		if(hearing.Condition() && hearing0.Condition() )
		{
			ChangeSearchState();
		}
	}
	public void UpdateSearchState()
	{
		search.Execute();
	
	}
	private void ChangeAttackState()
	{
		currentState = FSMStates.Attack;
	}
	private void ChangeSearchState()
	{
		currentState = FSMStates.Search;
	}
	private void OnFootstep() {}
}
