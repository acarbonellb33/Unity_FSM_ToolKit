using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolStateScript : StateScript
{
    public float patrolSpeed = 5f;
    public float patrolRadius = 10f;
    public GameObject patrolPointPrefab;
    public List<GameObject> patrolPoints;
    private int counter = 0;

    public PatrolStateScript()
    {
        SetStateName("Patrol");
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
