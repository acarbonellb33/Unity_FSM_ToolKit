using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class attackTest : BehaviorScript
{
	[Header("Chase")]
	[SerializeField]
	public ChaseStateScript chase;

	[Header("Attack")]
	[SerializeField]
	public AttackStateScript attack;

	[Header("Hearing 0")]
	[SerializeField]
	public HearingConditionScript hearing0;

	[Header("Seeing")]
	[SerializeField]
	public SeeingConditionScript seeing;

	[Header("Search")]
	[SerializeField]
	public SearchStateScript search;

	[Header("Hearing 1")]
	[SerializeField]
	public HearingConditionScript hearing1;

	[Header("Distance 0")]
	[SerializeField]
	public DistanceConditionScript distance0;

	[Header("Distance 1")]
	[SerializeField]
	public DistanceConditionScript distance1;

	[Header("Patrol")]
	[SerializeField]
	public PatrolStateScript patrol;

	[Header("Distance 2")]
	[SerializeField]
	public DistanceConditionScript distance2;

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
		if(distance2.Condition())
		{
			ChangeSearchState();
		}
	}
	public void UpdateAttackState()
	{
		attack.Execute();
		if(distance0.Condition() && distance1.Condition())
		{
			ChangeChaseState();
		}
		else if(distance0.Condition() && !distance1.Condition())
		{
			ChangeSearchState();
		}
	}
	public void UpdateSearchState()
	{
		search.Execute();
		if(hearing1.Condition())
		{
			ChangeAttackState();
		}
	}
	public void UpdatePatrolState()
	{
		patrol.Execute();
		if(distance0.Condition() && distance1.Condition())
		{
			ChangeChaseState();
		}
		else if(distance0.Condition() && !distance1.Condition())
		{
			ChangeSearchState();
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
