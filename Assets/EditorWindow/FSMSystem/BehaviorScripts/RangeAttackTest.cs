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
	public class RangeAttackTest : BehaviorScript
	{
		[Header("Distance 0")]
		[SerializeField]
		public DistanceConditionScript distance0;

		[Header("Attack")]
		[SerializeField]
		public AttackStateScript attack;

		[Header("Idle")]
		[SerializeField]
		public IdleStateScript idle;

		[Header("Distance 1")]
		[SerializeField]
		public DistanceConditionScript distance1;

		private FsmStates _previousState;
		private void Start()
		{
			ChangeIdleState();
		}
		void Update()
		{
			switch (CurrentState)
			{
				case FsmStates.Attack:
					UpdateAttackState();
					break;
				case FsmStates.Idle:
					UpdateIdleState();
					break;
			}
		}
		public void UpdateAttackState()
		{
			attack.Execute();
			if(distance1.Condition())
			{
				ChangeIdleState();
			}
		}
		public void UpdateIdleState()
		{
			idle.Execute();
			if(distance0.Condition())
			{
				ChangeAttackState();
			}
		}
		private void ChangeAttackState()
		{
			CurrentState = FsmStates.Attack;
			_previousState = CurrentState;
			GetComponent<Animator>().SetTrigger("Punch");
		}
		private void ChangeIdleState()
		{
			CurrentState = FsmStates.Idle;
			_previousState = CurrentState;
			GetComponent<Animator>().SetTrigger("Idle");
		}
		private void OnFootstep() {}
	}
}
