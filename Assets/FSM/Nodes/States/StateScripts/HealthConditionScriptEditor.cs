using UnityEditor;

// CustomEditor attribute indicates that this class provides a custom editor for HealthConditionScript
[CustomEditor(typeof(HealthConditionScript))]
public class HealthConditionScriptEditor : Editor
{
    // This method is called when Unity's inspector GUI is being drawn for an object of type HealthConditionScript
    public override void OnInspectorGUI()
    {
        // The OnInspectorGUI method is empty, because the HealthConditionScript public variables are handled by the behavior script
    }
}
