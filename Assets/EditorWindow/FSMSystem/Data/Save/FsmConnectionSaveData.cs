namespace EditorWindow.FSMSystem.Data.Save
{
    using System;
    using UnityEngine;
    [Serializable]
    public class FsmConnectionSaveData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public string NodeId { get; set; }
    }
}
