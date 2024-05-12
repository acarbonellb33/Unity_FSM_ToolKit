#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Windows
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Data.Save;
    using Elements;
    using Utilities;
    public class CreateScriptableObjectWindow : EditorWindow
    {
        private string _savePath;

        [MenuItem("Window/FSM/FSM Graph Creator")]
        public static void ShowWindow()
        {
            GetWindow<CreateScriptableObjectWindow>("Create Scriptable Object");
        }

        private void OnEnable()
        {
            AddUIElements();
            AddStyles();
        }

        private void AddUIElements()
        {
            Image image = new Image();
            image.image = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/EditorWindow/FSMSystem/Textures/logo_ai.png",
                typeof(Texture2D));
            Label label = FSMElementUtility.CreateLabel("Customizable AI Setup");

            TextField textArea = FSMElementUtility.CreateTextArea(
                "This window will create a new FSM Graph scriptable object. " +
                "You can customize the name and when pressing saving, the scriptable object will be created and saved at a specified path." +
                "The path is set to Assets/EditorWindow/FSMSystem/Graphs by default.");

            TextField textArea2 = FSMElementUtility.CreateTextArea(
                "Once the scriptable object is created, you have to select your enemy GameObject and add the FSMGraph component. " +
                "Then, you can drag and drop the created scriptable object into the FSMGraph component or select directly your created " +
                "scriptable object from the DataContainer field.");
            TextField textField = FSMElementUtility.CreateTextField(null, "AI Graph Name");
            _savePath = "Assets/EditorWindow/FSMSystem/Graphs";

            Button createButton = FSMElementUtility.CreateButton("Setup AI Graph", () =>
            {
                // Create the scriptable object instance
                FSMGraphSaveData newScriptableObject = ScriptableObject.CreateInstance<FSMGraphSaveData>();

                // Set its name using the provided name
                newScriptableObject.Initialize(textField.value, "", new FSMHitSaveData());

                FSMInitialNode node = new FSMInitialNode();
                node.Initialize("Initial State", null, new Vector2(100, 100));

                FSMNodeSaveData nodeSaveData = new FSMNodeSaveData()
                {
                    Id = node.Id,
                    Name = node.StateName,
                    Connections = node.Choices,
                    NodeType = node.NodeType,
                    Position = new Vector2(100, 100),
                    DataObject = null,
                };
                newScriptableObject.Nodes.Add(nodeSaveData);

                string fullPath = $"{_savePath}/{textField.value}.asset";
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