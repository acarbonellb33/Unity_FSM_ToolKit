using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class attackTest : BehaviorScript
{
	[Header("Attack")]
	[SerializeField]
	public AttackStateScript attack;

	[Header("Hearing 0")]
	[SerializeField]
	public HearingConditionScript hearing0;

	[Header("Hearing 1")]
	[SerializeField]
	public HearingConditionScript hearing1;

	[Header("Chase")]
	[SerializeField]
	public ChaseStateScript chase;

	[Header("Seeing 0")]
	[SerializeField]
	public SeeingConditionScript seeing0;

	float waitHitTime = 6f;
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
			case FSMStates.Attack:
				UpdateAttackState();
				break;
			case FSMStates.Chase:
				UpdateChaseState();
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
		if(hearing1.Condition())
		{
			ChangeChaseState();
		}
	}
	public void UpdateChaseState()
	{
		chase.Execute();
		if(seeing0.Condition() && hearing0.Condition())
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
			currentState = FSMStates.Chase;
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
