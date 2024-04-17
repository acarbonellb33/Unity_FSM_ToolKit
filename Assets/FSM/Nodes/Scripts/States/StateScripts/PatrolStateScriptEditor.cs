using UnityEditor;

// CustomEditor attribute indicates that this class provides a custom editor for PatrolStateScript
[CustomEditor(typeof(PatrolStateScript))]
public class PatrolStateScriptEditor : Editor
{
    // This method is called when Unity's inspector GUI is being drawn for an object of type PatrolStateScript
    public override void OnInspectorGUI()
    {
        // The OnInspectorGUI method is empty, because the PatrolStateScript public variables are handled by the behavior script
    }
}

