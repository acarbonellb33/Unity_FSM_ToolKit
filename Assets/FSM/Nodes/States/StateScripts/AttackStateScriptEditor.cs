using UnityEditor;

// CustomEditor attribute indicates that this class provides a custom editor for AttackStateScript
[CustomEditor(typeof(AttackStateScript))]
public class AttackStateScriptEditor : Editor
{
    // This method is called when Unity's inspector GUI is being drawn for an object of type AttackStateScript
    public override void OnInspectorGUI()
    {
        // The OnInspectorGUI method is empty, because the AttackStateScript public variables are handled by the behavior script
    }
}
