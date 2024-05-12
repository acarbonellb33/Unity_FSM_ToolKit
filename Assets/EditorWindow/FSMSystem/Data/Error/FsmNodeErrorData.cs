#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Data.Error
{
    using System.Collections.Generic;
    using Elements;
    public class FsmNodeErrorData
    {
        public FsmErrorData ErrorData { get;} = new();
        public List<FsmNode> Nodes { get;} = new();
    }
}
#endif