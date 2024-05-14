namespace EditorWindow.FSMSystem.Data.Save
{
    using System;
    using UnityEngine;
    [Serializable]
    public class FsmHitNodeSaveData
    {
        [field: SerializeField] public bool HasHitOverride { get; set; }
        [field: SerializeField] public bool CanGetHit { get; set; }
        [field: SerializeField] public float TimeToWait { get; set; }
        [field: SerializeField] public bool CanDie { get; set; }

        public void Initialize(bool hasHitOverride, bool canGetHit, float timeToWait, bool canDie)
        {
            HasHitOverride = hasHitOverride;
            CanGetHit = canGetHit;
            TimeToWait = timeToWait;
            CanDie = canDie;
        }
    }
}