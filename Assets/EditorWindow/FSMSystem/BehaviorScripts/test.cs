using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class test : BehaviorScript
{
	[Header("Patrol")]
	[SerializeField]
	public PatrolStateScript patrol;

	[Header("Attack")]
	[SerializeField]
	public AttackStateScript attack;

	[Header("Hearing")]
	[SerializeField]
	public HearingConditionScript hearing;

	[Header("Distance")]
	[SerializeField]
	public DistanceConditionScript distance;

	private void Start()
	{
		currentState = FSMStates.Patrol;
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
		if(hearing.Condition())
		{
			ChangeAttackState();
		}
	}
	public void UpdateAttackState()
	{
		attack.Execute();
		if(distance.Condition())
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
}
