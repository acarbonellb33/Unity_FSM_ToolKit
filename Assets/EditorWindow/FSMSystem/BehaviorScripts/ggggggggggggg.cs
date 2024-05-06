using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class ggggggggggggg : BehaviorScript
{
	[Header("Seeing")]
	[SerializeField]
	public SeeingConditionScript seeing;

	[Header("Chase")]
	[SerializeField]
	public ChaseStateScript chase;

	[Header("Search")]
	[SerializeField]
	public SearchStateScript search;

	[Header("Hearing")]
	[SerializeField]
	public HearingConditionScript hearing;

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
		GetComponent<Animator>().SetFloat("MotionSpeed", 10);
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
