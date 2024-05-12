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
        private EnemyHealthSystem enemyHealth;

        // Public variable to define the minimum health threshold for the condition to be true
        public FSMOperands operand;
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
            if (TryGetComponent(out enemyHealth))
            {
                Debug.LogError("EnemyHealthSystem component is not added in the Enemy GameObject");
            }
        }

        // Implementation of the Condition method from the ICondition interface
        public bool Condition()
        {
            switch (operand)
            {
                case FSMOperands.LessThan:
                    return enemyHealth.GetCurrentHealth() < health;
                case FSMOperands.GreaterThan:
                    return enemyHealth.GetCurrentHealth() > health;
                case FSMOperands.EqualTo:
                    return enemyHealth.GetCurrentHealth().Equals(health);
                case FSMOperands.NotEqualTo:
                    return !enemyHealth.GetCurrentHealth().Equals(health);
                default:
                    return false;
            }
        }
    }
}
#endif