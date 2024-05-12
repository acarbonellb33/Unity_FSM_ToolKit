namespace EditorWindow.FSMSystem.Data.Save
{
    using System;
    using UnityEngine;

    [Serializable]
    public class FsmAnimatorSaveData
    {
        [field: SerializeField] public bool TriggerEnable { get; set; }
        [field: SerializeField] public string ParameterName { get; set; }
        [field: SerializeField] public string ParameterType { get; set; }
        [field: SerializeField] public string Value { get; set; }

        public void Initialize(bool triggerEnable, string parameterName, string parameterType, string value)
        {
            TriggerEnable = triggerEnable;
            ParameterName = parameterName;
            ParameterType = parameterType;
            Value = value;
        }
    }
}