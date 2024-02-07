using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class arnau : BehaviorScript
{
	[Header("Chase")]
	[SerializeField]
	public ChaseStateScript chase;

	[Header("Hearing")]
	[SerializeField]
	public HearingConditionScript hearing;

	[Header("Distance")]
	[SerializeField]
	public DistanceConditionScript distance;

	[Header("Patrol")]
	[SerializeField]
	public PatrolStateScript patrol;

	private void Start()
	{
		currentState = FSMStates.Patrol;
	}
	void Update()
	{
		switch (currentState)
		{
			case FSMStates.Chase:
				UpdateChaseState();
				break;
			case FSMStates.Patrol:
				UpdatePatrolState();
				break;
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
	public void UpdatePatrolState()
	{
		patrol.Execute();
		if(hearing.Condition())
		{
			ChangeChaseState();
		}
	}
	private void ChangeChaseState()
	{
		currentState = FSMStates.Chase;
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
}
