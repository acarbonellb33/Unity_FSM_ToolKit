using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMNodeSO : ScriptableObject
{
    [field: SerializeField] public string NodeName { get; set; }
    [field: SerializeField] public List<FSMNodeConnectionData> Connections { get; set; }
    [field: SerializeField] public FSMNodeType NodeType { get; set; }
    [field: SerializeField] public string DataObject { get; set; }

    public void Initialize(string nodeName, string text, List<FSMNodeConnectionData> connections, FSMNodeType nodeType, string dataObject)
    {
        NodeName = nodeName;
        Connections = connections;
        NodeType = nodeType;
        DataObject = dataObject;
    }
}
