namespace FSM.Nodes.Data
{
    using System;
    using UnityEngine;
    using ScriptableObjects;
    [Serializable]
    public class FSMNodeConnectionData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public FSMNodeSO NextNode { get; set; }
    }
}