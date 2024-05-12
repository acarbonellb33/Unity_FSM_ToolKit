namespace FSM.Nodes.States.StatesData
{
    public class HearingData : StateScriptData
    {
        public float hearingRange = 10f;
        public float minPlayerSpeed = 5f;

        public HearingData()
        {
            SetStateName("Hearing");
        }
    }
}