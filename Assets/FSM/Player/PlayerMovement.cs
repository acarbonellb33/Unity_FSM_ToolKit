namespace FSM.Player
{
    using UnityEngine;
    /// <summary>
    /// Controls the movement of the player character.
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] CharacterController controller;
        public float speed;
        [SerializeField] float gravity = -23.81f;

        [SerializeField] Transform groundCheck;
        [SerializeField] float groundDistance = 0.5f; //radi de la esfera
        [SerializeField] LayerMask groundMask;

        [SerializeField] float jumpHeight = 3f; //radi de la esfera

        Vector3 _velocity;
        bool _isGrounded;

        void Update()
        {
            _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(move * speed * Time.deltaTime);

            if (Input.GetButtonDown("Jump") && _isGrounded)
            {
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            _velocity.y += gravity * Time.deltaTime;

            controller.Move(_velocity * Time.deltaTime);
        }
    }
}