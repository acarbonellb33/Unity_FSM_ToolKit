namespace EditorWindow.FSMSystem.Data.Save
{
    using System.Collections.Generic;
    using UnityEngine;
    public class FSMGraphSaveData : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public string InitialState { get; set; }
        [field: SerializeField] public List<FSMNodeSaveData> Nodes { get; set; }
        [field: SerializeField] public List<string> OldUngroupedNames { get; set; }
        [field: SerializeField] public string GameObject { get; set; }
        [field: SerializeField] public FSMHitSaveData HitData { get; set; }

        public void Initialize(string fileName, string initialState, FSMHitSaveData hitSaveData)
        {
            FileName = fileName;
            InitialState = initialState;
            HitData = hitSaveData;
            Nodes = new List<FSMNodeSaveData>();
        }
    }
}