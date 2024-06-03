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
	public class BerserkTest : BehaviorScript
	{
		[Header("Variable")]
		[SerializeField]
		public VariableConditionScript variable;

		[Header("Health")]
		[SerializeField]
		public HealthConditionScript health;

		[Header("Distance 0")]
		[SerializeField]
		public DistanceConditionScript distance0;

		[Header("Chase")]
		[SerializeField]
		public ChaseStateScript chase;

		[Header("Distance 1")]
		[SerializeField]
		public DistanceConditionScript distance1;

		[Header("Attack")]
		[SerializeField]
		public AttackStateScript attack;

		[Header("Idle")]
		[SerializeField]
		public IdleStateScript idle;

		[Header("Distance 2")]
		[SerializeField]
		public DistanceConditionScript distance2;

		private FsmStates _previousState;
		private void Start()
		{
			ChangeIdleState();
		}
		void Update()
		{
			switch (CurrentState)
			{
				case FsmStates.Variable:
					UpdateVariableState();
					break;
				case FsmStates.Chase:
					UpdateChaseState();
					break;
				case FsmStates.Attack:
					UpdateAttackState();
					break;
				case FsmStates.Idle:
					UpdateIdleState();
					break;
			}
		}
		public void UpdateVariableState()
		{
			variable.SetStateScript(attack);
			variable.Execute();
			ChangeAttackState();
		}
		public void UpdateChaseState()
		{
			chase.Execute();
			if(distance1.Condition() && health.Condition())
			{
				ChangeAttackState();
			}
			else if(distance1.Condition() && !health.Condition())
			{
				ChangeVariableState();
			}
		}
		public void UpdateAttackState()
		{
			attack.Execute();
			if(distance2.Condition())
			{
				ChangeIdleState();
			}
		}
		public void UpdateIdleState()
		{
			idle.Execute();
			if(distance0.Condition())
			{
				ChangeChaseState();
			}
		}
		private void ChangeVariableState()
		{
			CurrentState = FsmStates.Variable;
			_previousState = CurrentState;
		}
		private void ChangeChaseState()
		{
			CurrentState = FsmStates.Chase;
			_previousState = CurrentState;
			GetComponent<Animator>().SetTrigger("Run");
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
