using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class ccccccccccc : BehaviorScript
{
	[Header("Patrol")]
	[SerializeField]
	public PatrolStateScript patrol;

	[Header("Hearing")]
	[SerializeField]
	public HearingConditionScript hearing;

	[Header("Hearing 0")]
	[SerializeField]
	public HearingConditionScript hearing0;

	[Header("Chase")]
	[SerializeField]
	public ChaseStateScript chase;

	[Header("Seeing")]
	[SerializeField]
	public SeeingConditionScript seeing;

	private void Start()
	{
		currentState = FSMStates.Patrol;
		GetComponent<Animator>().SetFloat("Speed", GetComponent<NavMeshAgent>().speed);
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
		if(hearing.Condition() )
		{
	
		}
	}
	public void UpdateChaseState()
	{
		chase.Execute();
		if(seeing.Condition() )
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
	private void OnFootstep() {}
}
