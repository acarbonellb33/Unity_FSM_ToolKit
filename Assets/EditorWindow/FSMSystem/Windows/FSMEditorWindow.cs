using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class FSMEditorWindow : EditorWindow
{
    private static FSMGraphView _graphView;
    private static string _fileName = "";
    private static Label _fileNameTextField;
    private Button _saveButton;
    private Button _miniMapButton;
    private Button _generateScriptButton;
    private static PopupField<string> _popupField;
    public static List<string> _stateNames = new List<string>();
    private static FSMEditorWindow _window;
    private static FSMGraphSaveData _saveData;
    private static FSMInspector _fsmInspector;
    public static void OpenWithSaveData(FSMGraphSaveData saveData, FSMInspector inspector)
    {
        _saveData = saveData;
        _fileName = saveData.FileName;
        _fsmInspector = inspector;
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
        _popupField.value = null;
        _stateNames.Clear();
        FSMIOUtility.Initialize(saveData.FileName, _graphView, _popupField.value);
        FSMIOUtility.Load();
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
        Box box = new Box();
        
        _fileNameTextField = FSMElementUtility.CreateLabel("File Name: "+_fileName);
        
        _saveButton = FSMElementUtility.CreateButton("Save", () => Save());
        
        Button reloadButton = FSMElementUtility.CreateButton("Reload", () => Reload());
        Button clearButton = FSMElementUtility.CreateButton("Clear", () => Clear());
        //Button resetButton = FSMElementUtility.CreateButton("Reset", () => ResetGraph());
        _miniMapButton = FSMElementUtility.CreateButton("MiniMap", () => ToggleMiniMap());
        _generateScriptButton = FSMElementUtility.CreateButton("Generate Script", () => GenerateScript());
        _popupField = new PopupField<string>("Select Initial State", _stateNames, 0);
        
        toolbar.Add(_fileNameTextField);
        toolbar.Add(_saveButton);
        toolbar.Add(reloadButton);
        toolbar.Add(clearButton);
        //toolbar.Add(resetButton);
        toolbar.Add(_miniMapButton);
        toolbar.Add(_generateScriptButton);
        toolbar.Add(_popupField);
        
        toolbar.AddStyleSheets("FSMSystem/FSMToolbarStyle.uss");
        rootVisualElement.Add(toolbar);
        box.AddStyleSheets("FSMSystem/FSMToolbarStyle.uss");
        rootVisualElement.Add(box);
    }

    #region Toolbar Actions
    private bool Save()
    {
        if(_popupField.value == null || _popupField.value == "")
        {
            EditorUtility.DisplayDialog(
                "Invalid Initial State!",
                "Please select a valid initial state.",
                "OK"
            );
            return false;
        }
        FSMIOUtility.Initialize(_fileName, _graphView, _popupField.value);
        FSMIOUtility.Save();
        _fsmInspector.UpdateComponentOfGameObject(_saveData);
        return true;
    }
    private void Clear()
    {
        _graphView.ClearGraph();
        ClearPopupField();
    }
    private void Reload()
    {
        Clear();
        FSMIOUtility.Initialize(_saveData.FileName, _graphView, _popupField.value);
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
    /*public static void UpdateFileName(string fileName)
    {
        _fileNameTextField.value = fileName;
    }*/
    public static void UpdatePopupField(List<string> stateNames, string initialState)
    {
        _stateNames.Clear();
        foreach (string name in stateNames)
        {
            if (!_stateNames.Contains(name))
            {
                _stateNames.Add(name);
                if(name.Equals(initialState))
                {
                    _popupField.value = name;
                }
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
    public string GetFileName()
    {
        return _fileName;
    }

    #endregion
    
}
