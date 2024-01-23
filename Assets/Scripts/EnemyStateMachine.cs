using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{

	public PatrolState patrolState;

	[Header("Patrol Settings")]

	public System.Single patrolSpeed = 5;
	public System.Int32 patrolRadius = 10;

	private HearingCondition hearingCondition;

	
	public void Update()
	{
	}

	public void Patrol()
	{
		patrolState.Execute();
		if (hearingCondition.Condition())
		{
			//ttack();
		}
	}

}
