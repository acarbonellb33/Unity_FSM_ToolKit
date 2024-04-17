using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FSMNodeSaveData
{
    [field: SerializeField] public string Id { get; set; }
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public string Text { get; set; }
    [field: SerializeField] public List<FSMConnectionSaveData> Connections { get; set; }
    [field: SerializeField] public string GroupId { get; set; }
    [field: SerializeField] public FSMNodeType NodeType { get; set; }
    [field: SerializeField] public Vector2 Position { get; set; }
    [field: SerializeField] public string DataObject { get; set; }
}
