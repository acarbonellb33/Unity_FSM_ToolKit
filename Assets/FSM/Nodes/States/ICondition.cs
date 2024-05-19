namespace FSM.Nodes.States
{
    /// <summary>
    /// Interface for defining conditions.
    /// </summary>
    public interface ICondition
    {
        /// <summary>
        /// Evaluates the condition.
        /// </summary>
        /// <returns>True if the condition is met, otherwise false.</returns>
        public bool Condition();
    }
}