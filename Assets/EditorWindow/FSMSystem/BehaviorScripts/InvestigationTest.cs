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
	public class InvestigationTest : BehaviorScript
	{
		[Header("Search")]
		[SerializeField]
		public SearchStateScript search;

		[Header("CustomCondition")]
		[SerializeField]
		public CustomConditionScript customCondition;

		private FsmStates _previousState;
		private void Start()
		{
			ChangeSearchState();
		}
		void Update()
		{
			switch (CurrentState)
			{
				case FsmStates.Search:
					UpdateSearchState();
					break;
			}
		}
		public void UpdateSearchState()
		{
			search.Execute();
			if(customCondition.Condition())
			{
				ChangeSearchState();
			}
		}
		private void ChangeSearchState()
		{
			CurrentState = FsmStates.Search;
			_previousState = CurrentState;
			GetComponent<Animator>().SetTrigger("Walking");
		}
		private void OnFootstep() {}
	}
}
