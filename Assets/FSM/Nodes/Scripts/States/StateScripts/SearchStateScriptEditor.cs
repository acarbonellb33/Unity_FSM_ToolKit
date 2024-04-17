using UnityEditor;

// CustomEditor attribute indicates that this class provides a custom editor for SearchStateScript
[CustomEditor(typeof(SearchStateScript))]
public class SearchStateScriptEditor : Editor
{
    // This method is called when Unity's inspector GUI is being drawn for an object of type SearchStateScript
    public override void OnInspectorGUI()
    {
        // The OnInspectorGUI method is empty, because the SearchStateScript public variables are handled by the behavior script
    }
}