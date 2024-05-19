namespace EditorWindow.FSMSystem.Data.Save
{
    using System;
    using UnityEngine;
    /// <summary>
    /// Represents save data for FSM connections.
    /// </summary>
    [Serializable]
    public class FsmConnectionSaveData
    {
        /// <summary>
        /// Gets or sets the text associated with the connection.
        /// </summary>
        [field: SerializeField] public string Text { get; set; }
        /// <summary>
        /// Gets or sets the ID of the connected node.
        /// </summary>
        [field: SerializeField] public string NodeId { get; set; }
    }
}
