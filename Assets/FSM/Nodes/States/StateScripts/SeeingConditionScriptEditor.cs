using UnityEditor;

// CustomEditor attribute indicates that this class provides a custom editor for SeeingConditionScript
[CustomEditor(typeof(SeeingConditionScript))]
public class SeeingConditionScriptEditor : Editor
{
    // This method is called when Unity's inspector GUI is being drawn for an object of type SeeingConditionScript
    public override void OnInspectorGUI()
    {
        // The OnInspectorGUI method is empty, because the SeeingConditionScript public variables are handled by the behavior script
    }
}

