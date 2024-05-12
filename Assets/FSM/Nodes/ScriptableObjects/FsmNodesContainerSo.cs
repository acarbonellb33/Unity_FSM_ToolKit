namespace FSM.Nodes.ScriptableObjects
{
    using System.Collections.Generic;
    using UnityEngine;
    using Utilities;
    public class FsmNodesContainerSo : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }

        [field: SerializeField]
        public SerializableDictionary<FsmNodeGroupSo, List<FsmNodeSo>> GroupedNodes { get; set; }

        [field: SerializeField] public List<FsmNodeSo> UngroupedNodes { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;
            GroupedNodes = new SerializableDictionary<FsmNodeGroupSo, List<FsmNodeSo>>();
            UngroupedNodes = new List<FsmNodeSo>();
        }

        public List<string> GetGroupNames()
        {
            List<string> groupedStateNames = new List<string>();

            foreach (FsmNodeGroupSo group in GroupedNodes.Keys)
            {
                groupedStateNames.Add(group.GroupName);
            }

            return groupedStateNames;
        }

        public List<string> GetGroupedStateNames(FsmNodeGroupSo dialogueGroup)
        {
            List<FsmNodeSo> groupedDialogues = GroupedNodes[dialogueGroup];
            List<string> groupedDialogueNames = new List<string>();

            foreach (FsmNodeSo groupedDialogue in groupedDialogues)
            {
                groupedDialogueNames.Add(groupedDialogue.NodeName);
            }

            return groupedDialogueNames;
        }

        public List<string> GetUngroupedStateNames()
        {
            List<string> ungroupedDialogueNames = new List<string>();
            foreach (FsmNodeSo ungroupedDialogue in UngroupedNodes)
            {
                ungroupedDialogueNames.Add(ungroupedDialogue.NodeName);
            }

            return ungroupedDialogueNames;
        }
    }
}