using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class FSMEditorWindow : EditorWindow
{
    private FSMGraphView _graphView;
    private readonly string _fileName = "New FSM";
    private static TextField _fileNameTextField;
    private Button _saveButton;
    private Button _miniMapButton;
    
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
        
        toolbar.Add(_fileNameTextField);
        toolbar.Add(_saveButton);
        toolbar.Add(loadButton);
        toolbar.Add(clearButton);
        toolbar.Add(resetButton);
        toolbar.Add(_miniMapButton);
        
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
        FSMIOUtility.Initialize(_fileNameTextField.value, _graphView);
        FSMIOUtility.Save();
    }
    private void Clear()
    {
        _graphView.ClearGraph();
    }
    private void ResetGraph()
    {
        _graphView.ClearGraph();
        UpdateFileName(_fileName);
    }
    private void Load()
    {
        string filePath = EditorUtility.OpenFilePanel("FSM Graphs", "Assets/EditorWindow/FSMSystem/Graphs", "asset");
        if (string.IsNullOrEmpty(filePath))return;
        Clear();
        FSMIOUtility.Initialize(Path.GetFileNameWithoutExtension(filePath), _graphView);
        FSMIOUtility.Load();
    }
    
    private void ToggleMiniMap()
    {
        _graphView.ToggleMiniMap();
        _miniMapButton.ToggleInClassList("fsm-toolbar__button__selected");
    }
    #endregion

    #region Utilities
    public static void UpdateFileName(string fileName)
    {
        _fileNameTextField.value = fileName;
    }
    public void EnableSaving()
    {
        _saveButton.SetEnabled(true);
    }
    public void DisableSaving()
    {
        _saveButton.SetEnabled(false);
    }

    #endregion
    
}
