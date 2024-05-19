namespace EditorWindow.FSMSystem.Data.Save
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using FSM.Enumerations;
    [Serializable]
    public class FsmNodeSaveData
    {
        /// <summary>
        /// Gets or sets the ID of the node.
        /// </summary>
        [field: SerializeField] public string Id { get; set; }
        /// <summary>
        /// Gets or sets the name of the node.
        /// </summary>
        [field: SerializeField] public string Name { get; set; }
        /// <summary>
        /// Gets or sets the list of connections associated with the node.
        /// </summary>
        [field: SerializeField] public List<FsmConnectionSaveData> Connections { get; set; }
        /// <summary>
        /// Gets or sets the type of the node.
        /// </summary>
        [field: SerializeField] public FsmNodeType NodeType { get; set; }
        /// <summary>
        /// Gets or sets the position of the node.
        /// </summary>
        [field: SerializeField] public Vector2 Position { get; set; }
        /// <summary>
        /// Gets or sets the save data about the animation option selected associated with the node.
        /// </summary>
        [field: SerializeField] public FsmAnimatorSaveData AnimatorSaveData { get; set; }
        /// <summary>
        /// Gets or sets the save data for hit override values associated with the node.
        /// </summary>
        [field: SerializeField] public FsmHitNodeSaveData HitNodeSaveData { get; set; }
        /// <summary>
        /// Gets or sets the data object saved in a json associated with the node.
        /// </summary>
        [field: SerializeField] public string DataObject { get; set; }
    }
}