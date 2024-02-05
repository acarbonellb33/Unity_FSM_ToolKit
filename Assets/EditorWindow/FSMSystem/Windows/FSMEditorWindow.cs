using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class FSMEditorWindow : EditorWindow
{
    private FSMGraphView _graphView;
    private readonly string _fileName = "New FSM";
    private static TextField _fileNameTextField;
    private Button _saveButton;
    private Button _miniMapButton;
    private Button _generateScriptButton;
    private PopupField<string> _popupField;
    public static List<string> _stateNames = new List<string>();

    [MenuItem("Window/FSM/FSM Graph")]
    public static void Open()
    {
        GetWindow<FSMEditorWindow>("FSM Graph");
    }
    
    private void OnEnable()
    {
        AddGraphView();  
        
        AddToolbar();
        
        AddStyles();
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
        
        _fileNameTextField = FSMElementUtility.CreateTextField(_fileName, "File Name: ");
        
        _saveButton = FSMElementUtility.CreateButton("Save", () => Save());
        
        Button loadButton = FSMElementUtility.CreateButton("Load", () => Load());
        Button clearButton = FSMElementUtility.CreateButton("Clear", () => Clear());
        Button resetButton = FSMElementUtility.CreateButton("Reset", () => ResetGraph());
        _miniMapButton = FSMElementUtility.CreateButton("MiniMap", () => ToggleMiniMap());
        _generateScriptButton = FSMElementUtility.CreateButton("Generate Script", () => GenerateScript());
        _popupField = new PopupField<string>("Select Initial State", _stateNames, 0);

        toolbar.Add(_fileNameTextField);
        toolbar.Add(_saveButton);
        toolbar.Add(loadButton);
        toolbar.Add(clearButton);
        toolbar.Add(resetButton);
        toolbar.Add(_miniMapButton);
        toolbar.Add(_generateScriptButton);
        toolbar.Add(_popupField);
        
        toolbar.AddStyleSheets("FSMSystem/FSMToolbarStyle.uss");
        rootVisualElement.Add(toolbar);
    }

    #region Toolbar Actions
    private void Save()
    {
        if (string.IsNullOrEmpty(_fileNameTextField.value))
        {
            EditorUtility.DisplayDialog(
                "Invalid file name!",
                "Please enter a valid file name.",
                "OK"
            );
            return;
        }
        if(_popupField.value == null || _popupField.value == "")
        {
            EditorUtility.DisplayDialog(
                "Invalid Initial State!",
                "Please select a valid initial state.",
                "OK"
            );
            return;
        }
        FSMIOUtility.Initialize(_fileNameTextField.value, _graphView, _popupField.value);
        FSMIOUtility.Save();
    }
    private void Clear()
    {
        _graphView.ClearGraph();
        ClearPopupField();
    }
    private void ResetGraph()
    {
        _graphView.ClearGraph();
        UpdateFileName(_fileName);
        ClearPopupField();
    }
    private void Load()
    {
        string filePath = EditorUtility.OpenFilePanel("FSM Graphs", "Assets/EditorWindow/FSMSystem/Graphs", "asset");
        if (string.IsNullOrEmpty(filePath))return;
        Clear();
        FSMIOUtility.Initialize(Path.GetFileNameWithoutExtension(filePath), _graphView, _popupField.value);
        FSMIOUtility.Load();
    }
    
    private void ToggleMiniMap()
    {
        _graphView.ToggleMiniMap();
        _miniMapButton.ToggleInClassList("fsm-toolbar__button__selected");
    }
    private void GenerateScript()
    {
        Save();
        FSMGraphSaveData saveData = FSMIOUtility.LoadAsset<FSMGraphSaveData>("Assets/EditorWindow/FSMSystem/Graphs",  _fileNameTextField.value);
      
        EnemyStateMachineEditor.GenerateScript(saveData);
    }
    #endregion

    #region Utilities
    public static void UpdateFileName(string fileName)
    {
        _fileNameTextField.value = fileName;
    }
    public static void UpdatePopupField(List<string> stateNames)
    {
        _stateNames.Clear();
        foreach (string name in stateNames)
        {
            if (!_stateNames.Contains(name))
            {
                _stateNames.Add(name);
            }
        }
    }
    private void ClearPopupField()
    {
        _popupField.value = null;
        _stateNames.Clear();
    }
    public void EnableSaving()
    {
        _saveButton.SetEnabled(true);
        _generateScriptButton.SetEnabled(true);
    }
    public void DisableSaving()
    {
        _saveButton.SetEnabled(false);
        _generateScriptButton.SetEnabled(false);
    }
    
    #endregion
    
}
