using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class FSMGroupSaveData
{
    [field: SerializeField] public string Id { get; set; }
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public Vector2 Position { get; set; }
}
