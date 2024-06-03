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
	public class AmbushTest : BehaviorScript
	{
		[Header("Attack")]
		[SerializeField]
		public AttackStateScript attack;

		[Header("Distance 2")]
		[SerializeField]
		public DistanceConditionScript distance2;

		[Header("Distance 1")]
		[SerializeField]
		public DistanceConditionScript distance1;

		[Header("Distance 0")]
		[SerializeField]
		public DistanceConditionScript distance0;

		[Header("Seeing")]
		[SerializeField]
		public SeeingConditionScript seeing;

		[Header("Idle")]
		[SerializeField]
		public IdleStateScript idle;

		[Header("Chase")]
		[SerializeField]
		public ChaseStateScript chase;

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
				case FsmStates.Chase:
					UpdateChaseState();
					break;
			}
		}
		public void UpdateAttackState()
		{
			attack.Execute();
			if(distance2.Condition())
			{
				ChangeChaseState();
			}
			else if(!distance2.Condition())
			{
				ChangeAttackState();
			}
		}
		public void UpdateIdleState()
		{
			idle.Execute();
			if(distance0.Condition() && seeing.Condition())
			{
				ChangeChaseState();
			}
		}
		public void UpdateChaseState()
		{
			chase.Execute();
			if(distance1.Condition())
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
		private void ChangeChaseState()
		{
			CurrentState = FsmStates.Chase;
			_previousState = CurrentState;
			GetComponent<Animator>().SetTrigger("Run");
		}
		private void OnFootstep() {}
	}
}
