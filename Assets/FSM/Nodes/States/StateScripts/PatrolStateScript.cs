namespace FSM.Nodes.States.StateScripts
{
    using System.Collections.Generic;
    using UnityEngine;
    using Utilities;
    // PatrolStateScript class inherits from StateScript and implements IAction interface
    public class PatrolStateScript : StateScript, IAction
    {
        // List to store patrol points as GameObjects
        public List<string> patrolPoints;
        private int _counter;

        public PatrolStateScript()
        {
            // Set the state name to "Patrol" using the SetStateName method inherited from StateScript
            SetStateName("Patrol");
            patrolPoints = new List<string>();
            _counter = 0;
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
            if (Agent.remainingDistance <= 0.1)
            {
                // Reset the counter if it reaches the end of the patrol points list
                if (_counter == patrolPoints.Count)
                {
                    _counter = 0;
                }

                // Check if the patrolPoints list is empty
                if (patrolPoints.Count == 0)
                {
                    return;
                }

                // Set the agent's destination to the position of the current patrol point
                GameObject patrolPoint = FindGameObjectWithId<IDGenerator>(patrolPoints[_counter]);
                Agent.SetDestination(patrolPoint.transform.position);

                // Increment the counter for the next patrol point
                _counter++;
            }
        }

        // Method to remove a patrol point from the list
        public void RemovePatrolPoint(string patrolPoint)
        {
            // Remove the specified patrol point from the list
            patrolPoints.Remove(patrolPoint);
        }
        public GameObject FindGameObjectWithId<T>(string id) where T : MonoBehaviour
        {
            // Find all GameObjects with the component of type T
            T[] components = GameObject.FindObjectsOfType<T>();

            // Iterate through each GameObject and check if it has the specified ID
            foreach (T component in components)
            {
                // Check if the component has an IDGenerator attached
                IDGenerator idGenerator = component.GetComponent<IDGenerator>();
                if (idGenerator != null && idGenerator.GetUniqueID() == id)
                {
                    // Return the GameObject if the ID matches
                    return component.gameObject;
                }
            }

            // Return null if no matching GameObject is found
            return null;
        }
    }
}
