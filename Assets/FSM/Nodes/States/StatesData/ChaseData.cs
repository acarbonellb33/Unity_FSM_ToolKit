namespace FSM.Nodes.States.StatesData
{
    public class ChaseData : StateScriptData
    {
        public float chaseSpeed = 5f;
        public float chaseRange = 10f;

        public ChaseData()
        {
            SetStateName("Chase");
        }
    }
}