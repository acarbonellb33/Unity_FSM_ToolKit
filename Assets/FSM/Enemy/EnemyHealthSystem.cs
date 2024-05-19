namespace FSM.Enemy
{
    using UnityEngine;

    /// <summary>
    /// Class responsible for managing the health of an enemy.
    /// </summary>
    public class EnemyHealthSystem : MonoBehaviour
    {
        public float maxHealth = 100f;
        private float _currentHealth;
        private float _previousHealth;
        private EnemyHealthBar _enemyHealthBar;

        private void Awake()
        {
            _enemyHealthBar = GetComponentInChildren<EnemyHealthBar>();
        }

        void Start()
        {
            _currentHealth = maxHealth;
            _previousHealth = _currentHealth;
            _enemyHealthBar.UpdateHealthBar(maxHealth, _currentHealth);
        }

        /// <summary>
        /// Inflicts damage to the enemy and updates the health bar.
        /// </summary>
        /// <param name="amount">Amount of damage to inflict.</param>
        public void TakeDamage(float amount)
        {
            _currentHealth -= amount;
            if (_currentHealth <= 0) Die();
            _enemyHealthBar.UpdateHealthBar(maxHealth, _currentHealth);
        }

        /// <summary>
        /// Destroys the enemy GameObject.
        /// </summary>
        public void Die()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Returns the current health of the enemy.
        /// </summary>
        public float GetCurrentHealth()
        {
            return _currentHealth;
        }

        /// <summary>
        /// Returns the previous health of the enemy.
        /// </summary>
        public float GetPreviousHealth()
        {
            return _previousHealth;
        }

        /// <summary>
        /// Sets the previous health of the enemy.
        /// </summary>
        /// <param name="health">The previous health value to set.</param>
        public void SetPreviousHealth(float health)
        {
            _previousHealth = health;
        }

        /// <summary>
        /// Returns the maximum health of the enemy.
        /// </summary>
        public float GetMaxHealth()
        {
            return maxHealth;
        }
    }
}
