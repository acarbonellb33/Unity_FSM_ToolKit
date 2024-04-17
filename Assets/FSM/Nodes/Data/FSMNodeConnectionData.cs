using System;
using UnityEngine;

[Serializable]
public class FSMNodeConnectionData
{
    [field: SerializeField] public string Text { get; set; }
    [field: SerializeField] public FSMNodeSO NextNode { get; set; }
}

