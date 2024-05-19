namespace EditorWindow.FSMSystem.Data.Save
{
    using System.Collections.Generic;
    using UnityEngine;
    /// <summary>
    /// Represents save data for FSM graphs.
    /// </summary>
    public class FsmGraphSaveData : ScriptableObject
    {
        /// <summary>
        /// Gets or sets the name of the file associated with the graph.
        /// </summary>
        [field: SerializeField] public string FileName { get; set; }
        /// <summary>
        /// Gets or sets the initial state of the graph.
        /// </summary>
        [field: SerializeField] public string InitialState { get; set; }
        /// <summary>
        /// Gets or sets the list of nodes in the graph.
        /// </summary>
        [field: SerializeField] public List<FsmNodeSaveData> Nodes { get; set; }
        /// <summary>
        /// Gets or sets the list of old node names in the graph.
        /// </summary>
        [field: SerializeField] public List<string> OldNodeNames { get; set; }
        /// <summary>
        /// Gets or sets the name of the GameObject associated with the graph.
        /// </summary>
        [field: SerializeField] public string GameObject { get; set; }
        /// <summary>
        /// Gets or sets the save data for hit events in the graph.
        /// </summary>
        [field: SerializeField] public FsmHitSaveData HitData { get; set; }
        /// <summary>
        /// Initializes the save data with specified parameters.
        /// </summary>
        /// <param name="fileName">The name of the file associated with the graph.</param>
        /// <param name="initialState">The initial state of the graph.</param>
        /// <param name="hitSaveData">The save data for hit events in the graph.</param>
        public void Initialize(string fileName, string initialState, FsmHitSaveData hitSaveData)
        {
            FileName = fileName;
            InitialState = initialState;
            HitData = hitSaveData;
            Nodes = new List<FsmNodeSaveData>();
        }
    }
}