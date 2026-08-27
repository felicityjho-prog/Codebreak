using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private float yVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        Vector3 move = transform.right * x + transform.forward * z;

        // ✅ Ground handling (fix sinking & clipping)
        if (controller.isGrounded)
        {
            if (yVelocity < 0)
                yVelocity = -2f; // keeps player grounded
        }

        // gravity
        yVelocity += gravity * Time.deltaTime;

        move.y = yVelocity;

        // ✅ Move (with collision)
        controller.Move(move * speed * Time.deltaTime);
    }
}