using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class FSMGraphView : GraphView
{
    private FSMSearchWindow _searchWindow;
    private FSMEditorWindow _window;

    private MiniMap _miniMap;

    public SerializableDictionary<string, FSMNodeErrorData> _ungroupedNodes;
    private SerializableDictionary<string, FSMGroupErrorData> _groups;
    public SerializableDictionary<Group, SerializableDictionary<string, FSMNodeErrorData>> _groupedNodes;

    private int repeatedNameCount;

    public int RepeatedNameCount
    {
        get { return repeatedNameCount; }
        set
        {
            repeatedNameCount = value;
            if (repeatedNameCount == 0)
            {
                _window.EnableSaving();
            }
            else
            {
                _window.DisableSaving();
            }
        }
    }

    public FSMGraphView(FSMEditorWindow window)
    {
        _window = window;
        _ungroupedNodes = new SerializableDictionary<string, FSMNodeErrorData>();
        _groups = new SerializableDictionary<string, FSMGroupErrorData>();
        _groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, FSMNodeErrorData>>();

        AddManipulators();
        AddSearchWindow();
        AddMiniMap();
        AddGridBackground();
        OnElementsDeleted();
        OnGroupElementsAdded();
        OnGroupElementsRemoved();
        OnGroupRenamed();
        OnGraphViewChanged();
        AddStyles();
        AddMiniMapStyles();
        
        //AddElement(CreateNode("Initial State", new Vector2(100, 45), FSMNodeType.Initial));
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> compatiblePorts = new List<Port>();
        ports.ForEach((port) =>
        {
            if (startPort == port) return;
            if (startPort.node == port.node) return;
            if (startPort.direction == port.direction) return;
            if (startPort.node.GetType() == typeof(FSMStateNode) && port.node.GetType() == typeof(FSMStateNode)) return;
            compatiblePorts.Add(port);
        });
        return compatiblePorts;
    }

    public FSMNode CreateNode(string nodeName, Vector2 position, FSMNodeType nodeT, bool shouldDraw = true)
    {
        Type nodeType = Type.GetType($"FSM{nodeT}Node");
        FSMNode node = (FSMNode)Activator.CreateInstance(nodeType);
        
        if(nodeT == FSMNodeType.Transition || nodeT == FSMNodeType.DualTransition)
        {
            int count = 0;
            foreach (string stateName in _ungroupedNodes.Keys)
            {
                if (stateName.Split(" ")[0] == nodeName.Split(" ")[0])
                {
                    nodeName = nodeName.Split(" ")[0];
                    _ungroupedNodes.UpdateKey(stateName, nodeName+ " " + count);
                    GetNodeFromGraph(stateName).SetStateName(nodeName+ " " + count);
                    //GetNodeFromGraph(stateName).SetStateName(nodeName + " " + count);
                    count++;
                    nodeName = nodeName + " " + count;
                }
            }
        }
        node.Initialize(nodeName, this, position);

        if (shouldDraw)
        {
            node.Draw();
        }

        if (nodeT == FSMNodeType.State) FSMEditorWindow._stateNames.Add(nodeName);
        AddUngroupedNode(node);
        return node;
    }

    /*public FSMTransitionNode CreateTransition(Vector2 position)
    {
        FSMTransitionNode node = new FSMTransitionNode();
        node.Initialize(this, position);
        node.Draw();
        AddUngroupedNode(node);
        return node;
    }*/

    public FSMGroup CreateGroup(string title, Vector2 localMousePosition)
    {
        FSMGroup group = new FSMGroup(title, localMousePosition);
        AddGroup(group);
        AddElement(group);
        foreach (GraphElement selectedElement in selection)
        {
            if (!(selectedElement is FSMNode))
            {
                continue;
            }

            FSMNode node = (FSMNode)selectedElement;
            group.AddElement(node);
        }

        return group;
    }

    public void AddUngroupedNode(FSMNode node)
    {
        string nodeName = node.StateName;
        if (!_ungroupedNodes.ContainsKey(nodeName))
        {
            FSMNodeErrorData nodeErrorData = new FSMNodeErrorData();
            nodeErrorData.Nodes.Add(node);
            _ungroupedNodes.Add(nodeName, nodeErrorData);
            return;
        }

        List<FSMNode> ungroupedNodesList = _ungroupedNodes[nodeName].Nodes;

        ungroupedNodesList.Add(node);

        Color errorColor = _ungroupedNodes[nodeName].ErrorData.Color;
        node.SetErrorStyle(errorColor);
        
        if (ungroupedNodesList.Count == 2)
        {
            ++RepeatedNameCount;
            ungroupedNodesList[0].SetErrorStyle(errorColor);
        }
    }

    public void RemoveUngroupedNode(FSMNode node)
    {
        string nodeName = node.StateName;
        List<FSMNode> ungroupedNodesList = _ungroupedNodes[nodeName].Nodes;

        ungroupedNodesList.Remove(node);
        node.ResetStyle();

        if (ungroupedNodesList.Count == 1)
        {
            --RepeatedNameCount;
            ungroupedNodesList[0].ResetStyle();
            return;
        }

        if (ungroupedNodesList.Count == 0)
        {
            _ungroupedNodes.Remove(nodeName);
            if(node.NodeType == FSMNodeType.Transition || node.NodeType == FSMNodeType.DualTransition)
            {
                int count = 0;
                foreach (string stateName in _ungroupedNodes.Keys)
                {
                    if (stateName.Split(" ")[0] == nodeName.Split(" ")[0])
                    {
                        nodeName = nodeName.Split(" ")[0];
                        
                        _ungroupedNodes.UpdateKey(stateName, nodeName + " " + count);
                        GetNodeFromGraph(stateName).SetStateName(nodeName + " " + count);
                        count++;
                    }
                }
            }
        }
    }
    
    private FSMNode GetNodeFromGraph(string nodeName)
    {
        foreach (GraphElement element in graphElements)
        {
            if (element is FSMNode node)
            {
                if (node.StateName == nodeName)
                {
                    return node; 
                }
            }
        }
        return null;
    }

    private void OnElementsDeleted()
    {
        deleteSelection = (operationName, askUser) =>
        {
            Type groupType = typeof(FSMGroup);
            Type edgeType = typeof(Edge);

            List<FSMGroup> groupsToDelete = new List<FSMGroup>();
            List<Edge> edgesToDelete = new List<Edge>();
            List<FSMNode> nodesToDelete = new List<FSMNode>();
            foreach (GraphElement element in selection)
            {
                if (element is FSMNode node)
                {
                    if (element is FSMStateNode n)
                    {
                        FSMEditorWindow._stateNames.Remove(n.StateName);
                    }

                    nodesToDelete.Add(node);
                    continue;
                }

                if (element.GetType() == edgeType)
                {
                    edgesToDelete.Add((Edge)element);
                    continue;
                }

                if (element.GetType() != groupType)
                {
                    continue;
                }

                FSMGroup group = (FSMGroup)element;
                groupsToDelete.Add(group);
            }

            foreach (FSMGroup group in groupsToDelete)
            {
                List<FSMNode> groupNodes = new List<FSMNode>();
                foreach (GraphElement element in group.containedElements)
                {
                    if (!(element is FSMNode))
                    {
                        continue;
                    }

                    FSMNode node = (FSMNode)element;
                    groupNodes.Add(node);
                }

                group.RemoveElements(groupNodes);
                RemoveGroup(group);
                RemoveElement(group);
            }

            DeleteElements(edgesToDelete);

            foreach (FSMNode node in nodesToDelete)
            {
                if (node.Group != null)
                {
                    node.Group.RemoveElement(node);
                }
                RemoveUngroupedNode(node);
                node.DisconnectAllPorts();
                RemoveElement(node);
            }
        };
    }

    private void OnGroupElementsAdded()
    {
        elementsAddedToGroup = (group, elements) =>
        {
            foreach (GraphElement element in elements)
            {
                if (!(element is FSMNode))
                {
                    continue;
                }

                FSMGroup newGroup = (FSMGroup)group;
                FSMNode node = (FSMNode)element;
                RemoveUngroupedNode(node);
                AddGroupedNode(node, newGroup);
            }
        };
    }

    private void OnGroupElementsRemoved()
    {
        elementsRemovedFromGroup = (group, elements) =>
        {
            foreach (GraphElement element in elements)
            {
                if (!(element is FSMNode))
                {
                    continue;
                }

                FSMNode node = (FSMNode)element;
                RemoveGroupedNode(node, group);
                AddUngroupedNode(node);
            }
        };
    }

    private void OnGroupRenamed()
    {
        groupTitleChanged = (group, title) =>
        {
            FSMGroup fsmGroup = (FSMGroup)group;
            RemoveGroup(fsmGroup);
            fsmGroup.PreviousTitle = title;
            AddGroup(fsmGroup);
        };
    }

    private void OnGraphViewChanged()
    {
        graphViewChanged = (changes) =>
        {
            if (changes.edgesToCreate != null)
            {
                foreach (Edge edge in changes.edgesToCreate)
                {
                    FSMNode nextNode = (FSMNode)edge.input.node;
                    FSMConnectionSaveData choiceData = (FSMConnectionSaveData)edge.output.userData;
                    choiceData.NodeId = nextNode.Id;

                    if (choiceData.Text == "Initial Node")
                    {
                        _window.initialState = nextNode.StateName;
                    }
                }
            }

            if (changes.elementsToRemove != null)
            {
                Type edgeType = typeof(Edge);

                foreach (GraphElement element in changes.elementsToRemove)
                {
                    if (element.GetType() != edgeType)
                    {
                        continue;
                    }

                    Edge edge = (Edge)element;
                    FSMConnectionSaveData choiceData = (FSMConnectionSaveData)edge.output.userData;
                    choiceData.NodeId = "";
                    
                    if (choiceData.Text == "Initial Node")
                    {
                        _window.initialState = "";
                    }
                }
            }

            return changes;
        };
    }

    public void AddGroupedNode(FSMNode node, FSMGroup group)
    {
        string nodeName = node.StateName;
        node.Group = group;
        if (!_groupedNodes.ContainsKey(group))
        {
            _groupedNodes.Add(group, new SerializableDictionary<string, FSMNodeErrorData>());
        }

        if (!_groupedNodes[group].ContainsKey(nodeName))
        {
            FSMNodeErrorData nodeErrorData = new FSMNodeErrorData();
            nodeErrorData.Nodes.Add(node);
            _groupedNodes[group].Add(nodeName, nodeErrorData);
            return;
        }

        List<FSMNode> groupedNodesList = _groupedNodes[group][nodeName].Nodes;

        groupedNodesList.Add(node);
        Color errorColor = _groupedNodes[group][nodeName].ErrorData.Color;
        node.SetErrorStyle(errorColor);
        if (groupedNodesList.Count == 2)
        {
            ++RepeatedNameCount;
            groupedNodesList[0].SetErrorStyle(errorColor);
        }
    }

    public void RemoveGroupedNode(FSMNode node, Group group)
    {
        string nodeName = node.StateName;
        node.Group = null;
        List<FSMNode> groupedNodesList = _groupedNodes[group][nodeName].Nodes;

        groupedNodesList.Remove(node);
        node.ResetStyle();

        if (groupedNodesList.Count == 1)
        {
            --RepeatedNameCount;
            groupedNodesList[0].ResetStyle();
            return;
        }

        if (groupedNodesList.Count == 0)
        {
            _groupedNodes[group].Remove(nodeName);
            if (_groupedNodes[group].Count == 0)
            {
                _groupedNodes.Remove(group);
            }
        }
    }

    private void AddGroup(FSMGroup group)
    {
        string groupName = group.title;
        if (!_groups.ContainsKey(groupName))
        {
            FSMGroupErrorData groupErrorData = new FSMGroupErrorData();
            groupErrorData.Groups.Add(group);
            _groups.Add(groupName, groupErrorData);
            return;
        }

        List<FSMGroup> groupsList = _groups[groupName].Groups;
        groupsList.Add(group);
        Color errorColor = _groups[groupName].ErrorData.Color;
        group.SetErrorStyle(errorColor);
        if (groupsList.Count == 2)
        {
            ++RepeatedNameCount;
            groupsList[0].SetErrorStyle(errorColor);
        }
    }

    private void RemoveGroup(FSMGroup group)
    {
        string groupName = group.PreviousTitle;
        List<FSMGroup> groupsList = _groups[groupName].Groups;
        groupsList.Remove(group);
        group.ResetStyle();
        if (groupsList.Count == 1)
        {
            --RepeatedNameCount;
            groupsList[0].ResetStyle();
            return;
        }

        if (groupsList.Count == 0)
        {
            _groups.Remove(groupName);
        }
    }

    private void AddMiniMap()
    {
        _miniMap = new MiniMap()
        {
            anchored = true
        };
        _miniMap.SetPosition(new Rect(10, 45, 200, 140));
        Add(_miniMap);

        _miniMap.visible = false;
    }

    private void AddGridBackground()
    {
        GridBackground gridBackground = new GridBackground();
        gridBackground.StretchToParentSize();
        Insert(0, gridBackground);
    }

    private void AddStyles()
    {
        this.AddStyleSheets("FSMSystem/FSMGraphViewStyle.uss", "FSMSystem/FSMNodeStyle.uss");
    }

    private void AddMiniMapStyles()
    {
        StyleColor backgroundColor = new StyleColor(new Color32(29, 29, 30, 255));
        StyleColor borderColor = new StyleColor(new Color32(51, 51, 51, 255));

        _miniMap.style.backgroundColor = backgroundColor;
        _miniMap.style.borderBottomColor = borderColor;
        _miniMap.style.borderLeftColor = borderColor;
        _miniMap.style.borderRightColor = borderColor;
        _miniMap.style.borderTopColor = borderColor;
    }

    private void AddManipulators()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(CreateStateItemMenu("Attack"));
        this.AddManipulator(CreateStateItemMenu("Patrol"));
        this.AddManipulator(CreateStateItemMenu("Chase"));
        this.AddManipulator(CreateStateItemMenu("Search"));
        this.AddManipulator(CreateTransitionItemMenu("Hearing"));
        this.AddManipulator(CreateTransitionItemMenu("Seeing"));
        this.AddManipulator(CreateTransitionItemMenu("Distance"));
        this.AddManipulator(CreateTransitionItemMenu("Health"));
        this.AddManipulator(CreateDualTransitionStateItemMenu("Hearing"));
        this.AddManipulator(CreateDualTransitionStateItemMenu("Seeing"));
        this.AddManipulator(CreateDualTransitionStateItemMenu("Distance"));
        this.AddManipulator(CreateDualTransitionStateItemMenu("Health"));
        this.AddManipulator(CreateGroupContextualMenu());
    }

    private void AddSearchWindow()
    {
        if (_searchWindow == null)
        {
            _searchWindow = ScriptableObject.CreateInstance<FSMSearchWindow>();
            _searchWindow.Initialize(this);
        }

        nodeCreationRequest = context =>
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
    }

    private IManipulator CreateGroupContextualMenu()
    {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction("Create Group",
                menuActionEvent => CreateGroup("State Group",
                    GetLocalMousePosition(menuActionEvent.eventInfo.localMousePosition)))
        );

        return contextualMenuManipulator;
    }

    private IManipulator CreateStateItemMenu(string nodeName)
    {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction($"Create State/{nodeName}", menuActionEvent =>
                AddElement(CreateNode(nodeName, GetLocalMousePosition(menuActionEvent.eventInfo.localMousePosition),
                    FSMNodeType.State))));

        return contextualMenuManipulator;
    }

    private IManipulator CreateTransitionItemMenu(string transitionName)
    {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction($"Create Transition/{transitionName}", menuActionEvent =>
                AddElement(CreateNode(transitionName,
                    GetLocalMousePosition(menuActionEvent.eventInfo.localMousePosition), FSMNodeType.Transition))));

        return contextualMenuManipulator;
    }
    private IManipulator CreateDualTransitionStateItemMenu(string transitionName)
    {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction($"Create Dual Transition/{transitionName}", menuActionEvent =>
                AddElement(CreateNode(transitionName,
                    GetLocalMousePosition(menuActionEvent.eventInfo.localMousePosition), FSMNodeType.DualTransition))));

        return contextualMenuManipulator;
    }

    public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
    {
        Vector2 worldMousePosition = mousePosition;
        if (isSearchWindow)
        {
            worldMousePosition -= _window.position.position;
        }

        Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);
        return localMousePosition;
    }

    public void ClearGraph()
    {
        graphElements.ForEach(graphElement => RemoveElement(graphElement));
        _groups.Clear();
        _groupedNodes.Clear();
        _ungroupedNodes.Clear();

        repeatedNameCount = 0;
    }

    public void ToggleMiniMap()
    {
        _miniMap.visible = !_miniMap.visible;
    }

    public FSMEditorWindow GetWindow()
    {
        return _window;
    }
}
