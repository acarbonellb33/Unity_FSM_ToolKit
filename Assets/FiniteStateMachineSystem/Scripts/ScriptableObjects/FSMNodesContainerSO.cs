using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMNodesContainerSO : ScriptableObject
{
    [field: SerializeField] public string FileName { get; set; }
    [field: SerializeField] public SerializableDictionary<FSMNodeGroupSO, List<FSMNodeSO>> GroupedNodes { get; set; }
    [field: SerializeField] public List<FSMNodeSO> UngroupedNodes { get; set; }
    
    public void Initialize(string fileName)
    {
        FileName = fileName;
        GroupedNodes = new SerializableDictionary<FSMNodeGroupSO, List<FSMNodeSO>>();
        UngroupedNodes = new List<FSMNodeSO>();
    }
}
