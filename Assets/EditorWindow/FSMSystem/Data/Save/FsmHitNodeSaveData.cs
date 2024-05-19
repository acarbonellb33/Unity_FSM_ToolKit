namespace EditorWindow.FSMSystem.Data.Save
{
    using System;
    using UnityEngine;
    /// <summary>
    /// Represents save data for hit events in the nodes of the FSM graphs.
    /// </summary>
    [Serializable]
    public class FsmHitNodeSaveData
    {
        /// <summary>
        /// Gets or sets a value indicating whether the hit node has the hit override option enabled.
        /// </summary>
        [field: SerializeField] public bool HasHitOverride { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the enemy can get hit.
        /// </summary>
        [field: SerializeField] public bool CanGetHit { get; set; }
        /// <summary>
        /// Gets or sets the time to wait for the enemy to be stunned.
        /// </summary>
        [field: SerializeField] public float TimeToWait { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the enemy can die.
        /// </summary>
        [field: SerializeField] public bool CanDie { get; set; }
        /// <summary>
        /// Initializes the hit save data with specified parameters.
        /// </summary>
        /// <param name="hasHitOverride">Whether the hit node has the hit override option enabled.</param>
        /// <param name="canGetHit">Whether the enemy can get hit.</param>
        /// <param name="timeToWait">The time to wait for the enemy to be stunned.</param>
        /// <param name="canDie">Whether the enemy can die.</param>
        public void Initialize(bool hasHitOverride, bool canGetHit, float timeToWait, bool canDie)
        {
            HasHitOverride = hasHitOverride;
            CanGetHit = canGetHit;
            TimeToWait = timeToWait;
            CanDie = canDie;
        }
    }
}