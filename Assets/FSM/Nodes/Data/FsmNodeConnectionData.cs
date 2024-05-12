namespace FSM.Nodes.Data
{
    using System;
    using UnityEngine;
    using ScriptableObjects;
    [Serializable]
    public class FsmNodeConnectionData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public FsmNodeSo NextNode { get; set; }
    }
}