using System.Collections.Generic;
using UnityEngine;

// PatrolStateScript class inherits from StateScript and implements IAction interface
public class PatrolStateScript : StateScript, IAction
{
    // List to store patrol points as GameObjects
    public List<GameObject> patrolPoints;
    private int counter;
    
    public PatrolStateScript()
    {
        // Set the state name to "Patrol" using the SetStateName method inherited from StateScript
        SetStateName("Patrol");
        counter = 0;
    }

    // Implementation of the Execute method from the IAction interface
    public void Execute()
    {
        // Call the MoveToNextPatrolPosition method to move the agent to the next patrol point
        MoveToNextPatrolPosition();
    }

    // Private method to move the agent to the next patrol position
    private void MoveToNextPatrolPosition()
    {
        // Check if the agent's remaining distance is close to 0
        if(agent.remainingDistance <= 0.1)
        {
            // Reset the counter if it reaches the end of the patrol points list
            if(counter == patrolPoints.Count)
            {
                counter = 0;
            }

            // Check if the patrolPoints list is empty
            if(patrolPoints.Count == 0)
            {
                return;
            }

            // Set the agent's destination to the position of the current patrol point
            agent.SetDestination(patrolPoints[counter].transform.position);

            // Increment the counter for the next patrol point
            counter++;
        }
    }

    // Method to remove a patrol point from the list
    public void RemovePatrolPoint(GameObject patrolPoint)
    {
        // Remove the specified patrol point from the list
        patrolPoints.Remove(patrolPoint);

        // Update the names of the remaining patrol points
        for(int i = 0; i < patrolPoints.Count; i++)
        {
            patrolPoints[i].name = "Patrol Point " + i;
        }
    }
}
