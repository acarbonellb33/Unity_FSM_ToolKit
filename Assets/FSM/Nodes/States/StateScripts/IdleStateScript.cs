namespace FSM.Nodes.States.StateScripts
{
    public class IdleStateScript : StateScript, IAction
    {
        public IdleStateScript()
        {
            SetStateName("Idle");
        }
        public void Execute() {}
    }
}

