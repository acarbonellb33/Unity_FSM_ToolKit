// PatrolState scriptable object
using UnityEngine;

[CreateAssetMenu(fileName = "NewPatrolState", menuName = "Enemy States/Patrol State")]
public class PatrolState : ScriptableObject
{
    public float patrolSpeed = 5f;
    public int patrolRadius = 10;

    public void Execute()
    {
        Debug.Log("Patrolling...");
    }
}