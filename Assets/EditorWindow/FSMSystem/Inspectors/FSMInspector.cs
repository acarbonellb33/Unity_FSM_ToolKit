using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FSMGraph))]
public class FSMInspector : Editor
{
    public GameObject graphContainer;
    /* Dialogue Scriptable Objects */
    private SerializedProperty graphContainerProperty;
    private SerializedProperty graphGroupProperty;
    private SerializedProperty graphProperty;

    /* Filters */
    private SerializedProperty groupedStatesProperty;
    //private SerializedProperty startingDialoguesOnlyProperty;

    /* Indexes */
    private SerializedProperty selectedStatesGroupIndexProperty;
    private SerializedProperty selectedStateIndexProperty;

    private void OnEnable()
    {
        graphContainerProperty = serializedObject.FindProperty("graphContainer");
        graphGroupProperty = serializedObject.FindProperty("graphGroup");
        graphProperty = serializedObject.FindProperty("graph");

        groupedStatesProperty = serializedObject.FindProperty("groupedStates");
        //startingDialoguesOnlyProperty = serializedObject.FindProperty("startingDialoguesOnly");

        selectedStatesGroupIndexProperty = serializedObject.FindProperty("selectedStatesGroupIndex");
        selectedStateIndexProperty = serializedObject.FindProperty("selectedStateIndex");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawGraphContainerArea();
        
        FSMGraphSaveData currentGraphContainer = (FSMGraphSaveData) graphContainerProperty.objectReferenceValue;

        if (currentGraphContainer == null)
        {
            StopDrawing("Select a FSM Graph Container to see the rest of the Inspector.");
            return;
        }
        
        //Add a button to open the FSM Graph 
        if (GUILayout.Button("Open FSM Graph"))
        {
            FSMEditorWindow.OpenWithSaveData(currentGraphContainer);
        }

        /*DrawFiltersArea();

        List<string> stateNames;
        string graphFolderPath = $"Assets/FSMSystem/FSMs/{currentGraphContainer.FileName}";
        string graphInfoMessage = string.Empty;

        if (groupedStatesProperty.boolValue)
        {
            List<string> groupedStateNames = currentGraphContainer.GetGroupNames();
            if(groupedStateNames.Count == 0)
            {
                StopDrawing("There are no FSM Graph Groups in the selected FSM Graph Container.");
                return;
            }
            DrawStatesGroupArea(currentGraphContainer, groupedStateNames);
            FSMNodeGroupSO currentGroup = (FSMNodeGroupSO) graphGroupProperty.objectReferenceValue;
            stateNames = currentGraphContainer.GetGroupedStateNames(currentGroup);
            graphFolderPath += $"/Groups/{currentGroup.GroupName}/Nodes";
            graphInfoMessage = "There are no Nodes in the selected Group.";
        }
        else
        {
            stateNames = currentGraphContainer.GetUngroupedStateNames();
            graphFolderPath += "/Global/Nodes";
            graphInfoMessage = "There are no Nodes in the selected FSM Graph Container.";
        }
        if(stateNames.Count == 0)
        {
            StopDrawing(graphInfoMessage);
            return;
        }
        
        DrawStatesArea(stateNames, graphFolderPath);*/

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawGraphContainerArea()
    {
        FSMInspectorUtility.DrawHeader("FSM Graph Container");
        graphContainerProperty.DrawPropertyField();
        FSMInspectorUtility.DrawSpace();
    }

    private void DrawFiltersArea()
    {
        FSMInspectorUtility.DrawHeader("Filters");
        groupedStatesProperty.DrawPropertyField();
        FSMInspectorUtility.DrawSpace();
    }

    private void DrawStatesGroupArea(FSMNodesContainerSO nodesContainer, List<string> groupedStateNames)
    {
        FSMInspectorUtility.DrawHeader("FSM Graph Group");
        
        int oldSelectedStateGroupIndex = selectedStatesGroupIndexProperty.intValue;
        FSMNodeGroupSO oldSelectedGroup = (FSMNodeGroupSO) graphGroupProperty.objectReferenceValue;
        bool isOldSelectedGroupNull = oldSelectedGroup == null;
        string oldSelectedGroupName = isOldSelectedGroupNull ? string.Empty : oldSelectedGroup.GroupName;
        
        UpdateIndexOnNamesListUpdate(groupedStateNames, selectedStatesGroupIndexProperty, oldSelectedStateGroupIndex, oldSelectedGroupName, isOldSelectedGroupNull);

        selectedStatesGroupIndexProperty.intValue = FSMInspectorUtility.DrawPopup("Graph Group", selectedStatesGroupIndexProperty, groupedStateNames.ToArray());
        string selectedGroupName = groupedStateNames[selectedStatesGroupIndexProperty.intValue];
        FSMNodeGroupSO selectedGroup = FSMIOUtility.LoadAsset<FSMNodeGroupSO>($"Assets/FSMSystem/FSMs/{nodesContainer.FileName}/Groups/{selectedGroupName}", selectedGroupName);
        graphGroupProperty.objectReferenceValue = selectedGroup;
        FSMInspectorUtility.DrawDisabledFields(() => graphGroupProperty.DrawPropertyField());
        FSMInspectorUtility.DrawSpace();
    }

    private void DrawStatesArea(List<string> stateNames, string graphFolderPath)
    {
        FSMInspectorUtility.DrawHeader("FSM Graph");
        
        int oldSelectedStateIndex = selectedStateIndexProperty.intValue;
        FSMNodeSO oldSelectedState = (FSMNodeSO) graphProperty.objectReferenceValue;
        bool isOldSelectedNull = oldSelectedState == null;
        string oldSelectedName = isOldSelectedNull ? string.Empty : oldSelectedState.NodeName;
        
        UpdateIndexOnNamesListUpdate(stateNames, selectedStateIndexProperty, oldSelectedStateIndex, oldSelectedName, isOldSelectedNull);
        
        selectedStateIndexProperty.intValue = FSMInspectorUtility.DrawPopup("Graph", selectedStateIndexProperty, stateNames.ToArray());
        string selectedStateName = stateNames[selectedStateIndexProperty.intValue];
        FSMNodeSO selectedState = FSMIOUtility.LoadAsset<FSMNodeSO>(graphFolderPath, selectedStateName);
        graphProperty.objectReferenceValue = selectedState;
        FSMInspectorUtility.DrawDisabledFields(() => graphProperty.DrawPropertyField());
        
    }

    private void StopDrawing(string reason, MessageType messageType = MessageType.Info)
    {
        FSMInspectorUtility.DrawHelpBox(reason, messageType);
        FSMInspectorUtility.DrawSpace();
        FSMInspectorUtility.DrawHelpBox("You need to select a Graph for this component to work properly at Runtime!", MessageType.Warning); 
        serializedObject.ApplyModifiedProperties();
    }

    #region IndexMethods

    private void UpdateIndexOnNamesListUpdate(List<string> optionNames, SerializedProperty indexProperty, int oldSelectedPropertyIndex, string oldPropertyName, bool isOldPropertyNull)
    {
        if (isOldPropertyNull)
        {
            indexProperty.intValue = 0;

            return;
        }

        bool oldIndexIsOutOfBoundsOfNamesListCount = oldSelectedPropertyIndex > optionNames.Count - 1;
        bool oldNameIsDifferentThanSelectedName = oldIndexIsOutOfBoundsOfNamesListCount || oldPropertyName != optionNames[oldSelectedPropertyIndex];

        if (oldNameIsDifferentThanSelectedName)
        {
            if (optionNames.Contains(oldPropertyName))
            {
                indexProperty.intValue = optionNames.IndexOf(oldPropertyName);

                return;
            }

            indexProperty.intValue = 0;
        }
    } 

    #endregion
    
    [MenuItem("Assets/Create/ScriptableObjects/FSMNodesContainer")]
    public static void CreateFSMNodesContainer()
    {
        FSMNodesContainerSO asset = ScriptableObject.CreateInstance<FSMNodesContainerSO>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path))
        {
            path = "Assets";
        }
        else if (!string.IsNullOrEmpty(System.IO.Path.GetExtension(path)))
        {
            path = path.Replace(System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string fileName = asset.FileName;
        if (string.IsNullOrEmpty(fileName))
        {
            fileName = "NewFSMNodesContainer";
        }
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + fileName + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}
