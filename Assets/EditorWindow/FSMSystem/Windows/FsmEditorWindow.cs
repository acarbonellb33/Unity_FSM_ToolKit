#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Windows
{
    using System;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;
    using PopupWindow = UnityEditor.PopupWindow;
    using Data.Save;
    using Utilities;
    using FSM.Nodes;
    using FSM.Utilities;
    public class FsmEditorWindow : EditorWindow
    {
        private static FsmGraphView _graphView;
        private static string _fileName = "";
        private static Label _fileNameTextField;
        private Button _saveButton;
        private Button _miniMapButton;
        private static FsmEditorWindow _window;
        private static FsmGraphSaveData _saveData;
        private static FsmHitStatePopup _hitStatePopup;

        public string initialState;
        
        private const string FsmInspectorKey = "FSMInspectorData";

        private bool _isCompiling;
        private bool _shouldClose;

        public static void OpenWithSaveData(FsmGraphSaveData saveData)
        {
            _saveData = saveData;
            _fileName = saveData.FileName;
            _hitStatePopup = new FsmHitStatePopup();
            _hitStatePopup.Initialize(saveData.HitData);

            OnHierarchyChanged();

            EditorPrefs.SetString(FsmInspectorKey, FindGameObjectWithClass<FsmGraph>().ToString());

            if (_window == null)
            {
                _window = CreateWindow<FsmEditorWindow>("FSM Graph");
            }
            else
            {
                GetWindow<FsmEditorWindow>("FSM Graph");
            }

            string assetPath = $"Assets/EditorWindow/FSMSystem/Graphs/{saveData.FileName}.asset";
            if (string.IsNullOrEmpty(assetPath)) return;
            _graphView.ClearGraph();
            FsmIOUtility.Initialize(saveData.FileName, _graphView, saveData.InitialState, saveData.HitData);
            FsmIOUtility.Load();
        }

        private static GameObject FindGameObjectWithClass<T>() where T : MonoBehaviour
        {
            T[] components = GameObject.FindObjectsOfType<T>();
            if (components == null || components.Length == 0)
            {
                return null;
            }

            return components[0].gameObject;
        }

        private void OnEnable()
        {
            AddGraphView();

            AddToolbar();

            AddStyles();

            EditorApplication.update += OnEditorUpdate;

            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;

            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
        }

        private void OnEditorUpdate()
        {
            if (_isCompiling && !EditorApplication.isCompiling)
            {
                _isCompiling = false;

                PerformActionAfterCompilation();
            }
        }

        private void PerformActionAfterCompilation()
        {
            string inspectorJson = EditorPrefs.GetString(FsmInspectorKey);
            string pattern = @"\s*\([^)]*\)";
            string result = Regex.Replace(inspectorJson, pattern, "");

            GameObject gameObject = GameObject.Find(result);

            if (_shouldClose) Close();
            gameObject.GetComponent<FsmGraph>().UpdateComponentOfGameObject();
            _shouldClose = false;
        }

        private static void OnHierarchyChanged()
        {
            // Get all root GameObjects in the scene
            GameObject[] rootGameObjects =
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

            foreach (GameObject rootGameObject in rootGameObjects)
            {
                EnsureIDGeneratorRecursive(rootGameObject);
            }
        }

        private static void EnsureIDGeneratorRecursive(GameObject gameObject)
        {
            if (gameObject.GetComponent<IDGenerator>() == null)
            {
                // Add IDGenerator component if not present
                IDGenerator generator = gameObject.AddComponent<IDGenerator>();
                generator.GetUniqueID();
                generator.hideFlags = HideFlags.HideInInspector;
            }

            // Recursively check child GameObjects
            foreach (Transform child in gameObject.transform)
            {
                EnsureIDGeneratorRecursive(child.gameObject);
            }
        }

        private void AddGraphView()
        {
            _graphView = new FsmGraphView(this);
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("FSMSystem/FSMVariables.uss");
        }

        private void AddToolbar()
        {
            var toolbar = new Toolbar();
            var box = new Box();

            _fileNameTextField = FsmElementUtility.CreateLabel("File Name: " + _fileName);

            _saveButton = FsmElementUtility.CreateButton("Save", Save);

            var reloadButton = FsmElementUtility.CreateButton("Reload", Reload);
            var clearButton = FsmElementUtility.CreateButton("Clear", Clear);
            _miniMapButton = FsmElementUtility.CreateButton("MiniMap", ToggleMiniMap);

            var animatorField = new ObjectField
            {
                label = "Animator",
                objectType = typeof(Animator)
            };
            animatorField.RegisterValueChangedCallback(_ =>
            {
                //_animator = evt.newValue as Animator;
                // Handle changes to the animator here
            });
            animatorField.AddToClassList("fsm-toolbar__animator-field");

            Button hitStateButton = null;
            hitStateButton = FsmElementUtility.CreateButton("Hit State", () => OpenHitPopup(hitStateButton));
            hitStateButton.AddToClassList("Button--hit-state");

            toolbar.Add(_fileNameTextField);
            toolbar.Add(_saveButton);
            toolbar.Add(reloadButton);
            toolbar.Add(clearButton);
            toolbar.Add(_miniMapButton);
            toolbar.Add(animatorField);
            toolbar.Add(hitStateButton);


            toolbar.AddStyleSheets("FSMSystem/FSMToolbarStyle.uss");
            rootVisualElement.Add(toolbar);
            box.AddStyleSheets("FSMSystem/FSMToolbarStyle.uss");
            rootVisualElement.Add(box);
        }

        #region Toolbar Actions

        private void Save()
        {
            if (String.IsNullOrEmpty(initialState))
            {
                initialState = _saveData.InitialState;
            }

            if (String.IsNullOrEmpty(initialState))
            {
                EditorUtility.DisplayDialog(
                    "Invalid Initial State!",
                    "Please select a valid initial state.",
                    "OK"
                );
                return;
            }

            _isCompiling = true;

            FsmHitSaveData hitData = new FsmHitSaveData();
            hitData.Initialize(_hitStatePopup.IsHitStateEnabled(), _hitStatePopup.GetTimeToWait(),
                _hitStatePopup.CanDie());

            FsmIOUtility.Initialize(_fileName, _graphView, initialState, hitData);
            if (FsmIOUtility.Save())
            {
                _shouldClose = true;
                FsmEnemyStateMachineEditor.GenerateScript(_saveData);
            }
        }

        private void Clear()
        {
            _graphView.ClearGraph();
        }

        private void Reload()
        {
            Clear();

            FsmIOUtility.Initialize(_saveData.FileName, _graphView, initialState, _saveData.HitData);
            FsmIOUtility.Load();
        }

        private void ToggleMiniMap()
        {
            _graphView.ToggleMiniMap();
            _miniMapButton.ToggleInClassList("fsm-toolbar__button__selected");
        }

        private void OpenHitPopup(VisualElement buttonElement)
        {
            Rect buttonRect = buttonElement.worldBound;
            PopupWindow.Show(new Rect(buttonRect.x - 187.5f, buttonRect.y - 77.5f, 250, 100), _hitStatePopup);
        }

        #endregion

        private void OnDestroy()
        {
            if(_shouldClose) return;
            // Prompt the user to save changes before closing
            int option = EditorUtility.DisplayDialogComplex("Save Changes",
                "Do you want to save changes before closing?",
                "Save", "Discard", "Cancel");

            switch (option)
            {
                case 0: // Save
                    Save();
                    break;
                case 1: // Discard
                    // Don't save changes, just close the window
                    break;
                case 2: // Cancel
                    // User canceled, don't close the window
                    // To prevent closing, we simply don't call the base OnDestroy method
                    return;
            }
        }

        #region Utilities

        public void EnableSaving()
        {
            _saveButton.SetEnabled(true);
            //_generateScriptButton.SetEnabled(true);
        }

        public void DisableSaving()
        {
            _saveButton.SetEnabled(false);
            //_generateScriptButton.SetEnabled(false);
        }

        public string GetFileName()
        {
            return _fileName;
        }

        public FsmHitStatePopup GetHitStatePopup()
        {
            return _hitStatePopup;
        }

        #endregion
    }
}
#endif