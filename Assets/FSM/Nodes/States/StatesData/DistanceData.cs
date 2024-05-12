namespace FSM.Nodes.States.StatesData
{
    using Enumerations;

    public class DistanceData : StateScriptData
    {
        public FSMOperands operand;
        public float distanceFromPlayer = 10f;

        public DistanceData()
        {
            SetStateName("Distance");
        }
    }
}