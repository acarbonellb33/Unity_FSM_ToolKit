using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FSMGroupErrorData
{
    public FSMErrorData ErrorData { get; set; }
    public List<FSMGroup> Groups { get; set; }

    public FSMGroupErrorData()
    {
        ErrorData = new FSMErrorData();
        Groups = new List<FSMGroup>();
    }
}
