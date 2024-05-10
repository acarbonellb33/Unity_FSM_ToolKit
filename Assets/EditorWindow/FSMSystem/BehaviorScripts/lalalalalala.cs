using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class lalalalalala : BehaviorScript
{
	[Header("Chase")]
	[SerializeField]
	public ChaseStateScript chase;

	[Header("Distance 0")]
	[SerializeField]
	public DistanceConditionScript distance0;

	[Header("Seeing")]
	[SerializeField]
	public SeeingConditionScript seeing;

	[Header("Attack")]
	[SerializeField]
	public AttackStateScript attack;

	float waitHitTime = 2f;
	float hitLastTime = 0f;
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
	public void UpdateChaseState()
	{
		chase.Execute();
		if(seeing.Condition())
		{
			ChangeAttackState();
		}
	}
	public void UpdateAttackState()
	{
		attack.Execute();
		if(distance0.Condition())
		{
			ChangeChaseState();
		}
	}
	public void UpdateHitState()
	{
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		agent.isStopped = true;
		if(Time.time >= hitLastTime + waitHitTime)
		{
			currentState = FSMStates.Chase;
			agent.isStopped = false;
		}
	}
	public void UpdateDieState()
	{
		GetComponent<EnemyHealthSystem>().Die();
	}
	private void ChangeChaseState()
	{
		currentState = FSMStates.Chase;
	}
	private void ChangeAttackState()
	{
		currentState = FSMStates.Attack;
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
