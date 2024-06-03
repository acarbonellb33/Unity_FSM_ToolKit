namespace FSM.Player
{
    using UnityEngine;
    public class RespawnSystem : MonoBehaviour
    {
        public GameObject player;
        public GameObject deathScreen;
        private GameObject _currentCheckpoint;
        private PlayerMovement _playerMovement;
        private HealthSystem _healthSystem;
        private CharacterController _characterController;
        
        private bool _isDead;

        private void Start()
        {
            _playerMovement = player.GetComponent<PlayerMovement>();
            _healthSystem = player.GetComponent<HealthSystem>();
            _characterController = player.GetComponent<CharacterController>();
            _isDead = false;
        }
        private void Update()
        {
            if(_isDead && Input.GetKeyDown(KeyCode.R))
            {
                Respawn();
            }
        }
        private void Respawn()
        {
            _isDead = false;
            player.transform.position = transform.position;
            _playerMovement.enabled = true;
            _characterController.enabled = true;
            _healthSystem.Heal();
            deathScreen.SetActive(false);
        }
        public void ShowDeathScreen()
        {
            deathScreen.SetActive(true);
            _playerMovement.enabled = false;
            _characterController.enabled = false;
            _isDead = true;
        }
    }
}