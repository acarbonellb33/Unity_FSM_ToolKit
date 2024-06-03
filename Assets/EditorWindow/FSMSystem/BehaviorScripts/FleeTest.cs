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
	public class FleeTest : BehaviorScript
	{
		[Header("Idle")]
		[SerializeField]
		public IdleStateScript idle;

		[Header("Distance 0")]
		[SerializeField]
		public DistanceConditionScript distance0;

		[Header("CustomCondition")]
		[SerializeField]
		public CustomConditionScript customCondition;

		[Header("Flee")]
		[SerializeField]
		public FleeStateScript flee;

		private FsmStates _previousState;
		private void Start()
		{
			ChangeIdleState();
		}
		void Update()
		{
			switch (CurrentState)
			{
				case FsmStates.Idle:
					UpdateIdleState();
					break;
				case FsmStates.Flee:
					UpdateFleeState();
					break;
			}
		}
		public void UpdateIdleState()
		{
			idle.Execute();
			if(distance0.Condition())
			{
				ChangeFleeState();
			}
		}
		public void UpdateFleeState()
		{
			flee.Execute();
			if(customCondition.Condition())
			{
				ChangeIdleState();
			}
		}
		private void ChangeIdleState()
		{
			CurrentState = FsmStates.Idle;
			_previousState = CurrentState;
			GetComponent<Animator>().SetTrigger("Idle");
		}
		private void ChangeFleeState()
		{
			CurrentState = FsmStates.Flee;
			_previousState = CurrentState;
			GetComponent<Animator>().SetTrigger("Run");
		}
		private void OnFootstep() {}
	}
}
