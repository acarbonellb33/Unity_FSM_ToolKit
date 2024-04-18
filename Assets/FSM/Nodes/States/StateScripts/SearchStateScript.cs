using UnityEngine;
using UnityEngine.AI;

public class SearchStateScript : StateScript, IAction
{
    public float exploreRadius = 10f; // Radius within which the enemy will explore
    private Vector3 randomDestination; // Random destination within the explore radius
    
    private bool isSearching = false; // Flag to indicate if the enemy is currently searching
    
    public SearchStateScript()
    {
        // Set the state name to "Search" using the SetStateName method inherited from StateScript
        SetStateName("Search");
    }

    // Override the Execute method from the base StateScript class
    public void Execute()
    {
        if (!isSearching)
        {
            //Set the random destination for the first time
            SetRandomDestination();
            isSearching = true;
        }
        // Set the agent's destination to a random position within the search area
        // 'agent' and 'searchArea' are inherited from the parent class StateScript
        if (Vector3.Distance(transform.position, randomDestination) < 1f)
        {
            SetRandomDestination();
        }
    }
    
    void SetRandomDestination()
    {
        // Generate a random direction within the explore radius
        Vector3 randomDirection = Random.insideUnitSphere * exploreRadius;
        randomDirection += transform.position;
        
        // Find a point on the NavMesh within the explore radius
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, exploreRadius, NavMesh.AllAreas);

        // Set the random destination to the point on the NavMesh and set the agent's destination
        randomDestination = hit.position;
        agent.SetDestination(randomDestination);
    }
}
