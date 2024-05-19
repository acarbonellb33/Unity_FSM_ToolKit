#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Data.Error
{
    using System.Collections.Generic;
    using Elements;

    /// <summary>
    /// Represents error data associated with FSM nodes.
    /// </summary>
    public class FsmNodeErrorData
    {
        /// <summary>
        /// Gets the error data associated with the FSM node error.
        /// </summary>
        public FsmErrorData ErrorData { get;} = new();

        /// <summary>
        /// Gets the list of FSM nodes associated with the error.
        /// </summary>
        public List<FsmNode> Nodes { get;} = new();
    }
}
#endif