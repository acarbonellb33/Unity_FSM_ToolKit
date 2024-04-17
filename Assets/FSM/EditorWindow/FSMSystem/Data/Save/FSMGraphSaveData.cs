using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FSMGraphSaveData : ScriptableObject
{
    [field: SerializeField] public string FileName { get; set; }
    [field: SerializeField] public string InitialState { get; set; }
    [field: SerializeField] public List<FSMNodeSaveData> Nodes { get; set; }
    [field: SerializeField] public List<FSMGroupSaveData> Groups { get; set; }
    [field: SerializeField] public List<string> OldGroupedNames { get; set; }
    [field: SerializeField] public List<string> OldUngroupedNames { get; set; }
    [field: SerializeField] public SerializableDictionary<string, List<string>> OldGroupedNodeNames { get; set; }
    [field: SerializeField] public string GameObject { get; set; }
    
    public void Initialize(string fileName, string initialState)
    {
        FileName = fileName;
        InitialState = initialState;
        Nodes = new List<FSMNodeSaveData>();
        Groups = new List<FSMGroupSaveData>();
    }
}
