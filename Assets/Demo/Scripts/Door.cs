namespace Demo.Scripts
{
    using UnityEngine;
    using UnityEngine.UI;
    public class Door : MonoBehaviour
    {
        public GameObject infoText;
        
        private Animator _animator;
        private bool _isFirstTime = true;
        private void Start()
        {
            _animator = GetComponent<Animator>();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _animator.SetTrigger("Open");
            }

            if (_isFirstTime)
            {
                infoText.GetComponent<Text>().enabled = true;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _animator.SetTrigger("Close");
            }
            if (_isFirstTime)
            {
                infoText.GetComponent<Text>().enabled = false;
                _isFirstTime = false;
            }
        }
    }
}
