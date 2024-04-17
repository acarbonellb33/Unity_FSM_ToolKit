using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolStateScript : StateScript, IAction
{
    public List<GameObject> patrolPoints;
    private int counter;

    public PatrolStateScript()
    {
        SetStateName("Patrol");
        counter = 0;
    }
    public void Execute()
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
