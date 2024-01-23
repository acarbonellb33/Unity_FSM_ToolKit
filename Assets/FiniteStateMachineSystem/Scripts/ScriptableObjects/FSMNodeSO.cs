using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMNodeSO : ScriptableObject
{
    [field: SerializeField] public string NodeName { get; set; }
    [field: SerializeField] [field: TextArea()]public string Text { get; set; }
    [field: SerializeField] public List<FSMNodeConnectionData> Connections { get; set; }
    [field: SerializeField] public FSMDialogueType NodeType { get; set; }
    [field: SerializeField] public bool IsStartingNode { get; set; }
    
    public void Initialize(string nodeName, string text, List<FSMNodeConnectionData> connections, FSMDialogueType nodeType, bool isStartingNode)
    {
        NodeName = nodeName;
        Text = text;
        Connections = connections;
        NodeType = nodeType;
        IsStartingNode = isStartingNode;
    }
}
