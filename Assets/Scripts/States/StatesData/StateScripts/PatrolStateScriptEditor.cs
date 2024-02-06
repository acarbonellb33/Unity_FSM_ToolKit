using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(PatrolStateScript))]
public class PatrolStateScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Do not draw the default inspector for MyComponent
    }
}
