using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;
using System.Threading.Tasks;

[Serializable]
public class lalalalalala : BehaviorScript
{
	[Header("Attack")]
	[SerializeField]
	public AttackStateScript attack;

	[Header("Seeing")]
	[SerializeField]
	public SeeingConditionScript seeing;

	[Header("Patrol")]
	[SerializeField]
	public PatrolStateScript patrol;

	[Header("Distance 0")]
	[SerializeField]
	public DistanceConditionScript distance0;

	float waitHitTime = 2f;
	float hitLastTime = 0f;
	private void Start()
	{
		currentState = FSMStates.Patrol;
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
		if(distance0.Condition())
		{
			ChangePatrolState();
		}
	}
	public void UpdatePatrolState()
	{
		patrol.Execute();
		if(seeing.Condition())
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
			currentState = FSMStates.Patrol;
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
		newGameObject.AddComponent<IDGenerator>().GetUniqueID();
		Debug.Log(newGameObject.GetComponent<IDGenerator>());
		patrol.patrolPoints.Add(newGameObject.GetComponent<IDGenerator>().GetUniqueID());
		return newGameObject;
	}
	public void RemoveObjectFromList(string patrolPoint)
	{
		patrol.RemovePatrolPoint(patrolPoint);
		GameObject patrolPointObject = FSMIOUtility.FindGameObjectWithId<IDGenerator>(patrolPoint);
		if(patrolPointObject != null)
		{
			DestroyImmediate(patrolPointObject);
		}
	}
	private void OnFootstep() {}
}
