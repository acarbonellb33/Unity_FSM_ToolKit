#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Elements
{
    using Data.Save;
    using Utilities;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using Windows;
    using FSM.Enumerations;

    /// <summary>
    /// Represents the initial node in an FSM graph, inheriting from <see cref="FsmNode"/>. Used to know the first state to execute in the FSM.
    /// </summary>
    public class FsmInitialNode : FsmNode
    {
        /// <summary>
        /// Initializes the FSM initial node.
        /// </summary>
        /// <param name="nodeName">The name of the node.</param>
        /// <param name="fsmGraphView">The graph view this node belongs to.</param>
        /// <param name="vectorPos">The position of the node in the graph.</param>
        public override void Initialize(string nodeName, FsmGraphView fsmGraphView, Vector2 vectorPos)
        {
            base.Initialize(nodeName, fsmGraphView, vectorPos);
            NodeType = FsmNodeType.Initial;

            var connectionSaveData = new FsmConnectionSaveData()
            {
                Text = "Initial Node",
            };

            Connections.Add(connectionSaveData);

            mainContainer.AddToClassList("fsm-node_main-container-initial");
            extensionContainer.AddToClassList("fsm-node_extension-container");
        }

        /// <summary>
        /// Draws the node, adding a label for the state name and configuring output ports.
        /// </summary>
        public override void Draw()
        {
            var stateNameField = FsmElementUtility.CreateLabel(StateName, callback =>
            {
                StateName = callback.newValue;
            });

            stateNameField.AddClasses("fsm-node_label");
            titleContainer.Insert(0, stateNameField);

            foreach (var connection in Connections)
            {
                OutputPort = this.CreatePort(connection.Text, Orientation.Horizontal, Direction.Output);
                if (!OutputPort.connected)
                {
                    OutputPort.portColor = Color.red;
                }

                OutputPort.userData = connection;

                outputContainer.Add(OutputPort);
                outputContainer.AddToClassList("fsm-node_input-output-container");
            }

            RefreshExpandedState();
        }
    }
}
#endif
