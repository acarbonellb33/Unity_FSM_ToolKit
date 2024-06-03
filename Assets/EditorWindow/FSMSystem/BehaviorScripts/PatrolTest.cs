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
	public class PatrolTest : BehaviorScript
	{
		[Header("NextState")]
		[SerializeField]
		public NextStateConditionScript nextState;

		[Header("Patrol")]
		[SerializeField]
		public PatrolStateScript patrol;

		private FsmStates _previousState;
		private void Start()
		{
			ChangePatrolState();
		}
		void Update()
		{
			switch (CurrentState)
			{
				case FsmStates.Patrol:
					UpdatePatrolState();
					break;
			}
		}
		public void UpdatePatrolState()
		{
			patrol.Execute();
			if(nextState.Condition())
			{
				ChangePatrolState();
			}
		}
		private void ChangePatrolState()
		{
			CurrentState = FsmStates.Patrol;
			_previousState = CurrentState;
			GetComponent<Animator>().SetTrigger("Pistol Walk");
		}
		public void AddObjectToList()
		{
			patrol.patrolPoints.Add("");
		}
		public void RemoveObjectFromList(string patrolPoint)
		{
			patrol.RemovePatrolPoint(patrolPoint);
			GameObject patrolPointObject = patrol.FindGameObjectWithId<IDGenerator>(patrolPoint);
			if(patrolPointObject != null)
			{
				DestroyImmediate(patrolPointObject);
			}
		}
		private void OnFootstep() {}
	}
}
