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
	public class meleeAttack : BehaviorScript
	{
		[Header("Distance 0")]
		[SerializeField]
		public DistanceConditionScript distance0;

		[Header("Attack")]
		[SerializeField]
		public AttackStateScript attack;

		[Header("Distance 1")]
		[SerializeField]
		public DistanceConditionScript distance1;

		[Header("Chase")]
		[SerializeField]
		public ChaseStateScript chase;

		private float _waitHitTime = 2f;
		private float _hitLastTime = 0f;
		private bool _canGetHit = true;
		private bool _canDie = false;

		private FsmStates _previousState;
		private void Start()
		{
			ChangeChaseState();
		}
		void Update()
		{
			switch (CurrentState)
			{
				case FsmStates.Attack:
					UpdateAttackState();
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
		public void UpdateAttackState()
		{
			attack.Execute();
			SetHitData(true, 2f, false);
			if(distance1.Condition())
			{
				ChangeChaseState();
			}
		}
		public void UpdateChaseState()
		{
			chase.Execute();
			SetHitData(true, 2f, false);
			if(distance0.Condition())
			{
				ChangeAttackState();
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
		private void ChangeAttackState()
		{
			CurrentState = FsmStates.Attack;
			_previousState = CurrentState;
			GetComponent<Animator>().SetTrigger("Punch");
		}
		private void ChangeChaseState()
		{
			CurrentState = FsmStates.Chase;
			_previousState = CurrentState;
			GetComponent<Animator>().SetTrigger("Run");
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
