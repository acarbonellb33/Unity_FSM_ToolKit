#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Data.Error
{
    using System.Collections.Generic;
    using Elements;
    public class FSMNodeErrorData
    {
        public FSMErrorData ErrorData { get; set; }
        public List<FSMNode> Nodes { get; set; }

        public FSMNodeErrorData()
        {
            ErrorData = new FSMErrorData();
            Nodes = new List<FSMNode>();
        }
    }
}
#endif