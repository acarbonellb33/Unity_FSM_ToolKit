using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class albertMaricon : BehaviorScript
{
	[Header("Chase")]
	[SerializeField]
	public ChaseStateScript chase;

	[Header("Seeing")]
	[SerializeField]
	public SeeingConditionScript seeing;

	[Header("Attack")]
	[SerializeField]
	public AttackStateScript attack;

	[Header("Hearing 1")]
	[SerializeField]
	public HearingConditionScript hearing1;

	[Header("Search")]
	[SerializeField]
	public SearchStateScript search;

	[Header("Patrol")]
	[SerializeField]
	public PatrolStateScript patrol;

	[Header("Hearing 2")]
	[SerializeField]
	public HearingConditionScript hearing2;

	[Header("Hearing 0")]
	[SerializeField]
	public HearingConditionScript hearing0;

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
		if(seeing.Condition() && hearing0.Condition() )
		{
			ChangeAttackState();
		}
	}
	public void UpdateAttackState()
	{
		attack.Execute();
		if(hearing1.Condition() )
		{
			ChangeSearchState();
		}
		else if(hearing1.Condition() )
		{
			ChangePatrolState();
		}
	}
	public void UpdateSearchState()
	{
		search.Execute();
		if(hearing2.Condition() )
		{
			ChangeChaseState();
		}
	}
	public void UpdatePatrolState()
	{
		patrol.Execute();
		if(hearing2.Condition() )
		{
			ChangeChaseState();
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
