using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[Serializable]
public class aaaaaaaa : BehaviorScript
{
	[Header("Patrol")]
	[SerializeField]
	public PatrolStateScript patrol;

	[Header("Hearing")]
	[SerializeField]
	public HearingConditionScript hearing;

	[Header("Chase")]
	[SerializeField]
	public ChaseStateScript chase;

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
	public void AddObjectToList(GameObject obj)
	{
		patrol.patrolPoints.Add(obj);
	}
}
