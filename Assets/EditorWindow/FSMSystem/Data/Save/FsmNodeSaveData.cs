namespace EditorWindow.FSMSystem.Data.Save
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using FSM.Enumerations;
    [Serializable]
    public class FsmNodeSaveData
    {
        [field: SerializeField] public string Id { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public List<FsmConnectionSaveData> Connections { get; set; }
        [field: SerializeField] public FsmNodeType NodeType { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }
        [field: SerializeField] public FsmAnimatorSaveData AnimatorSaveData { get; set; }
        [field: SerializeField] public string DataObject { get; set; }
    }
}