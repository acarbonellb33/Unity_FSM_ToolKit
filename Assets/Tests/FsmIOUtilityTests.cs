using FSM.Nodes.States;

namespace Tests
{
    using NUnit.Framework;
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;
    using System.IO;
    using EditorWindow.FSMSystem.Data.Save;
    using EditorWindow.FSMSystem.Elements;
    using EditorWindow.FSMSystem.Utilities;
    using EditorWindow.FSMSystem.Windows;
    using FSM.Enumerations;
    
    public class FsmIOUtilityTests
    {
        private const string GraphName = "TestGraph";
        private const string ContainerFolderPath = "Assets/FSMSystem/FSMs/TestGraph";
        private FsmGraphView _graphView;
        private string _graphName;
        private string _initialState;
        private FsmHitSaveData _hitData;

        [SetUp]
        public void SetUp()
        {
            // Create an instance of FsmGraphView
            _graphView = new FsmGraphView(ScriptableObject.CreateInstance<FsmEditorWindow>());
            _graphName = "TestGraph";
            _initialState = "InitialState";
            _hitData = new FsmHitSaveData();

            // Initialize FsmIOUtility
            FsmIOUtility.Initialize(_graphName, _graphView, _initialState, _hitData);
        }

        [Test]
        public void TestSave()
        {
            // Arrange: Set up the graph with nodes and connections
            var node = new FsmNode
            {
                Id = "1",
                StateName = "Chase",
                NodeType = FsmNodeType.State,
                Connections = new List<FsmConnectionSaveData>()
            };
            _graphView.AddElement(node);

            // Act: Call the Save method
            bool result = FsmIOUtility.Save();

            // Assert: Verify the Save method result
            Assert.IsTrue(result);
            Assert.IsTrue(Directory.Exists(ContainerFolderPath));
        }

        [Test]
        public void TestLoad()
        {
            // Arrange: Save a graph to load later
            TestSave();
            _graphView.nodes.ToList().ForEach(node => _graphView.RemoveElement(node));

            // Act: Call the Load method
            FsmIOUtility.Load();

            // Assert: Verify the graph is loaded correctly
            Assert.IsNotNull(_graphView);
            Assert.AreEqual(1, _graphView.nodes.ToList().Count);
        }
        
        [Test]
        public void LoadNode_LoadsNodeCorrectly()
        {
            TestSave();
            FsmNodeSaveData node = new FsmNodeSaveData
            {
                Id = "1",
                Name = "Chase",
                NodeType = FsmNodeType.State,
                Connections = new List<FsmConnectionSaveData>(),
                AnimatorSaveData = new FsmAnimatorSaveData(),
                HitNodeSaveData = new FsmHitNodeSaveData()
            };
            FsmNode loadedNode = FsmIOUtility.LoadNode(node, GraphName);
            Assert.AreEqual(((FsmNode)_graphView.nodes.ToList()[0]).Id, loadedNode.Id);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up any created assets to avoid clutter
            AssetDatabase.DeleteAsset($"Assets/EditorWindow/FSMSystem/Graphs/{GraphName}.asset");
            if (Directory.Exists(ContainerFolderPath))
            {
                Directory.Delete(ContainerFolderPath, true);
                File.Delete($"{ContainerFolderPath}.meta");
            }

            AssetDatabase.Refresh();
        }
    }
}