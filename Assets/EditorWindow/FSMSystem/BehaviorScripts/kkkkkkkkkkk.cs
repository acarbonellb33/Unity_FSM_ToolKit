using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class kkkkkkkkkkk : BehaviorScript
{
	[Header("Distance")]
	[SerializeField]
	public DistanceConditionScript distance;

	[Header("Seeing")]
	[SerializeField]
	public SeeingConditionScript seeing;

	[Header("Hearing")]
	[SerializeField]
	public HearingConditionScript hearing;

	[Header("Chase")]
	[SerializeField]
	public ChaseStateScript chase;

	[Header("Search")]
	[SerializeField]
	public SearchStateScript search;

	[Header("Health")]
	[SerializeField]
	public HealthConditionScript health;

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
			case FSMStates.Search:
				UpdateSearchState();
				break;
		}
	}
	public void UpdateChaseState()
	{
		chase.Execute();
		if(health.Condition())
		{
			ChangeSearchState();
		}
	}
	public void UpdateSearchState()
	{
		search.Execute();
		if(distance.Condition())
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
