// PatrolState scriptable object

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "NewPatrolState", menuName = "Enemy States/Patrol State")]
public class PatrolState : State
{
    public float patrolSpeed = 5f;
    public int patrolRadius = 10;
    
    public List<GameObject> patrolPoints;
    
    private int counter = 0;
    /*public List<Vector3> patrolPositions = new List<Vector3> { new Vector3(23.5400009f,0.0799999982f,10.4751863f), new Vector3(-28f,0.0799999982f,10.4751863f)
        ,new Vector3(-28f,0.0799999982f,-13f)
        ,new Vector3(23.5400009f,0.0799999982f,-13f)};*/

    private PatrolState()
    {
        patrolPoints = new List<GameObject>();
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