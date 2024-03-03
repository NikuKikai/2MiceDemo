using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveControl : MonoBehaviour
{
    [SerializeField] float speed = 12f;
    [SerializeField] float gravity = -9.8f * 2;
    [SerializeField] float jumpHeight = 3f;

    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] LayerMask groundMask;

    CharacterController controller;
    Vector3 velocity;
    bool isGrounded;
    bool isMoving;
    Vector3 lastPosition = new Vector3(0f, 0f, 0f);


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

        // ==== Default Key Input
        // float x = Input.GetAxis("Horizontal");
        // float z = Input.GetAxis("Vertical");
        // InputXYZDelta_Mouse(new Vector3(x, 0, z));

        // // Jump
        // if (Input.GetButtonDown("Jump") && isGrounded) {
        //     velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        // }

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

    public void InputXYZDelta_Face(Vector3 v)
    {
        UpdateDelta(v * 2);
    }
    public void InputXYZDelta_Key(Vector3 v)
    {
        UpdateDelta(v);
    }
    // for space key jump
    public void InputKey(KeyCode key)
    {
        if (key == KeyCode.Space) {
            UpdateDelta(new Vector3(0, 1, 0));
        }
    }

    void UpdateDelta(Vector3 v)
    {
        var move = transform.right * v.x + transform.forward * v.z;
        controller.Move(move * speed * Time.deltaTime);

        // jump
        if (v.y > 0.1f && isGrounded) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
