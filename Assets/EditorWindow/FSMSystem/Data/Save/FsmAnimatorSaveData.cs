namespace EditorWindow.FSMSystem.Data.Save
{
    using System;
    using UnityEngine;
    
    /// <summary>
    /// Represents save data for FSM Animator parameters.
    /// </summary>
    [Serializable]
    public class FsmAnimatorSaveData
    {
        /// <summary>
        /// Gets or sets a value indicating whether the trigger for setting animations is enabled.
        /// </summary>
        [field: SerializeField] public bool AnimationTrigger { get; set; }
        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        [field: SerializeField] public string ParameterName { get; set; }
        /// <summary>
        /// Gets or sets the type of the parameter.
        /// </summary>
        [field: SerializeField] public string ParameterType { get; set; }
        /// <summary>
        /// Gets or sets the value of the parameter.
        /// </summary>
        [field: SerializeField] public string Value { get; set; }
        /// <summary>
        /// Initializes the save data with specified parameters.
        /// </summary>
        /// <param name="triggerEnable">Whether the trigger for setting animations is enabled.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="parameterType">The type of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        public void Initialize(bool triggerEnable, string parameterName, string parameterType, string value)
        {
            AnimationTrigger = triggerEnable;
            ParameterName = parameterName;
            ParameterType = parameterType;
            Value = value;
        }
    }
}