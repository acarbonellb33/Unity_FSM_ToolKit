namespace FSM.Nodes.ScriptableObjects
{
    using UnityEngine;

    public class FsmNodeGroupSo : ScriptableObject
    {
        [field: SerializeField] public string GroupName { get; set; }

        public void Initialize(string groupName)
        {
            GroupName = groupName;
        }
    }
}