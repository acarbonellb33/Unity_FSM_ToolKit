#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Elements
{
    using Data.Save;
    using Utilities;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using Windows;
    using FSM.Enumerations;
    public class FsmInitialNode : FsmNode
    {
        public override void Initialize(string nodeName, FsmGraphView fsmGraphView, Vector2 vectorPos)
        {
            base.Initialize(nodeName, fsmGraphView, vectorPos);
            NodeType = FsmNodeType.Initial;
            var connectionSaveData = new FsmConnectionSaveData()
            {
                Text = "Initial Node",
            };
            Choices.Add(connectionSaveData);
            mainContainer.AddToClassList("fsm-node_main-container-initial");
            extensionContainer.AddToClassList("fsm-node_extension-container");
        }

        public override void Draw()
        {
            var stateNameField = FsmElementUtility.CreateLabel(StateName, callback =>
            {
                StateName = callback.newValue;
            });
            stateNameField.AddClasses("fsm-node_label");
            titleContainer.Insert(0, stateNameField);

            foreach (var connection in Choices)
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