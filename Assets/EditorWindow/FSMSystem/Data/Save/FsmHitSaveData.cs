namespace EditorWindow.FSMSystem.Data.Save
{
    using System;
    using UnityEngine;
    [Serializable]
    public class FsmHitSaveData
    {
        [field: SerializeField] public bool HitEnable { get; set; }
        [field: SerializeField] public float TimeToWait { get; set; }
        [field: SerializeField] public bool CanDie { get; set; }

        public void Initialize(bool hitEnable, float timeToWait, bool canDie)
        {
            HitEnable = hitEnable;
            TimeToWait = timeToWait;
            CanDie = canDie;
        }
    }
}