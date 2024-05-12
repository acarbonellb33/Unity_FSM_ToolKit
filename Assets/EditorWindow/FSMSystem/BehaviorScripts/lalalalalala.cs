namespace EditorWindow.FSMSystem.BehaviorScripts
{
	using UnityEngine;
	using System;
	using UnityEngine.AI;
	using FSM.Enemy;
	using FSM.Enumerations;
	using FSM.Nodes.States.StateScripts;
	using FSM.Utilities;

	[Serializable]
	public class lalalalalala : BehaviorScript
	{
		[Header("Chase")]
		[SerializeField]
		public ChaseStateScript chase;

		[Header("NextState 1")]
		[SerializeField]
		public NextStateConditionScript nextState1;

		[Header("NextState 0")]
		[SerializeField]
		public NextStateConditionScript nextState0;

		float waitHitTime = 2f;
		float hitLastTime = 0f;
		private void Start()
		{
			CurrentState = FsmStates.Chase;
			GetComponent<Animator>().SetFloat("Speed", GetComponent<NavMeshAgent>().speed);
		}
		void Update()
		{
			switch (CurrentState)
			{
				case FsmStates.Chase:
					UpdateChaseState();
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
		public void UpdateChaseState()
		{
			chase.Execute();
			if(nextState0.Condition() && nextState1.Condition())
			{
				ChangeChaseState();
			}
		}
		public void UpdateHitState()
		{
			NavMeshAgent agent = GetComponent<NavMeshAgent>();
			agent.isStopped = true;
			if(Time.time >= hitLastTime + waitHitTime)
			{
				CurrentState = FsmStates.Chase;
				agent.isStopped = false;
			}
		}
		public void UpdateDieState()
		{
			GetComponent<EnemyHealthSystem>().Die();
		}
		private void ChangeChaseState()
		{
			CurrentState = FsmStates.Chase;
		}
		private void ChangeHitState()
		{
			CurrentState = FsmStates.Hit;
		}
		private void ChangeDieState()
		{
			CurrentState = FsmStates.Die;
		}
		private void OnFootstep() {}
	}
}
