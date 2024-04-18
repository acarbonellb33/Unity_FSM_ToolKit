using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class uuuuuuuuuuu : BehaviorScript
{
	[Header("Search")]
	[SerializeField]
	public SearchStateScript search;

	[Header("Hearing")]
	[SerializeField]
	public HearingConditionScript hearing;

	[Header("Attack")]
	[SerializeField]
	public AttackStateScript attack;

	[Header("Hearing 0")]
	[SerializeField]
	public HearingConditionScript hearing0;

	[Header("Seeing")]
	[SerializeField]
	public SeeingConditionScript seeing;

	private void Start()
	{
		currentState = FSMStates.Search;
		GetComponent<Animator>().SetFloat("Speed", GetComponent<NavMeshAgent>().speed);
	}
	void Update()
	{
		switch (currentState)
		{
			case FSMStates.Search:
				UpdateSearchState();
				break;
			case FSMStates.Attack:
				UpdateAttackState();
				break;
		}
	}
	public void UpdateSearchState()
	{
		search.Execute();
		if(hearing.Condition() && hearing.Condition() )
		{
			ChangeSearchState();
		}
	}
	public void UpdateAttackState()
	{
		attack.Execute();
		if(seeing.Condition() && seeing.Condition() )
		{
		}
	}
	private void ChangeSearchState()
	{
		currentState = FSMStates.Search;
	}
	private void ChangeAttackState()
	{
		currentState = FSMStates.Attack;
	}
	private void OnFootstep() {}
}
