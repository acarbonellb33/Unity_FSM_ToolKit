#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Windows
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Data.Save;
    using Elements;
    using Utilities;
    using System.Collections.Generic;
    using Inspectors;
    using UnityEditor.Animations;
    /// <summary>
    /// Editor window for creating FSM Graph scriptable objects and configuring enemy setup.
    /// </summary>
    public class FsmCreateScriptableObjectWindow : EditorWindow
    {
        private string _savePath;
        private VisualElement _lastClickedButton;
        
        /// <summary>
        /// Animation clips to import
        /// </summary>
        private readonly List<AnimationClip> _selectedAnimations = new();
        private static List<AnimationClip> _allAnimations = new();
        private GameObject _characterModel;
        private string _characterGameObjectName;
        
        [MenuItem("Window/FSM/FSM Graph")]
        public static void ShowWindow()
        {
            FsmCreateScriptableObjectWindow window = GetWindow<FsmCreateScriptableObjectWindow>("Create Scriptable Object");
            window.minSize = new Vector2(400, 800);
        }

        private void OnEnable()
        {
            _selectedAnimations.Clear();
            _allAnimations.Clear();
            _allAnimations = new List<AnimationClip>(Resources.FindObjectsOfTypeAll<AnimationClip>());
            AddUIElements();
            AddStyles();
        }

        private void AddUIElements()
        {
            var image = new Image
            {
                image = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/EditorWindow/FSMSystem/Textures/logo_ai.png",
                    typeof(Texture2D)),
            };
            var label = FsmElementUtility.CreateLabel("Customizable AI Setup");
            
            var createGraphButton = new Button { text = "Create FSM Graph" };
            createGraphButton.AddToClassList("button");
            createGraphButton.clicked += () =>
            {
                if (_lastClickedButton != createGraphButton)
                {
                    _lastClickedButton?.RemoveFromClassList("selected");
                    _lastClickedButton = createGraphButton;
                    _lastClickedButton.AddToClassList("selected");
                    if(rootVisualElement.childCount > 4)rootVisualElement.RemoveAt(4);
                    CreateScriptableObjectFsmGraph();
                }
            };

            var createEnemyButton = new Button { text = "Create Enemy" };
            createEnemyButton.AddToClassList("button");
            createEnemyButton.clicked += () =>
            {
                if (_lastClickedButton != createEnemyButton)
                {
                    _lastClickedButton?.RemoveFromClassList("selected");
                    _lastClickedButton = createEnemyButton;
                    _lastClickedButton.AddToClassList("selected");
                    if(rootVisualElement.childCount > 4)rootVisualElement.RemoveAt(4);
                    CreateScriptableObjectAndEnemy();
                }
            };

            
            label.AddToClassList("customLabelTitle");

            rootVisualElement.Add(image);
            rootVisualElement.Add(label);
            rootVisualElement.Add(createGraphButton);
            rootVisualElement.Add(createEnemyButton);
        }

        private void CreateScriptableObjectFsmGraph()
        {
            var container = new VisualElement();
            
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
                var newScriptableObject = CreateInstance<FsmGraphSaveData>();

                // Set its name using the provided name
                newScriptableObject.Initialize(textField.value, "", new FsmHitSaveData());

                var node = new FsmInitialNode();
                node.Initialize("Initial State", null, new Vector2(100, 100));

                var nodeSaveData = new FsmNodeSaveData()
                {
                    Id = node.Id,
                    Name = node.StateName,
                    Connections = node.Connections,
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
            textArea.AddToClassList("customLabelClass");
            textArea2.AddToClassList("customLabelClass");
            textField.AddToClassList("customLabelFilename");
            
            container.Add(textArea);
            container.Add(textArea2);
            container.Add(textField);
            container.Add(createButton);

            rootVisualElement.Add(container);
        }

        private void CreateScriptableObjectAndEnemy()
        {
            var container = new VisualElement();

            var textArea = FsmElementUtility.CreateTextArea(
                "Here you can custom your enemy. " +
                "You can choose to drag your own model or use the default one, the character will be added to the scene for you to customize later.");

            var textField = FsmElementUtility.CreateTextField(_characterGameObjectName, "Character Name");
            textField.RegisterValueChangedCallback(evt => _characterGameObjectName = evt.newValue);

            var characterModel = FsmElementUtility.CreateObjectField("FBX", _characterModel, evt => _characterModel = (GameObject)evt.newValue);
            
            var labelAnimations = FsmElementUtility.CreateLabel("Animations to Import");
            
            container.Add(textArea);
            container.Add(textField);
            container.Add(characterModel);
            container.Add(labelAnimations);
            
            foreach (AnimationClip clip in _allAnimations)
            {
                var toggle = FsmElementUtility.CreateToggle(clip.name, _selectedAnimations.Contains(clip), evt =>
                {
                    if (evt.newValue)
                    {
                        _selectedAnimations.Add(clip);
                    }
                    else
                    {
                        _selectedAnimations.Remove(clip);
                    }
                });
                container.Add(toggle);
            }

            var button = FsmElementUtility.CreateButton("Import Character", ImportCharacter);
            
            textArea.AddToClassList("customLabelClass");
            textField.AddToClassList("customLabelFilename");
            characterModel.AddToClassList("customObjectField");
            labelAnimations.AddToClassList("customLabelFilename");
            
            container.Add(button);
            rootVisualElement.Add(container);
        }

        private void ImportCharacter()
        {
            if (string.IsNullOrEmpty(_characterGameObjectName))
            {
                Debug.LogError("Character GameObject Name is empty!");
                return;
            }

            if (_characterModel == null)
            {
                Debug.LogError("Character Model is not assigned!");
                return;
            }

            // Instantiate the character model
            GameObject instantiatedCharacter = Instantiate(_characterModel);
            instantiatedCharacter.name = _characterGameObjectName;
            instantiatedCharacter.tag = "Enemy";
            
            GameObject childGameObject = new GameObject("FocalPoint");
            childGameObject.transform.SetParent(instantiatedCharacter.transform);
            childGameObject.transform.SetSiblingIndex(0);
            
            // Load the prefab from the AssetDatabase
            if (!string.IsNullOrEmpty("Assets/Prefabs/EnemyUI.prefab"))
            {
                GameObject childPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/EnemyUI.prefab");
                if (childPrefab != null)
                {
                    // Instantiate the prefab as a child of the instantiated character
                    GameObject childPrefabInstance = Instantiate(childPrefab, instantiatedCharacter.transform);
                    childPrefabInstance.name = childPrefab.name;
                    childPrefabInstance.transform.SetParent(instantiatedCharacter.transform);
                }
            }

            // Create Animation Controller
            Animator animator;
            if(instantiatedCharacter.GetComponent<Animator>() == null) animator = instantiatedCharacter.AddComponent<Animator>();
            else animator = instantiatedCharacter.GetComponent<Animator>();
            
            AnimatorController controller = CreateAnimatorController();

            // Add animations to the controller
            foreach (AnimationClip clip in _selectedAnimations)
            {
                AddAnimationToController(controller, clip);
            }

            // Set the default state to the first animation
            animator.runtimeAnimatorController = controller;
            animator.Play(_selectedAnimations[0].name);
            
            var component = instantiatedCharacter.AddComponent<FsmGraph>();
            component.hideFlags = HideFlags.None;
            Close();
        }

        private AnimatorController CreateAnimatorController()
        {
            FsmIOUtility.CreateFolder("Assets/Characters", _characterGameObjectName);
            AnimatorController controller =
                AnimatorController.CreateAnimatorControllerAtPath(
                    $"Assets/Characters/{_characterGameObjectName}/CharacterController.controller");
            return controller;
        }

        private void AddAnimationToController(AnimatorController controller, AnimationClip clip)
        {
            AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;
            AnimatorState state = rootStateMachine.AddState(clip.name);
            state.motion = clip;

            // Add a trigger for this animation
            controller.AddParameter(clip.name, AnimatorControllerParameterType.Trigger);

            // Set the default transition to this state
            AnimatorStateTransition defaultTransition = rootStateMachine.AddAnyStateTransition(state);
            defaultTransition.AddCondition(AnimatorConditionMode.If, 0, clip.name);
        }

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("FSMSystem/FSMInitialWindow.uss");
        }
    }
}
#endif