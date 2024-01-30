using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[Serializable]
public class AlbertCoronilla : BehaviorScript
{
	[Header("Patrol")]
	[SerializeField]
	public PatrolState patrol;

	[Header("Attack")]
	[SerializeField]
	public AttackState attack;

	[Header("Hearing")]
	[SerializeField]
	public HearingCondition hearing;

	[Header("Distance")]
	[SerializeField]
	public DistanceCondition distance;

	public void Patrol()
	{
		patrol.Execute();
		if(distance.Condition())
		{
			attack.Execute();
		}
	}
	public void Attack()
	{
		attack.Execute();
		if(hearing.Condition())
		{
			patrol.Execute();
		}
	}
}
