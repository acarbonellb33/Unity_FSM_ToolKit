using UnityEditor;

// CustomEditor attribute indicates that this class provides a custom editor for HearingConditionScript
[CustomEditor(typeof(HearingConditionScript))]
public class HearingConditionScriptEditor : Editor
{
    // This method is called when Unity's inspector GUI is being drawn for an object of type HearingConditionScript
    public override void OnInspectorGUI()
    {
        // The OnInspectorGUI method is empty, because the HearingConditionScript public variables are handled by the behavior script
    }
}

