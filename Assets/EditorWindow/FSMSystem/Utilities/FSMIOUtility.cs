using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class FSMIOUtility
{
    private static FSMGraphView _graphView;
    private static string _graphName;
    private static string _containerFolderPath;
    
    private static List<FSMGroup> _groups;
    private static List<FSMNode> _nodes;

    private static Dictionary<string, FSMNodeGroupSO> _createdNodeGroups;
    private static Dictionary<string, FSMNodeSO> _createdNodes;

    public static void Initialize(string graphName, FSMGraphView fsmGraphView)
    {
        _graphView = fsmGraphView;
        _graphName = graphName;
        _containerFolderPath = $"Assets/FSMSystem/FSMs/{graphName}";
        
        _groups = new List<FSMGroup>();
        _nodes = new List<FSMNode>();
        
        _createdNodeGroups = new Dictionary<string, FSMNodeGroupSO>();
        _createdNodes = new Dictionary<string, FSMNodeSO>();
    }
    
    #region SaveMethods
    public static void Save()
    {
        CreateStaticFolders();
        GetElementsFromGraphView();
        FSMGraphSaveData graphSaveData = CreateAsset<FSMGraphSaveData>("Assets/EditorWindow/FSMSystem/Graphs", _graphName);
        graphSaveData.Initialize(_graphName);
        FSMNodesContainerSO nodesContainer = CreateAsset<FSMNodesContainerSO>(_containerFolderPath, _graphName);
        nodesContainer.Initialize(_graphName);
        
        SaveGroups(graphSaveData, nodesContainer);
        SaveNodes(graphSaveData, nodesContainer);

        SaveAsset(graphSaveData);
        SaveAsset(nodesContainer);
    }

    #region Groups
    private static void SaveGroups(FSMGraphSaveData graphSaveData, FSMNodesContainerSO nodesContainerSo)
    {
        List<string> groupNames = new List<string>();
        foreach(FSMGroup group in _groups)
        {
            SaveGroupToGraph(group, graphSaveData);
            SaveGroupToScriptableObject(group, nodesContainerSo);
            groupNames.Add(group.title);
        }

        UpdateOldGroups(groupNames, graphSaveData);
    }
    private static void SaveGroupToGraph(FSMGroup group, FSMGraphSaveData graphSaveData)
    {
        FSMGroupSaveData groupSaveData = new FSMGroupSaveData()
        {
            Id = group.Id,
            Name = group.title,
            Position = group.GetPosition().position
        };
        graphSaveData.Groups.Add(groupSaveData);
    }
    
    private static void SaveGroupToScriptableObject(FSMGroup group, FSMNodesContainerSO nodesContainerSo)
    {
        string groupName = group.title;
        CreateFolder($"{_containerFolderPath}/Groups", groupName);
        CreateFolder($"{_containerFolderPath}/Groups/{groupName}", "Nodes");
        
        FSMNodeGroupSO nodesGroup = CreateAsset<FSMNodeGroupSO>($"{_containerFolderPath}/Groups/{groupName}", groupName);
        nodesGroup.Initialize(groupName);
        
        _createdNodeGroups.Add(group.Id, nodesGroup);
        
        nodesContainerSo.GroupedNodes.Add(nodesGroup, new List<FSMNodeSO>());

        SaveAsset(nodesGroup);
    }
    
    private static void UpdateOldGroups(List<string> currentGroupNames, FSMGraphSaveData graphSaveData)
    {
        if (graphSaveData.OldGroupedNames != null && graphSaveData.OldGroupedNames.Count != 0)
        {
            List<string> groupsToRemove = graphSaveData.OldGroupedNames.Except(currentGroupNames).ToList();

            foreach (string groupToRemove in groupsToRemove)
            {
                RemoveFolder($"{_containerFolderPath}/Groups/{groupToRemove}");
            }
        }

        graphSaveData.OldGroupedNames = new List<string>(currentGroupNames);
    }
    #endregion

    #region Nodes
    private static void SaveNodes(FSMGraphSaveData graphSaveData, FSMNodesContainerSO nodesContainer)
    {
        SerializableDictionary<string, List<string>> groupedNodeNames = new SerializableDictionary<string, List<string>>();
        List<string> ungroupedNodeNames = new List<string>();
        
        foreach(FSMNode node in _nodes)
        {
            SaveNodeToGraph(node, graphSaveData);
            SaveNodeToScriptableObject(node, nodesContainer);
            if(node.Group != null)
            {
                groupedNodeNames.AddItem(node.Group.title, node.StateName);
                continue;
            }
            
            ungroupedNodeNames.Add(node.StateName);
        }

        UpdateNodeConnections();
        UpdateOldGroupedNodes(groupedNodeNames, graphSaveData);
        UpdateOldUngroupedNodes(ungroupedNodeNames, graphSaveData);
    }
    private static void SaveNodeToGraph(FSMNode node, FSMGraphSaveData graphSaveData)
    {
        List<FSMConnectionSaveData> connections = new List<FSMConnectionSaveData>();
        foreach(FSMConnectionSaveData connection in node.Choices)
        {
            FSMConnectionSaveData connectionSaveData = new FSMConnectionSaveData()
            {
                //for adding all my custom values in each node
                NodeId = connection.NodeId,
            };
            connections.Add(connectionSaveData);
        }
        
        FSMNodeSaveData nodeSaveData = new FSMNodeSaveData()
        {
            Id = node.Id,
            Name = node.StateName,
            Connections = connections,
            GroupId = node.Group?.Id,
            DialogueType = node.DialogueType,
            Position = node.GetPosition().position
        };
        graphSaveData.Nodes.Add(nodeSaveData);
    }
    private static void SaveNodeToScriptableObject(FSMNode node, FSMNodesContainerSO nodesContainer)
    {
        FSMNodeSO nodeSo;
        if (node.Group != null)
        {
            nodeSo = CreateAsset<FSMNodeSO>($"{_containerFolderPath}/Groups/{node.Group.title}/Nodes", node.StateName);
            nodesContainer.GroupedNodes.AddItem(_createdNodeGroups[node.Group.Id], nodeSo);
        }
        else
        {
            nodeSo = CreateAsset<FSMNodeSO>($"{_containerFolderPath}/Global/Nodes", node.StateName);
            nodesContainer.UngroupedNodes.Add(nodeSo);
        }
        
        nodeSo.Initialize(
            node.StateName,
            "Text",
            ConvertNodeConnection(node.Choices),
            node.DialogueType,
            false
        );
        _createdNodes.Add(node.Id, nodeSo);
        SaveAsset(nodeSo);
    }
    private static List<FSMNodeConnectionData> ConvertNodeConnection(List<FSMConnectionSaveData> connections)
    {
        List<FSMNodeConnectionData> convertedConnections = new List<FSMNodeConnectionData>();
        foreach(FSMConnectionSaveData connection in connections)
        {
            FSMNodeConnectionData connectionSo = new FSMNodeConnectionData()
            {
                Text = connection.Text,
            };
            convertedConnections.Add(connectionSo);
        }
        return convertedConnections;
    }
    private static void UpdateNodeConnections()
    {
        Debug.Log(_nodes.Count);
        foreach(FSMNode node in _nodes)
        {
            Debug.Log("a");
            FSMNodeSO nodeSelected = _createdNodes[node.Id];
            for(int index = 0; index < node.Choices.Count; index++)
            {
                Debug.Log("b");
                FSMConnectionSaveData connection = node.Choices[index];
                if(string.IsNullOrEmpty(connection.NodeId))
                {
                    Debug.Log("c");
                    continue;
                }
                Debug.Log("d");
                Debug.Log(_createdNodes[connection.NodeId]);
                nodeSelected.Connections[index].NextNode = _createdNodes[connection.NodeId];
                SaveAsset(nodeSelected);
            }
        }
    }
    private static void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> currentGroupedNodeNames, FSMGraphSaveData graphSaveData)
    {
        if (graphSaveData.OldGroupedNodeNames != null && graphSaveData.OldGroupedNodeNames.Count != 0)
        {
            foreach (KeyValuePair<string, List<string>> oldGroupedNode in graphSaveData.OldGroupedNodeNames)
            {
                List<string> nodesToRemove = new List<string>();

                if (currentGroupedNodeNames.ContainsKey(oldGroupedNode.Key))
                {
                    nodesToRemove = oldGroupedNode.Value.Except(currentGroupedNodeNames[oldGroupedNode.Key]).ToList();
                }

                foreach (string nodeToRemove in nodesToRemove)
                {
                    RemoveAsset($"{_containerFolderPath}/Groups/{oldGroupedNode.Key}/Nodes", nodeToRemove);
                }
            }
        }
    }
    private static void UpdateOldUngroupedNodes(List<string> currentUngroupedNodeNames, FSMGraphSaveData graphSaveData)
    {
        if (graphSaveData.OldUngroupedNames != null && graphSaveData.OldUngroupedNames.Count != 0)
        {
            List<string> nodesToRemove = graphSaveData.OldUngroupedNames.Except(currentUngroupedNodeNames).ToList();

            foreach (string nodeToRemove in nodesToRemove)
            {
                RemoveAsset($"{_containerFolderPath}/Global/Nodes/", nodeToRemove);
            }
        }
        graphSaveData.OldUngroupedNames = new List<string>(currentUngroupedNodeNames);
    }
    #endregion
    
    #endregion

    #region CreationMethods
    private static void CreateStaticFolders()
    {
        CreateFolder("Assets/EditorWindow/FSMSystem", "Graphs");
        CreateFolder("Assets", "FSMSystem");
        CreateFolder("Assets/FSMSystem", "FSMs");
        CreateFolder("Assets/FSMSystem/FSMs", _graphName);
        CreateFolder(_containerFolderPath, "Global");
        CreateFolder(_containerFolderPath, "Groups");
        CreateFolder($"{_containerFolderPath}/Global", "Nodes");
    } 
    #endregion

    #region UtilityMethods
    private static void CreateFolder(string path, string folderName)
    {
        if (AssetDatabase.IsValidFolder($"{path}/{folderName}"))
        {
            return;
        }
        AssetDatabase.CreateFolder(path, folderName);
    }
    private static void RemoveFolder(string path)
    {
        FileUtil.DeleteFileOrDirectory($"{path}.meta");
        FileUtil.DeleteFileOrDirectory($"{path}/");
    }
    private static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
    {
        string assetPathAndName = $"{path}/{assetName}.asset";
        T asset = AssetDatabase.LoadAssetAtPath<T>(assetPathAndName);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, assetPathAndName);
        }
        return asset;
    }
    private static void RemoveAsset(string path, string assetName)
    {
        AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
    }
    private static void SaveAsset(UnityEngine.Object asset)
    {
        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    #endregion
    
    #region GetMethods
    private static void GetElementsFromGraphView()
    {
        Type groupType = typeof(FSMGroup);
        
        _graphView.graphElements.ForEach(graphElement =>
        {
            if (graphElement is FSMNode node)
            {
                _nodes.Add(node);
                return;
            }

            if (graphElement.GetType() == groupType)
            {
                FSMGroup group = (FSMGroup) graphElement;
                _groups.Add(group);
                return;
            }
        });
    }
    #endregion
}
