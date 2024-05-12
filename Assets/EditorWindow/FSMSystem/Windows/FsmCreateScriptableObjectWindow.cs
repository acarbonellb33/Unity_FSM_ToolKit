#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Windows
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Data.Save;
    using Elements;
    using Utilities;
    public class FsmCreateScriptableObjectWindow : EditorWindow
    {
        private string _savePath;

        [MenuItem("Window/FSM/FSM Graph Creator")]
        public static void ShowWindow()
        {
            GetWindow<FsmCreateScriptableObjectWindow>("Create Scriptable Object");
        }

        private void OnEnable()
        {
            AddUIElements();
            AddStyles();
        }

        private void AddUIElements()
        {
            var image = new Image
            {
                image = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/EditorWindow/FSMSystem/Textures/logo_ai.png",
                    typeof(Texture2D))
            };
            var label = FsmElementUtility.CreateLabel("Customizable AI Setup");

            var textArea = FsmElementUtility.CreateTextArea(
                "This window will create a new FSM Graph scriptable object. " +
                "You can customize the name and when pressing saving, the scriptable object will be created and saved at a specified path." +
                "The path is set to Assets/EditorWindow/FSMSystem/Graphs by default.");

            var textArea2 = FsmElementUtility.CreateTextArea(
                "Once the scriptable object is created, you have to select your enemy GameObject and add the FsmGraph component. " +
                "Then, you can drag and drop the created scriptable object into the FsmGraph component or select directly your created " +
                "scriptable object from the DataContainer field.");
            var textField = FsmElementUtility.CreateTextField(null, "AI Graph Name");
            _savePath = "Assets/EditorWindow/FSMSystem/Graphs";

            var createButton = FsmElementUtility.CreateButton("Setup AI Graph", () =>
            {
                // Create the scriptable object instance
                var newScriptableObject = ScriptableObject.CreateInstance<FsmGraphSaveData>();

                // Set its name using the provided name
                newScriptableObject.Initialize(textField.value, "", new FsmHitSaveData());

                var node = new FsmInitialNode();
                node.Initialize("Initial State", null, new Vector2(100, 100));

                var nodeSaveData = new FsmNodeSaveData()
                {
                    Id = node.Id,
                    Name = node.StateName,
                    Connections = node.Choices,
                    NodeType = node.NodeType,
                    Position = new Vector2(100, 100),
                    DataObject = null,
                };
                newScriptableObject.Nodes.Add(nodeSaveData);

                var fullPath = $"{_savePath}/{textField.value}.asset";
                AssetDatabase.CreateAsset(newScriptableObject, fullPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Close();
            });
            label.AddToClassList("customLabelTitle");
            textArea.AddToClassList("customLabelClass");
            textArea2.AddToClassList("customLabelClass");
            textField.AddToClassList("customLabelFilename");

            rootVisualElement.Add(image);
            rootVisualElement.Add(label);
            rootVisualElement.Add(textArea);
            rootVisualElement.Add(textArea2);
            rootVisualElement.Add(textField);
            rootVisualElement.Add(createButton);
        }

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("FSMSystem/FSMInitialWindow.uss");
        }
    }
}
#endif