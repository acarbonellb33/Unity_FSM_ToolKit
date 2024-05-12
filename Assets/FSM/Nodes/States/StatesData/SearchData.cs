namespace FSM.Nodes.States.StatesData
{
    public class SearchData : StateScriptData
    {
        public float exploreRadius = 10f;

        public SearchData()
        {
            SetStateName("Search");
        }
    }
}