namespace FSM.Nodes.Data
{
    using System;
    using UnityEngine;
    using ScriptableObjects;
    /// <summary>
    /// Data class representing a connection between FSM nodes.
    /// </summary>
    [Serializable]
    public class FsmNodeConnectionData
    {
        /// <summary>
        /// The text associated with the connection.
        /// </summary>
        [field: SerializeField] public string Text { get; set; }
        /// <summary>
        /// The next node connected to this connection.
        /// </summary>
        [field: SerializeField] public FsmNodeSo NextNode { get; set; }
    }
}