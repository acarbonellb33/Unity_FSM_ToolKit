using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class estelita : BehaviorScript
{
	[Header("Attack")]
	[SerializeField]
	public AttackStateScript attack;

	[Header("Hearing 0")]
	[SerializeField]
	public HearingConditionScript hearing0;

	[Header("Search")]
	[SerializeField]
	public SearchStateScript search;

	[Header("Seeing 0")]
	[SerializeField]
	public SeeingConditionScript seeing0;

	[Header("Hearing 2")]
	[SerializeField]
	public HearingConditionScript hearing2;

	[Header("Seeing")]
	[SerializeField]
	public SeeingConditionScript seeing;

	[Header("Seeing 2")]
	[SerializeField]
	public SeeingConditionScript seeing2;

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
		if(hearing2.Condition() && seeing0.Condition() )
		{
			ChangeSearchState();
		}
	}
	public void UpdateSearchState()
	{
		search.Execute();
		if(seeing2.Condition() && seeing.Condition() && hearing0.Condition() )
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
	private void OnFootstep() {}
}
