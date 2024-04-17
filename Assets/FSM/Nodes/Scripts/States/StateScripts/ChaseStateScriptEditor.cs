using UnityEditor;

// CustomEditor attribute indicates that this class provides a custom editor for ChaseStateScript
[CustomEditor(typeof(ChaseStateScript))]
public class ChaseStateScriptEditor : Editor
{
    // This method is called when Unity's inspector GUI is being drawn for an object of type ChaseStateScript
    public override void OnInspectorGUI()
    {
        // The OnInspectorGUI method is empty, because the ChaseStateScript public variables are handled by the behavior script
    }
}

