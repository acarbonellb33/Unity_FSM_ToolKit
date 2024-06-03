namespace FSM.Nodes.States.StatesData
{
    public class CustomConditionData : StateScriptData
    {
        public string selectedGameObject;
        public string selectedComponent;
        public string selectedFunction;

        public CustomConditionData()
        {
            SetStateName("CustomCondition");
        }
    }
}
