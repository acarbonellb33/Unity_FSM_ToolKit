using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class newUpdate : BehaviorScript
{
	[Header("Attack")]
	[SerializeField]
	public AttackStateScript attack;

	[Header("Hearing 0")]
	[SerializeField]
	public HearingConditionScript hearing0;

	[Header("Seeing 0")]
	[SerializeField]
	public SeeingConditionScript seeing0;

	[Header("Chase")]
	[SerializeField]
	public ChaseStateScript chase;

	[Header("Seeing 1")]
	[SerializeField]
	public SeeingConditionScript seeing1;

	[Header("Search")]
	[SerializeField]
	public SearchStateScript search;

	[Header("Hearing 1")]
	[SerializeField]
	public HearingConditionScript hearing1;

	[Header("Hearing 2")]
	[SerializeField]
	public HearingConditionScript hearing2;

	[Header("Seeing 2")]
	[SerializeField]
	public SeeingConditionScript seeing2;

	[Header("Hearing 3")]
	[SerializeField]
	public HearingConditionScript hearing3;

	[Header("Patrol")]
	[SerializeField]
	public PatrolStateScript patrol;

	float waitHitTime = 2f;
	float hitLastTime = 0f;
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
			case FSMStates.Chase:
				UpdateChaseState();
				break;
			case FSMStates.Search:
				UpdateSearchState();
				break;
			case FSMStates.Patrol:
				UpdatePatrolState();
				break;
			case FSMStates.Hit:
				UpdateHitState();
				break;
			case FSMStates.Die:
				UpdateDieState();
				break;
		}
		EnemyHealthSystem healthSystem = GetComponent<EnemyHealthSystem>();
		if(healthSystem.GetCurrentHealth() < healthSystem.GetPreviousHealth())
		{
			ChangeHitState();
			healthSystem.SetPreviousHealth(healthSystem.GetCurrentHealth());
		}
		if(healthSystem.GetCurrentHealth() <= 0)
		{
			ChangeDieState();
		}
	}
	public void UpdateAttackState()
	{
		attack.Execute();
		if(hearing0.Condition() && seeing0.Condition() && hearing2.Condition())
		{
			ChangePatrolState();
		}
		else if(hearing0.Condition() && seeing0.Condition() && !hearing2.Condition() && hearing3.Condition() && seeing2.Condition())
		{
			ChangePatrolState();
		}
		else if(hearing0.Condition() && !seeing0.Condition())
		{
			ChangeChaseState();
		}
	}
	public void UpdateChaseState()
	{
		chase.Execute();
		if(seeing1.Condition())
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
		if(hearing1.Condition())
		{
			ChangeAttackState();
		}
	}
	public void UpdateHitState()
	{
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		agent.isStopped = true;
		if(Time.time >= hitLastTime + waitHitTime)
		{
			currentState = FSMStates.Attack;
			agent.isStopped = false;
		}
	}
	public void UpdateDieState()
	{
		GetComponent<EnemyHealthSystem>().Die();
	}
	private void ChangeAttackState()
	{
		currentState = FSMStates.Attack;
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
	private void ChangeHitState()
	{
		currentState = FSMStates.Hit;
	}
	private void ChangeDieState()
	{
		currentState = FSMStates.Die;
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
