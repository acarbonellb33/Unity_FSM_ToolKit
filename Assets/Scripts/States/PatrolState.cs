// PatrolState scriptable object
using UnityEngine;

[CreateAssetMenu(fileName = "NewPatrolState", menuName = "Enemy States/Patrol State")]
public class PatrolState : State
{
    public float patrolSpeed = 5f;
    public int patrolRadius = 10;

    private PatrolState()
    {
        SetStateName("Patrol");
    }
    public override void Execute()
    {
        Debug.Log("Patrolling...");
    }
}