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
		[Header("Distance 0")]
		[SerializeField]
		public DistanceConditionScript distance0;

		[Header("Patrol")]
		[SerializeField]
		public PatrolStateScript patrol;

		[Header("Chase")]
		[SerializeField]
		public ChaseStateScript chase;

		[Header("Hearing")]
		[SerializeField]
		public HearingConditionScript hearing;

		[Header("Seeing 1")]
		[SerializeField]
		public SeeingConditionScript seeing1;

		[Header("Seeing 0")]
		[SerializeField]
		public SeeingConditionScript seeing0;

		[Header("Attack")]
		[SerializeField]
		public AttackStateScript attack;

		[Header("Health")]
		[SerializeField]
		public HealthConditionScript health;

		float waitHitTime = 2f;
		float hitLastTime = 0f;
		private void Start()
		{
			CurrentState = FsmStates.Patrol;
			GetComponent<Animator>().SetFloat("Speed", GetComponent<NavMeshAgent>().speed);
		}
		void Update()
		{
			switch (CurrentState)
			{
				case FsmStates.Patrol:
					UpdatePatrolState();
					break;
				case FsmStates.Chase:
					UpdateChaseState();
					break;
				case FsmStates.Attack:
					UpdateAttackState();
					break;
				case FsmStates.Hit:
					UpdateHitState();
					break;
				case FsmStates.Die:
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
		public void UpdatePatrolState()
		{
			patrol.Execute();
			if(seeing0.Condition() && seeing1.Condition())
			{
				ChangeChaseState();
			}
		}
		public void UpdateChaseState()
		{
			chase.Execute();
			if(hearing.Condition() && health.Condition())
			{
				ChangeAttackState();
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
		public void UpdateHitState()
		{
			NavMeshAgent agent = GetComponent<NavMeshAgent>();
			agent.isStopped = true;
			if(Time.time >= hitLastTime + waitHitTime)
			{
				CurrentState = FsmStates.Patrol;
				agent.isStopped = false;
			}
		}
		public void UpdateDieState()
		{
			GetComponent<EnemyHealthSystem>().Die();
		}
		private void ChangePatrolState()
		{
			CurrentState = FsmStates.Patrol;
		}
		private void ChangeChaseState()
		{
			CurrentState = FsmStates.Chase;
		}
		private void ChangeAttackState()
		{
			CurrentState = FsmStates.Attack;
		}
		private void ChangeHitState()
		{
			CurrentState = FsmStates.Hit;
		}
		private void ChangeDieState()
		{
			CurrentState = FsmStates.Die;
		}
		public void AddObjectToList()
		{
			patrol.patrolPoints.Add("");
		}
		public void RemoveObjectFromList(string patrolPoint)
		{
			patrol.RemovePatrolPoint(patrolPoint);
			GameObject patrolPointObject = FsmIOUtility.FindGameObjectWithId<IDGenerator>(patrolPoint);
			if(patrolPointObject != null)
			{
				DestroyImmediate(patrolPointObject);
			}
		}
		private void OnFootstep() {}
	}
}
#endif
