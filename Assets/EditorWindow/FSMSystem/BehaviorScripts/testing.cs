using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[Serializable]
public class testing : BehaviorScript
{
	[Header("Patrol")]
	[SerializeField]
	public PatrolState patrol;

	[Header("Chase")]
	[SerializeField]
	public ChaseState chase;

	[Header("Distance")]
	[SerializeField]
	public DistanceCondition distance;

	[Header("Hearing")]
	[SerializeField]
	public HearingCondition hearing;

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
		if(distance.Condition())
		{
			ChangeChaseState();
		}
	}
	public void UpdateChaseState()
	{
		chase.Execute();
		if(hearing.Condition())
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
