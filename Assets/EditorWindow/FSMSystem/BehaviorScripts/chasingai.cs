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
	public class chasingai : BehaviorScript
	{
		[Header("Distance 0")]
		[SerializeField]
		public DistanceConditionScript distance0;

		[Header("Search")]
		[SerializeField]
		public SearchStateScript search;

		[Header("Chase")]
		[SerializeField]
		public ChaseStateScript chase;

		[Header("Distance 1")]
		[SerializeField]
		public DistanceConditionScript distance1;

		[Header("NextState")]
		[SerializeField]
		public NextStateConditionScript nextState;

		private float _waitHitTime = 8f;
		private float _hitLastTime = 0f;
		private bool _canGetHit = true;
		private bool _canDie = false;

		private FsmStates _previousState;
		private void Start()
		{
			CurrentState = FsmStates.Chase;
			GetComponent<Animator>().SetFloat("Speed", GetComponent<NavMeshAgent>().speed);
		}
		void Update()
		{
			switch (CurrentState)
			{
				case FsmStates.Search:
					UpdateSearchState();
					break;
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
			if(_canGetHit && healthSystem.GetCurrentHealth() < healthSystem.GetPreviousHealth())
			{
				ChangeHitState();
				_hitLastTime = Time.time;
				healthSystem.SetPreviousHealth(healthSystem.GetCurrentHealth());
			}
			if(_canDie && healthSystem.GetCurrentHealth() <= 0)
			{
				ChangeDieState();
			}
		}
		public void UpdateSearchState()
		{
			search.Execute();
			SetHitData(true, 4f, true);
			if(distance0.Condition())
			{
				ChangeChaseState();
			}
		}
		public void UpdateChaseState()
		{
			chase.Execute();
			GetComponent<Animator>().SetBool("Grounded", true);
			SetHitData(true, 10f, false);
			if(nextState.Condition() && distance1.Condition())
			{
				ChangeSearchState();
			}
		}
		public void UpdateHitState()
		{
			NavMeshAgent agent = GetComponent<NavMeshAgent>();
			agent.isStopped = true;
			if(Time.time >= _hitLastTime + _waitHitTime)
			{
				CurrentState = _previousState;
				agent.isStopped = false;
			}
		}
		public void UpdateDieState()
		{
			GetComponent<EnemyHealthSystem>().Die();
		}
		private void ChangeSearchState()
		{
			CurrentState = FsmStates.Search;
			_previousState = CurrentState;
		}
		private void ChangeChaseState()
		{
			CurrentState = FsmStates.Chase;
			_previousState = CurrentState;
		}
		private void ChangeHitState()
		{
			CurrentState = FsmStates.Hit;
		}
		private void ChangeDieState()
		{
			CurrentState = FsmStates.Die;
		}
		private void SetHitData(bool canGetHit, float timeToWait, bool canDie)
		{
			_canGetHit = canGetHit;
			_waitHitTime = timeToWait;
			_canDie = canDie;
		}
		private void OnFootstep() {}
	}
}
