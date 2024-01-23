using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class FSMEditorWindow : EditorWindow
{
    private FSMGraphView _graphView;
    private readonly string _fileName = "New FSM";
    private TextField _fileNameTextField;
    private Button _saveButton;
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
        
        toolbar.Add(_fileNameTextField);
        toolbar.Add(_saveButton);
        toolbar.AddStyleSheets("FSMSystem/FSMToolbarStyle.uss");
        rootVisualElement.Add(toolbar);
    }
    
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
    
    public void EnableSaving()
    {
        _saveButton.SetEnabled(true);
    }
    
    public void DisableSaving()
    {
        _saveButton.SetEnabled(false);
    }
}
