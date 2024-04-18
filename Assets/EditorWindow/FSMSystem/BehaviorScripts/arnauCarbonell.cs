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

	[Header("Search")]
	[SerializeField]
	public SearchStateScript search;

	private void Start()
	{
		currentState = FSMStates.Search;
		GetComponent<Animator>().SetFloat("Speed", GetComponent<NavMeshAgent>().speed);
	}
	void Update()
	{
		switch (currentState)
		{
			case FSMStates.Chase:
				UpdateChaseState();
				break;
			case FSMStates.Search:
				UpdateSearchState();
				break;
		}
	}
	public void UpdateChaseState()
	{
		chase.Execute();
		if(hearing.Condition())
		{
			ChangeSearchState();
		}
	}
	public void UpdateSearchState()
	{
		search.Execute();
		if(seeing.Condition())
		{
			ChangeChaseState();
		}
	}
	private void ChangeChaseState()
	{
		currentState = FSMStates.Chase;
	}
	private void ChangeSearchState()
	{
		currentState = FSMStates.Search;
	}
	private void OnFootstep() {}
}