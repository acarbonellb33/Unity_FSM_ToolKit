#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Windows
{
    using System;
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;
    using PopupWindow = UnityEditor.PopupWindow;
    using Data.Save;
    using Utilities;
    using FSM.Nodes;
    using FSM.Utilities;
    /// <summary>
    /// Represents an editor window for managing finite state machines. This window contains a graph view for creating and editing FSMs. The window also contains a toolbar with buttons for saving, reloading, clearing, and toggling the minimap.
    /// </summary>
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

        private bool _isCompiling;
        private bool _shouldClose;
        /// <summary>
        /// Opens the FSM editor window with the provided save data.
        /// </summary>
        /// <param name="saveData">The save data containing information about the FSM graph.</param>
        public static void OpenWithSaveData(FsmGraphSaveData saveData)
        {
            _saveData = saveData;
            _fileName = saveData.FileName;
            _hitStatePopup = new FsmHitStatePopup();
            _hitStatePopup.Initialize(saveData.HitData);

            OnHierarchyChanged();

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
            EditorPrefs.SetBool("EnableHitState", _saveData.HitData.HitEnable);
            FsmIOUtility.Load();
        }
        // Called when the window is enabled
        private void OnEnable()
        {
            AddGraphView();

            AddToolbar();

            AddStyles();

            EditorApplication.update += OnEditorUpdate;

            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }
        // Called when the window is disabled
        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;

            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
        }
        // Called every frame to check if the script is compiling
        private void OnEditorUpdate()
        {
            if (_isCompiling && !EditorApplication.isCompiling)
            {
                _isCompiling = false;

                PerformActionAfterCompilation();
            }
        }
        // Called after the script has been compiled
        private void PerformActionAfterCompilation()
        {
            var gameObject = Selection.activeGameObject;

            if (_shouldClose) Close();
            gameObject.GetComponent<FsmGraph>().UpdateComponentOfGameObject();
            _shouldClose = false;
        }
        // Ensures that all GameObjects in the scene have an IDGenerator component
        private static void OnHierarchyChanged()
        {
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
            }

            // Recursively check child GameObjects
            foreach (Transform child in gameObject.transform)
            {
                EnsureIDGeneratorRecursive(child.gameObject);
            }
        }
        // Adds the toolbar to the window
        private void AddToolbar()
        {
            var toolbar = new Toolbar();
            var box = new Box();

            _fileNameTextField = FsmElementUtility.CreateLabel("File Name: " + _fileName);

            _saveButton = FsmElementUtility.CreateButton("Save", Save);

            var reloadButton = FsmElementUtility.CreateButton("Reload", Reload);
            var clearButton = FsmElementUtility.CreateButton("Clear", Clear);
            _miniMapButton = FsmElementUtility.CreateButton("MiniMap", ToggleMiniMap);

            Button hitStateButton = null;
            hitStateButton = FsmElementUtility.CreateButton("Hit State", () => OpenHitPopup(hitStateButton));
            hitStateButton.AddToClassList("Button--hit-state");

            toolbar.Add(_fileNameTextField);
            toolbar.Add(_saveButton);
            toolbar.Add(reloadButton);
            toolbar.Add(clearButton);
            toolbar.Add(_miniMapButton);
            toolbar.Add(hitStateButton);


            toolbar.AddStyleSheets("FSMSystem/FSMToolbarStyle.uss");
            rootVisualElement.Add(toolbar);
            box.AddStyleSheets("FSMSystem/FSMToolbarStyle.uss");
            rootVisualElement.Add(box);
        }
        // Called when the window is closed to override the default behavior
        private void OnDestroy()
        {
            if(_shouldClose) return;
            // Prompt the user to save changes before closing
            var option = EditorUtility.DisplayDialog("Save Changes",
                "Do you want to save changes before closing?",
                "Save", "Discard");

            if (option) Save();
        }
        
        #region Toolbar Actions
        //Event handlers for toolbar buttons
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
            EditorPrefs.SetBool("EnableHitState", _saveData.HitData.HitEnable);
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
        
        #region Utilities
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
        public void EnableSaving()
        {
            _saveButton.SetEnabled(true);
        }
        public void DisableSaving()
        {
            _saveButton.SetEnabled(false);
        }
        public FsmHitStatePopup GetHitStatePopup()
        {
            return _hitStatePopup;
        }
        #endregion
    }
}
#endif