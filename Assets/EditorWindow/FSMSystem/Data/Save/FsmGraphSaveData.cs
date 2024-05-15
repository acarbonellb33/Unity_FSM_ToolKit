namespace EditorWindow.FSMSystem.Data.Save
{
    using System.Collections.Generic;
    using UnityEngine;
    public class FsmGraphSaveData : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public string InitialState { get; set; }
        [field: SerializeField] public List<FsmNodeSaveData> Nodes { get; set; }
        [field: SerializeField] public List<string> OldNodeNames { get; set; }
        [field: SerializeField] public string GameObject { get; set; }
        [field: SerializeField] public FsmHitSaveData HitData { get; set; }

        public void Initialize(string fileName, string initialState, FsmHitSaveData hitSaveData)
        {
            FileName = fileName;
            InitialState = initialState;
            HitData = hitSaveData;
            Nodes = new List<FsmNodeSaveData>();
        }
    }
}