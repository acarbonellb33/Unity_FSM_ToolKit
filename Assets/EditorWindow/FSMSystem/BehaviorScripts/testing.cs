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

	[Header("Seeing 1")]
	[SerializeField]
	public SeeingConditionScript seeing1;

	[Header("Distance")]
	[SerializeField]
	public DistanceConditionScript distance;

	[Header("Seeing 0")]
	[SerializeField]
	public SeeingConditionScript seeing0;

	[Header("Chase")]
	[SerializeField]
	public ChaseStateScript chase;

	[Header("Search")]
	[SerializeField]
	public SearchStateScript search;

	[Header("Health")]
	[SerializeField]
	public HealthConditionScript health;

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
		if(distance.Condition() && seeing0.Condition())
		{
			ChangeChaseState();
		}
		else if(!distance.Condition())
		{
			ChangeSearchState();
		}
	}
	public void UpdateChaseState()
	{
		chase.Execute();
		if(seeing1.Condition())
		{
			ChangeAttackState();
		}
	}
	public void UpdateSearchState()
	{
		search.Execute();
		if(health.Condition())
		{
			ChangeChaseState();
		}
		else if(!health.Condition() && seeing1.Condition())
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
	private void ChangeHitState()
	{
		currentState = FSMStates.Hit;
	}
	private void ChangeDieState()
	{
		currentState = FSMStates.Die;
	}
	private void OnFootstep() {}
}
