#if UNITY_EDITOR
namespace FSM.Utilities
{
    using UnityEditor;
    [InitializeOnLoad]
    public class FsmTagLayerSetUp
    {
        static FsmTagLayerSetUp()
        {
            SetupTagsAndLayers();
        }

        static void SetupTagsAndLayers()
        {
            AddTag("Enemy");
            AddLayer("Grounded");
            AddLayer("Player");
        }

        static void AddTag(string tag)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(tag)) { return; }
            }

            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty newTagProp = tagsProp.GetArrayElementAtIndex(0);
            newTagProp.stringValue = tag;
            tagManager.ApplyModifiedProperties();
        }

        static void AddLayer(string layer)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layersProp = tagManager.FindProperty("layers");

            for (int i = 0; i < layersProp.arraySize; i++)
            {
                SerializedProperty l = layersProp.GetArrayElementAtIndex(i);
                if (l.stringValue.Equals(layer)) { return; }
            }

            for (int j = 8; j < layersProp.arraySize; j++)
            {
                SerializedProperty newLayerProp = layersProp.GetArrayElementAtIndex(j);
                if (string.IsNullOrEmpty(newLayerProp.stringValue))
                {
                    newLayerProp.stringValue = layer;
                    tagManager.ApplyModifiedProperties();
                    return;
                }
            }
        }
    }
}
#endif