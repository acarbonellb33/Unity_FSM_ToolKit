#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.BehaviorScripts
{
	using UnityEngine;
	using System;
	using UnityEngine.AI;
	using Utilities;
	using FSM.Enemy;
	using FSM.Enumerations;
	using FSM.Nodes.States.StateScripts;
	using FSM.Utilities;

	[Serializable]
	public class lalalalalala : BehaviorScript
	{
		[Header("Attack")]
		[SerializeField]
		public AttackStateScript attack;

		[Header("Distance 0")]
		[SerializeField]
		public DistanceConditionScript distance0;

		[Header("Seeing 0")]
		[SerializeField]
		public SeeingConditionScript seeing0;

		[Header("Patrol")]
		[SerializeField]
		public PatrolStateScript patrol;

		float waitHitTime = 2f;
		float hitLastTime = 0f;
		private void Start()
		{
			currentState = FSMStates.Patrol;
			GetComponent<Animator>().SetFloat("Speed", GetComponent<NavMeshAgent>().speed);
		}
		void Update()
		{
			switch (currentState)
			{
				case FSMStates.Attack:
					UpdateAttackState();
					break;
				case FSMStates.Patrol:
					UpdatePatrolState();
					break;
				case FSMStates.Hit:
					UpdateHitState();
					break;
				case FSMStates.Die:
					UpdateDieState();
					break;
			}
			EnemyHealthSystem healthSystem = GetComponent<EnemyHealthSystem>();
			if(healthSystem.GetCurrentHealth() < healthSystem.GetPreviousHealth())
			{
				ChangeHitState();
				healthSystem.SetPreviousHealth(healthSystem.GetCurrentHealth());
			}
			if(healthSystem.GetCurrentHealth() <= 0)
			{
				ChangeDieState();
			}
		}
		public void UpdateAttackState()
		{
			attack.Execute();
		if(distance0.Condition())
		{
			ChangePatrolState();
		}
		}
		public void UpdatePatrolState()
		{
			patrol.Execute();
		if(seeing0.Condition())
		{
			ChangeAttackState();
		}
		}
		public void UpdateHitState()
		{
			NavMeshAgent agent = GetComponent<NavMeshAgent>();
			agent.isStopped = true;
			if(Time.time >= hitLastTime + waitHitTime)
			{
				currentState = FSMStates.Patrol;
				agent.isStopped = false;
			}
		}
		public void UpdateDieState()
		{
			GetComponent<EnemyHealthSystem>().Die();
		}
		private void ChangeAttackState()
		{
			currentState = FSMStates.Attack;
		}
		private void ChangePatrolState()
		{
			currentState = FSMStates.Patrol;
		}
		private void ChangeHitState()
		{
			currentState = FSMStates.Hit;
		}
		private void ChangeDieState()
		{
			currentState = FSMStates.Die;
		}
		public void AddObjectToList()
		{
			patrol.patrolPoints.Add("");
		}
		public void RemoveObjectFromList(string patrolPoint)
		{
			patrol.RemovePatrolPoint(patrolPoint);
			GameObject patrolPointObject = FSMIOUtility.FindGameObjectWithId<IDGenerator>(patrolPoint);
			if(patrolPointObject != null)
			{
				DestroyImmediate(patrolPointObject);
			}
		}
		private void OnFootstep() {}
	}
}
#endif
