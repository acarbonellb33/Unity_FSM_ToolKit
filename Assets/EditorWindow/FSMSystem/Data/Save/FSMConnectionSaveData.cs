using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FSMConnectionSaveData
{
    [field: SerializeField] public string Text { get; set; }
    [field: SerializeField] public string NodeId { get; set; }
}
