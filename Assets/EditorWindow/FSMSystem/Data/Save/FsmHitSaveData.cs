namespace EditorWindow.FSMSystem.Data.Save
{
    using System;
    using UnityEngine;
    /// <summary>
    /// Represents save data for hit events in FSM graphs.
    /// </summary>
    [Serializable]
    public class FsmHitSaveData
    {
        /// <summary>
        /// Gets or sets a value indicating whether the enemy can get hit.
        /// </summary>
        [field: SerializeField] public bool HitEnable { get; set; }
        /// <summary>
        /// Gets or sets the time to wait for the enemy to be stunned.
        /// </summary>
        [field: SerializeField] public float TimeToWait { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the enemy can die.
        /// </summary>
        [field: SerializeField] public bool CanDie { get; set; }
        /// <summary>
        /// Initializes the save data with specified parameters.
        /// </summary>
        /// <param name="hitEnable">Whether the enemy can get hit.</param>
        /// <param name="timeToWait">The time to wait for the enemy to be stunned.</param>
        /// <param name="canDie">Whether the enemy can die.</param>
        public void Initialize(bool hitEnable, float timeToWait, bool canDie)
        {
            HitEnable = hitEnable;
            TimeToWait = timeToWait;
            CanDie = canDie;
        }
    }
}