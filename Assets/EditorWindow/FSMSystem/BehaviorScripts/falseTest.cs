using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class falseTest : BehaviorScript
{
	[Header("Chase")]
	[SerializeField]
	public ChaseStateScript chase;

	[Header("Hearing 0")]
	[SerializeField]
	public HearingConditionScript hearing0;

	[Header("Hearing 1")]
	[SerializeField]
	public HearingConditionScript hearing1;

	[Header("Hearing 2")]
	[SerializeField]
	public HearingConditionScript hearing2;

	[Header("Search")]
	[SerializeField]
	public SearchStateScript search;

	[Header("Seeing 0")]
	[SerializeField]
	public SeeingConditionScript seeing0;

	[Header("Seeing 1")]
	[SerializeField]
	public SeeingConditionScript seeing1;

	[Header("Patrol")]
	[SerializeField]
	public PatrolStateScript patrol;

	[Header("Seeing 2")]
	[SerializeField]
	public SeeingConditionScript seeing2;

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
			case FSMStates.Patrol:
				UpdatePatrolState();
				break;
		}
	}
	public void UpdateChaseState()
	{
		chase.Execute();
		if(hearing0.Condition())
		{
			ChangePatrolState();
		}
		else if(!hearing0.Condition() && hearing1.Condition() && seeing1.Condition())
		{
			ChangePatrolState();
		}
		else if(!hearing0.Condition() && !hearing1.Condition() && hearing2.Condition())
		{
			ChangePatrolState();
		}
		else if(!hearing0.Condition() && !hearing1.Condition() && !hearing2.Condition())
		{
			ChangeSearchState();
		}
	}
	public void UpdateSearchState()
	{
		search.Execute();
		if(seeing0.Condition())
		{
			ChangeChaseState();
		}
	}
	public void UpdatePatrolState()
	{
		patrol.Execute();
		if(seeing2.Condition())
		{
			ChangeSearchState();
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
	private void ChangePatrolState()
	{
		currentState = FSMStates.Patrol;
	}
	public GameObject AddObjectToList()
	{
		GameObject newGameObject = new GameObject("Patrol Point " + patrol.patrolPoints.Count);
		patrol.patrolPoints.Add(newGameObject);
		return newGameObject;
	}
	public void RemoveObjectFromList(GameObject patrolPoint)
	{
		patrol.RemovePatrolPoint(patrolPoint);
		if(GameObject.Find(patrolPoint.name) != null)
		{
			DestroyImmediate(patrolPoint);
		}
	}
	private void OnFootstep() {}
}
