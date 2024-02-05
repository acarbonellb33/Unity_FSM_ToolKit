// PatrolState scriptable object

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "NewPatrolState", menuName = "Enemy States/Patrol State")]
public class PatrolState : State
{
    public float patrolSpeed = 5f;
    public int patrolRadius = 10;
    [SerializeField]
    public GameObject patrolPointPrefab;
    
    public List<GameObject> patrolPoints;
    private int counter = 0;
    private PatrolState()
    {
        SetStateName("Patrol");
        patrolPoints = new List<GameObject>();
    }
    public override void Execute()
    {
        MoveToNextPatrolPosition();
    }
    
    private void MoveToNextPatrolPosition()
    {
        if(agent.remainingDistance <= 0.1)
        {
            if(counter==patrolPoints.Count)counter=0;
            if (patrolPoints.Count == 0) return;
            agent.SetDestination(patrolPoints[counter].transform.position);
            counter++;
        }
    }
    
    public void RemovePatrolPoint(GameObject patrolPoint)
    {
        patrolPoints.Remove(patrolPoint);
        for(int i = 0; i < patrolPoints.Count; i++)
        {
            patrolPoints[i].name = "Patrol Point " + i;
        }
    }
}