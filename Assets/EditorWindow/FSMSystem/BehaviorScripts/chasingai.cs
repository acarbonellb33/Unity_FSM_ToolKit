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
		[Header("Distance")]
		[SerializeField]
		public DistanceConditionScript distance;

		[Header("Chase")]
		[SerializeField]
		public ChaseStateScript chase;

		[Header("Search")]
		[SerializeField]
		public SearchStateScript search;

		[Header("NextState")]
		[SerializeField]
		public NextStateConditionScript nextState;

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
				case FsmStates.Search:
					UpdateSearchState();
					break;
			}
		}
		public void UpdateChaseState()
		{
			chase.Execute();
			if(nextState.Condition())
			{
				ChangeSearchState();
			}
		}
		public void UpdateSearchState()
		{
			search.Execute();
			if(distance.Condition())
			{
				ChangeChaseState();
			}
		}
		private void ChangeChaseState()
		{
			CurrentState = FsmStates.Chase;
		}
		private void ChangeSearchState()
		{
			CurrentState = FsmStates.Search;
		}
		private void OnFootstep() {}
	}
}
