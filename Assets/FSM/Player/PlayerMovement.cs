using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    CharacterController controller;
    public float speed;
    [SerializeField]
    float gravity = -23.81f;

    [SerializeField]
    Transform groundCheck;
    [SerializeField]
    float groundDistance = 0.5f;//radi de la esfera
    [SerializeField]
    LayerMask groundMask;
    
    [SerializeField]
    float jumpHeight = 3f;//radi de la esfera

    Vector3 velocity;
    bool isGrounded;
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.P))
        {
            GameObject.Find("Enemy").GetComponent<EnemyHealthSystem>().TakeDamage(10);
        }
    }
}
