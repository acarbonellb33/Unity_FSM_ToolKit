#if UNITY_EDITOR
namespace FSM.Nodes.States.StateScripts
{
    using UnityEngine;
    using Enemy;
    using Enumerations;
    // EnemyHealthCondition class inherits from MonoBehaviour
    public class HealthConditionScript : StateScript, ICondition
    {
        // Public reference to the enemy's health component
        private EnemyHealthSystem _enemyHealthSystem;

        // Public variable to define the minimum health threshold for the condition to be true
        public FsmOperands operand;
        public float health = 10f;

        public HealthConditionScript()
        {
            // Set the state name to "Health" using the SetStateName method inherited from StateScript
            SetStateName("Health");
        }

        // Start is called before the first frame update
        void Start()
        {
            // Check if the enemy health component is assigned
            if (TryGetComponent(out _enemyHealthSystem))
            {
                Debug.LogError("EnemyHealthSystem component is not added in the Enemy GameObject");
            }
        }

        // Implementation of the Condition method from the ICondition interface
        public bool Condition()
        {
            switch (operand)
            {
                case FsmOperands.LessThan:
                    return _enemyHealthSystem.GetCurrentHealth() < health;
                case FsmOperands.GreaterThan:
                    return _enemyHealthSystem.GetCurrentHealth() > health;
                case FsmOperands.EqualTo:
                    return _enemyHealthSystem.GetCurrentHealth().Equals(health);
                case FsmOperands.NotEqualTo:
                    return !_enemyHealthSystem.GetCurrentHealth().Equals(health);
                default:
                    return false;
            }
        }
    }
}
#endif