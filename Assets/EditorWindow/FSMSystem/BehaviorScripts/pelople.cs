using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class pelople : BehaviorScript
{
	[Header("Patrol")]
	[SerializeField]
	public PatrolStateScript patrol;

	[Header("Hearing")]
	[SerializeField]
	public HearingConditionScript hearing;

	[Header("Seeing 1")]
	[SerializeField]
	public SeeingConditionScript seeing1;

	[Header("Attack")]
	[SerializeField]
	public AttackStateScript attack;

	[Header("Health")]
	[SerializeField]
	public HealthConditionScript health;

	[Header("Seeing 0")]
	[SerializeField]
	public SeeingConditionScript seeing0;

	[Header("Seeing 2")]
	[SerializeField]
	public SeeingConditionScript seeing2;

	private void Start()
	{
		currentState = FSMStates.Attack;
		GetComponent<Animator>().SetFloat("Speed", GetComponent<NavMeshAgent>().speed);
	}
	void Update()
	{
		switch (currentState)
		{
			case FSMStates.Patrol:
				UpdatePatrolState();
				break;
			case FSMStates.Attack:
				UpdateAttackState();
				break;
		}
	}
	public void UpdatePatrolState()
	{
		patrol.Execute();
		if(seeing1.Condition() && hearing.Condition())
		{
			ChangeAttackState();
		}
		else if(!seeing1.Condition() && health.Condition())
		{
			ChangeAttackState();
		}
	}
	public void UpdateAttackState()
	{
		attack.Execute();
		if(seeing2.Condition() && seeing0.Condition())
		{
			ChangePatrolState();
		}
	}
	private void ChangePatrolState()
	{
		currentState = FSMStates.Patrol;
	}
	private void ChangeAttackState()
	{
		currentState = FSMStates.Attack;
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
