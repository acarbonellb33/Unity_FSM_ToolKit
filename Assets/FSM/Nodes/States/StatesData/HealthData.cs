namespace FSM.Nodes.States.StatesData
{
    using Enumerations;

    public class HealthData : StateScriptData
    {
        public FsmOperands operand;
        public float health = 10f;

        public HealthData()
        {
            SetStateName("Health");
        }
    }
}