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

    public List<string> GetGroupNames()
    {
        List<string> groupedStateNames = new List<string>();

        foreach (FSMNodeGroupSO group in GroupedNodes.Keys)
        {
            groupedStateNames.Add(group.GroupName);
        }

        return groupedStateNames;
    }

    public List<string> GetGroupedStateNames(FSMNodeGroupSO dialogueGroup)
    {
        List<FSMNodeSO> groupedDialogues = GroupedNodes[dialogueGroup];
        List<string> groupedDialogueNames = new List<string>();

        foreach (FSMNodeSO groupedDialogue in groupedDialogues)
        {
            groupedDialogueNames.Add(groupedDialogue.NodeName);
        }

        return groupedDialogueNames;
    }

    public List<string> GetUngroupedStateNames()
    {
        List<string> ungroupedDialogueNames = new List<string>();
        foreach (FSMNodeSO ungroupedDialogue in UngroupedNodes)
        {
            ungroupedDialogueNames.Add(ungroupedDialogue.NodeName);
        }

        return ungroupedDialogueNames;
    }
}


