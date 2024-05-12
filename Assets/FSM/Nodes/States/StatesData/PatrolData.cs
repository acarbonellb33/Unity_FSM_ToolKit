namespace FSM.Nodes.States.StatesData
{
    using System.Collections.Generic;

    public class PatrolData : StateScriptData
    {
        public List<string> patrolPoints;

        public PatrolData()
        {
            SetStateName("Patrol");
            patrolPoints = new List<string>();
        }
    }
}