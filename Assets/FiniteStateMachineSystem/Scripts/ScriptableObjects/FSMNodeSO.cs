using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMNodeSO : ScriptableObject
{
    [field: SerializeField] public string NodeName { get; set; }
    [field: SerializeField] [field: TextArea()]public string Text { get; set; }
    [field: SerializeField] public List<FSMNodeConnectionData> Connections { get; set; }
    [field: SerializeField] public FSMNodeType NodeType { get; set; }
    [field: SerializeField] public State ScriptableObject { get; set; }

    public void Initialize(string nodeName, string text, List<FSMNodeConnectionData> connections, FSMNodeType nodeType, State scriptableObject)
    {
        NodeName = nodeName;
        Text = text;
        Connections = connections;
        NodeType = nodeType;
        ScriptableObject = scriptableObject;
    }
}
