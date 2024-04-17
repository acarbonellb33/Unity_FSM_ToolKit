using UnityEngine;

public class FSMNodeGroupSO : ScriptableObject
{
    [field: SerializeField] public string GroupName { get; set; }

    public void Initialize(string groupName)
    {
        GroupName = groupName;
    }
}