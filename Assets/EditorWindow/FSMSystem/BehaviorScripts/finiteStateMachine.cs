using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[Serializable]
public class finiteStateMachine : BehaviorScript
{
	[Header("Chase")]
	[SerializeField]
	public ChaseState chase;

	[Header("Hearing")]
	[SerializeField]
	public HearingCondition hearing;

	[Header("Patrol")]
	[SerializeField]
	public PatrolState patrol;

	[Header("Distance")]
	[SerializeField]
	public DistanceCondition distance;

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
	public void AddObjectToList(GameObject obj)
	{
		patrol.patrolPoints.Add(obj);
	}
}
