namespace FSM.Enemy
{
    using UnityEngine;

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

        public void TakeDamage(float amount)
        {
            _currentHealth -= amount;
            if (_currentHealth <= 0) Die();
            _enemyHealthBar.UpdateHealthBar(maxHealth, _currentHealth);
        }

        public void Die()
        {
            Destroy(gameObject);
        }

        public float GetCurrentHealth()
        {
            return _currentHealth;
        }

        public float GetPreviousHealth()
        {
            return _previousHealth;
        }

        public void SetPreviousHealth(float health)
        {
            _previousHealth = health;
        }

        public float GetMaxHealth()
        {
            return maxHealth;
        }
    }
}