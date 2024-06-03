namespace FSM.Nodes.States.StatesData
{
    public class FleeData : StateScriptData
    {
        public float fleeDistance = 10f;
        public float fleeSpeed = 5f;
        public float detectionRange = 15f;
        
        public FleeData()
        {
            SetStateName("Flee");
        }
    }
}