using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class testing : BehaviorScript
{
	[Header("Attack")]
	[SerializeField]
	public AttackStateScript attack;

	[Header("Hearing 0")]
	[SerializeField]
	public HearingConditionScript hearing0;

	[Header("Patrol")]
	[SerializeField]
	public PatrolStateScript patrol;

	[Header("Hearing 2")]
	[SerializeField]
	public HearingConditionScript hearing2;

	[Header("Seeing")]
	[SerializeField]
	public SeeingConditionScript seeing;

	[Header("Search")]
	[SerializeField]
	public SearchStateScript search;

	[Header("Hearing 1")]
	[SerializeField]
	public HearingConditionScript hearing1;

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
			case FSMStates.Patrol:
				UpdatePatrolState();
				break;
			case FSMStates.Search:
				UpdateSearchState();
				break;
		}
	}
	public void UpdateAttackState()
	{
		attack.Execute();
		if(hearing0.Condition() && seeing.Condition() && hearing1.Condition() )
		{
			ChangePatrolState();
		}
		else if(hearing0.Condition() && hearing1.Condition() )
		{
			ChangePatrolState();
		}
	}
	public void UpdatePatrolState()
	{
		patrol.Execute();
		if(hearing2.Condition() )
		{
			ChangeAttackState();
		}
	}
	public void UpdateSearchState()
	{
		search.Execute();
		if(hearing2.Condition() )
		{
			ChangeAttackState();
		}
	}
	private void ChangeAttackState()
	{
		currentState = FSMStates.Attack;
	}
	private void ChangePatrolState()
	{
		currentState = FSMStates.Patrol;
	}
	private void ChangeSearchState()
	{
		currentState = FSMStates.Search;
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
