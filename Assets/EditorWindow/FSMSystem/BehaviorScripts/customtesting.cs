using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class customtesting : BehaviorScript
{
	[Header("Hearing")]
	[SerializeField]
	public HearingConditionScript hearing;

	[Header("Chase")]
	[SerializeField]
	public ChaseStateScript chase;

	[Header("Distance")]
	[SerializeField]
	public DistanceConditionScript distance;

	[Header("Custom")]
	[SerializeField]
	public CustomStateScript custom;

	private void Start()
	{
		currentState = FSMStates.Custom;
		GetComponent<Animator>().SetFloat("Speed", GetComponent<NavMeshAgent>().speed);
	}
	void Update()
	{
		switch (currentState)
		{
			case FSMStates.Chase:
				UpdateChaseState();
				break;
			case FSMStates.Custom:
				UpdateCustomState();
				break;
		}
	}
	public void UpdateChaseState()
	{
		chase.Execute();
		if(distance.Condition())
		{
			ChangeCustomState();
		}
	}
	public void UpdateCustomState()
	{
		custom.Execute();
		if(hearing.Condition())
		{
			ChangeChaseState();
		}
	}
	private void ChangeChaseState()
	{
		currentState = FSMStates.Chase;
	}
	private void ChangeCustomState()
	{
		currentState = FSMStates.Custom;
	}
	private void OnFootstep() {}
}
