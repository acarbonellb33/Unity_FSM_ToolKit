using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMNodeErrorData
{
    public FSMErrorData ErrorData { get; set; }
    public List<FSMNode> Nodes { get; set; }

    public FSMNodeErrorData()
    {
        ErrorData = new FSMErrorData();
        Nodes = new List<FSMNode>();
    }
}
