namespace FSM.Nodes.ScriptableObjects
{
    using System.Collections.Generic;
    using UnityEngine;
    using Enumerations;
    using Data;
    public class FsmNodeSo : ScriptableObject
    {
        [field: SerializeField] public string NodeName { get; set; }
        [field: SerializeField] public List<FsmNodeConnectionData> Connections { get; set; }
        [field: SerializeField] public FsmNodeType NodeType { get; set; }
        [field: SerializeField] public string DataObject { get; set; }

        public void Initialize(string nodeName, string text, List<FsmNodeConnectionData> connections,
            FsmNodeType nodeType,
            string dataObject)
        {
            NodeName = nodeName;
            Connections = connections;
            NodeType = nodeType;
            DataObject = dataObject;
        }
    }
}