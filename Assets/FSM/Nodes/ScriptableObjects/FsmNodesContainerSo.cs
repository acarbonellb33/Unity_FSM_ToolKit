namespace FSM.Nodes.ScriptableObjects
{
    using System.Collections.Generic;
    using UnityEngine;
    using Utilities;
    public class FsmNodesContainerSo : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public List<FsmNodeSo> UngroupedNodes { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;
            UngroupedNodes = new List<FsmNodeSo>();
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