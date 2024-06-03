using UnityEngine;

namespace FSM.Nodes.States.StateScripts
{
    public class VariableConditionScript : StateScript, IAction
    {
        private StateScript _selectedStateScript;
        public string selectedVariable;
        public string value;
        
        public VariableConditionScript()
        {
            SetStateName("Variable");
        }
        
        public void Execute()
        {
            if (_selectedStateScript != null && selectedVariable != null)
            {
                _selectedStateScript.SetVariableValue(selectedVariable, value);
            }
        }
        public void SetStateScript(StateScript stateScript)
        {
            _selectedStateScript = stateScript;
        }
    }
}