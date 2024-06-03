namespace FSM.Nodes.States.StatesData
{
    public class VariableData : StateScriptData
    {
        public string selectedVariable;
        public string value;

        public VariableData()
        {
            SetStateName("Variable");
        }
    }
}