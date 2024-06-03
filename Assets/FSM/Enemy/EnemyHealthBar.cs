namespace FSM.Enemy
{
    using UnityEngine;
    using UnityEngine.UI;
    /// <summary>
    /// Class responsible for updating the health bar of an enemy.
    /// </summary>
    public class EnemyHealthBar : MonoBehaviour
    {
        private Slider _slider;
        private Camera _playerCamera;
        private Transform _target;

        void Awake()
        {
            var player = GameObject.FindWithTag("Player");
            _playerCamera = player.GetComponentInChildren<Camera>();
            _target = transform.parent;
            _slider = GetComponent<Slider>();
        }
        /// <summary>
        /// Updates the health bar based on the provided max and current health values.
        /// </summary>
        /// <param name="maxHealth">Maximum health of the enemy.</param>
        /// <param name="currentHealth">Current health of the enemy.</param>
        public void UpdateHealthBar(float maxHealth, float currentHealth)
        {
            _slider.value = currentHealth / maxHealth;
        }

        private void Update()
        {
            transform.rotation = _playerCamera.transform.rotation;
            transform.position = _target.position;
        }
    }
}