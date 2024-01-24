// PatrolState scriptable object
using UnityEngine;

[CreateAssetMenu(fileName = "NewPatrolState", menuName = "Enemy States/Patrol State")]
public class PatrolState : EnemyState
{
    public float patrolSpeed = 5f;
    public int patrolRadius = 10;

    public override void Execute()
    {
        Debug.Log("Patrolling...");
    }
}