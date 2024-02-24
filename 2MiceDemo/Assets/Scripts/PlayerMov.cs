using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMov : MonoBehaviour
{
    public float speed = 12f;
    public float gravity = -9.8f * 2;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private CharacterController controller;
    Vector3 velocity;
    bool isGrounded;
    bool isMoving;
    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);


    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }

        // Move
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        InputDeltaXY(x, z);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        // Falling
        velocity.y += gravity * Time.deltaTime;
        // Exec jump
        controller.Move(velocity * Time.deltaTime);

        if (lastPosition != gameObject.transform.position && isGrounded) {
            isMoving = true;
        }
        else {
            isMoving = false;
        }
        lastPosition = gameObject.transform.position;
    }

    public void InputDeltaXY(float dx, float dy)
    {
        var move = transform.right * dx + transform.forward * dy;
        controller.Move(move * speed * Time.deltaTime);
    }
}
