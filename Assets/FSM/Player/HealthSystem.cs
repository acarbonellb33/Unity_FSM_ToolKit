namespace FSM.Player
{
    using UnityEngine;
    /// <summary>
    /// Manages the health system of a player character.
    /// </summary>
    public class HealthSystem : MonoBehaviour
    {
        public float maxHealth = 100f;
        private float _currentHealth;

        [SerializeField] private HealthBar healthBar;

        void Start()
        {
            _currentHealth = maxHealth;
            healthBar.UpdateHealthBar(maxHealth, _currentHealth);
        }
        /// <summary>
        /// Inflicts damage to the player.
        /// </summary>
        /// <param name="amount">Amount of damage to inflict.</param>
        public void TakeDamage(float amount)
        {
            _currentHealth -= amount;
            if (_currentHealth <= 0) Die();
            healthBar.UpdateHealthBar(maxHealth, _currentHealth);
        }
        /// <summary>
        /// Heals the player to full health.
        /// </summary>
        public void Heal()
        {
            _currentHealth = maxHealth;
        }

        private void Die()
        {
            //Destroy(gameObject);
        }

        #region Getters
        /// <summary>
        /// Returns the maximum health value.
        /// </summary>
        public float GetMaxHealth()
        {
            return maxHealth;
        }
        /// <summary>
        /// Returns the current health value.
        /// </summary>
        public float GetCurrentHealth()
        {
            return _currentHealth;
        }

        #endregion
    }
}