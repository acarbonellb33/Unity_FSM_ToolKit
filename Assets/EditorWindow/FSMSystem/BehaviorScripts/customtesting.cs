using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Reflection;

[Serializable]
public class customtesting : BehaviorScript
{
	[Header("Distance")]
	[SerializeField]
	public DistanceConditionScript distance;

	[Header("Custom")]
	[SerializeField]
	public CustomStateScript custom;

	[Header("Chase")]
	[SerializeField]
	public ChaseStateScript chase;

	[Header("Hearing")]
	[SerializeField]
	public HearingConditionScript hearing;

	private void Start()
	{
		currentState = FSMStates.Custom;
		GetComponent<Animator>().SetFloat("Speed", GetComponent<NavMeshAgent>().speed);
	}
	void Update()
	{
		switch (currentState)
		{
			case FSMStates.Custom:
				UpdateCustomState();
				break;
			case FSMStates.Chase:
				UpdateChaseState();
				break;
		}
	}
	public void UpdateCustomState()
	{
		custom.Execute();
		GetComponent<Animator>().SetBool("Grounded", false);
		if(hearing.Condition())
		{
			ChangeChaseState();
		}
		else if(!hearing.Condition())
		{
			ChangeChaseState();
		}
	}
	public void UpdateChaseState()
	{
		chase.Execute();
		GetComponent<Animator>().SetBool("FreeFall", false);
		if(distance.Condition())
		{
			ChangeCustomState();
		}
	}
	private void ChangeCustomState()
	{
		currentState = FSMStates.Custom;
	}
	private void ChangeChaseState()
	{
		currentState = FSMStates.Chase;
	}
	private void OnFootstep() {}
}
