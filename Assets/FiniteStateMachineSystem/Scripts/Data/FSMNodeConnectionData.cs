using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class FSMNodeConnectionData
{
    [field: SerializeField] public string Text { get; set; }
    [field: SerializeField] public FSMNodeSO NextNode { get; set; }
}
