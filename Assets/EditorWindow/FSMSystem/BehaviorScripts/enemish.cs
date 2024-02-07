using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class enemish : BehaviorScript
{
	[Header("Patrol")]
	[SerializeField]
	public PatrolStateScript patrol;

	[Header("Chase")]
	[SerializeField]
	public ChaseStateScript chase;

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
			case FSMStates.Chase:
				UpdateChaseState();
				break;
		}
	}
	public void UpdatePatrolState()
	{
		patrol.Execute();
		if(hearing.Condition())
		{
			ChangeChaseState();
		}
	}
	public void UpdateChaseState()
	{
		chase.Execute();
		if(distance.Condition())
		{
			ChangePatrolState();
		}
	}
	private void ChangePatrolState()
	{
		currentState = FSMStates.Patrol;
	}
	private void ChangeChaseState()
	{
		currentState = FSMStates.Chase;
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
