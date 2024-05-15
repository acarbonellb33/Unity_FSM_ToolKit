namespace FSM.Enemy
{
    using UnityEngine;
    using UnityEngine.UI;

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

        public void UpdateHealthBar(float maxHealth, float currentHealth)
        {
            _slider.value = currentHealth / maxHealth;
        }

        private void Update()
        {
            transform.rotation = _playerCamera.transform.rotation;
            transform.position = _target.position + new Vector3(0, 2, 0);
        }
    }
}