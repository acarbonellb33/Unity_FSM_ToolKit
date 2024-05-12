namespace FSM.Player
{
    using UnityEngine;

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

        public void TakeDamage(float amount)
        {
            _currentHealth -= amount;
            if (_currentHealth <= 0) Die();
            healthBar.UpdateHealthBar(maxHealth, _currentHealth);
        }

        public void Heal()
        {
            _currentHealth = maxHealth;
        }

        private void Die()
        {
            //Destroy(gameObject);
        }

        #region Getters

        public float GetMaxHealth()
        {
            return maxHealth;
        }

        public float GetCurrentHealth()
        {
            return _currentHealth;
        }

        #endregion
    }
}