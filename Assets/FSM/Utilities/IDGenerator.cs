namespace FSM.Utilities
{
    using System;
    using UnityEngine;
    /// <summary>
    /// Class for generating unique IDs for GameObjects.
    /// </summary>
    public class IDGenerator : MonoBehaviour
    {
        public string uniqueID;

        /// <summary>
        /// Call this method to get the unique ID. If the unique ID is not generated yet, it will generate it. Else, it will return the existing unique ID.
        /// </summary>
        /// <returns>The generated unique ID.</returns>
        public string GetUniqueID()
        {
            if (string.IsNullOrEmpty(uniqueID))
            {
                uniqueID = Guid.NewGuid().ToString();
                hideFlags = HideFlags.HideInInspector;
            }
            return uniqueID;
        }
        /// <summary>
        /// Call this method to generate a new unique ID.
        /// </summary>
        /// <returns></returns>
        public string GenerateNewID()
        {
            uniqueID = Guid.NewGuid().ToString();
            hideFlags = HideFlags.HideInInspector;
            return uniqueID;
        }
    }
}