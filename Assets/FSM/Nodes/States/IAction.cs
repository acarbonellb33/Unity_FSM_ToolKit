namespace FSM.Nodes.States
{
    /// <summary>
    /// Interface for defining actions.
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Executes the action.
        /// </summary>
        public void Execute();
    }
}