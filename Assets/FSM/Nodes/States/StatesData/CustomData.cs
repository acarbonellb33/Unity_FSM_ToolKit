namespace FSM.Nodes.States.StatesData
{
    public class CustomData : StateScriptData
    {
        public string selectedGameObject;
        public string selectedComponent;
        public string selectedFunction;

        public CustomData()
        {
            SetStateName("Custom");
        }
    }
}