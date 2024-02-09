using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class FSMEditorWindow : EditorWindow
{
    private static FSMGraphView _graphView;
    private static string _fileName = "";
    private static Label _fileNameTextField;
    private Button _saveButton;
    private Button _miniMapButton;
    public static List<string> _stateNames = new List<string>();
    private static FSMEditorWindow _window;
    private static FSMGraphSaveData _saveData;

    public string initialState;
    private static string _initialState;
    
    private const string SaveDataKey = "FSMSaveData";
    private const string FSMInspectorKey = "FSMInspectorData";
    
    private bool _isCompiling = false;
    public static void OpenWithSaveData(FSMGraphSaveData saveData)
    {
        _saveData = saveData;
        _fileName = saveData.FileName;

        PlayerPrefs.SetString(FSMInspectorKey, FindGameObjectWithClass<FSMGraph>().ToString());
        
        if(_window == null)
        {
            _window = CreateWindow<FSMEditorWindow>("FSM Graph");
        }
        else
        {
            GetWindow<FSMEditorWindow>("FSM Graph");
        }
        string assetPath = $"Assets/EditorWindow/FSMSystem/Graphs/{saveData.FileName}.asset";
        if (string.IsNullOrEmpty(assetPath))return;
        _graphView.ClearGraph();
        _stateNames.Clear();
        FSMIOUtility.Initialize(saveData.FileName, _graphView, "");
        FSMIOUtility.Load();
    }
    public static GameObject FindGameObjectWithClass<T>() where T : MonoBehaviour
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
    }
    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
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
        string inspectorJson = PlayerPrefs.GetString(FSMInspectorKey);
        string pattern = @"\s*\([^)]*\)";
        string result = Regex.Replace(inspectorJson, pattern, "");

        GameObject gameObject = GameObject.Find(result);

        Close();
        gameObject.GetComponent<FSMGraph>().UpdateComponentOfGameObject();
    }
    
    private void AddGraphView()
    {
        _graphView = new FSMGraphView(this);
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }
    
    private void AddStyles()
    {
        rootVisualElement.AddStyleSheets("FSMSystem/FSMVariables.uss");
    }
    
    private void AddToolbar()
    {
        Toolbar toolbar = new Toolbar();
        Box box = new Box();
        
        _fileNameTextField = FSMElementUtility.CreateLabel("File Name: "+_fileName);
        
        _saveButton = FSMElementUtility.CreateButton("Save", () => Save());
        
        Button reloadButton = FSMElementUtility.CreateButton("Reload", () => Reload());
        Button clearButton = FSMElementUtility.CreateButton("Clear", () => Clear());
        _miniMapButton = FSMElementUtility.CreateButton("MiniMap", () => ToggleMiniMap());

        toolbar.Add(_fileNameTextField);
        toolbar.Add(_saveButton);
        toolbar.Add(reloadButton);
        toolbar.Add(clearButton);
        toolbar.Add(_miniMapButton);

        toolbar.AddStyleSheets("FSMSystem/FSMToolbarStyle.uss");
        rootVisualElement.Add(toolbar);
        box.AddStyleSheets("FSMSystem/FSMToolbarStyle.uss");
        rootVisualElement.Add(box);
    }

    #region Toolbar Actions
    private bool Save()
    {
        if(String.IsNullOrEmpty(initialState))
        {
            EditorUtility.DisplayDialog(
                "Invalid Initial State!",
                "Please select a valid initial state.",
                "OK"
            );
            return false;
        }
        _isCompiling = true;
        FSMIOUtility.Initialize(_fileName, _graphView, initialState);
        FSMIOUtility.Save();
        EnemyStateMachineEditor.GenerateScript(_saveData);
        return true;
    }
    private void Clear()
    {
        _graphView.ClearGraph();
    }
    private void Reload()
    {
        Clear();
        FSMIOUtility.Initialize(_saveData.FileName, _graphView, initialState);
        FSMIOUtility.Load();
    }
    
    private void ToggleMiniMap()
    {
        _graphView.ToggleMiniMap();
        _miniMapButton.ToggleInClassList("fsm-toolbar__button__selected");
    }
    private void GenerateScript()
    {
        if (Save())
        {
            EnemyStateMachineEditor.GenerateScript(_saveData);
            //_fsmInspector.AddComponentToGameObject();
        }
    }
    #endregion

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

    #endregion
    
}
