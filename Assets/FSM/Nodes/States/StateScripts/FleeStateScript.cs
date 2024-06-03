namespace FSM.Nodes.States.StateScripts
{
    using UnityEngine;
    using UnityEngine.AI;
    
    public class FleeStateScript : StateScript, IAction
    {
        public float fleeDistance = 10f;
        public float fleeSpeed = 5f;
        public float detectionRange = 15f;

        private readonly float _reachedThreshold = 0.2f;
        
        public FleeStateScript()
        {
            SetStateName("Flee");
        }

        public void Execute()
        {
            if (IsPlayerInRange() && HasReachedDestination())
            {
                Agent.speed = fleeSpeed;
                SetRandomFleeDestination();
            }
        }
        private bool IsPathReachable(Vector3 targetPosition)
        {
            NavMeshPath path = new NavMeshPath();
            Agent.CalculatePath(targetPosition, path);
            return path.status == NavMeshPathStatus.PathComplete;
        }
        private void SetRandomFleeDestination()
        {
            Vector3 randomDirection = Random.insideUnitSphere * 50f;
            randomDirection += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, 100f, NavMesh.AllAreas))
            {
                if (IsPathReachable(hit.position))
                {
                    if (Vector3.Distance(transform.position, hit.position) >= fleeDistance)
                    {
                        Agent.SetDestination(hit.position);
                    }
                }
            }
        }
        private bool IsPlayerInRange()
        {
            return Vector3.Distance(transform.position, Player.transform.position) <= detectionRange;
        }
        public bool HasReachedDestination()
        {
            return !Agent.pathPending && Agent.remainingDistance <= _reachedThreshold;
        }
    }
}
