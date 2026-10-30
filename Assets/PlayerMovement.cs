
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float gravity = -9.81f;

    [Header("Character Animation")]
    [SerializeField] private Animator playerAnimator;

    private CharacterController controller;
    private float yVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Find the character's Animator if it was not assigned.
        if (playerAnimator == null)
        {
            playerAnimator = GetComponentInChildren<Animator>();
        }

        if (playerAnimator == null)
        {
            Debug.LogWarning(
                "PlayerMovement: No Animator found on Player or its children.",
                this
            );
        }
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Check whether the player is actually moving.
        bool hasMovement =
            new Vector2(x, z).sqrMagnitude > 0.01f;

        // Running requires movement plus Left Shift.
        bool isRunningNow =
            hasMovement && Input.GetKey(KeyCode.LeftShift);

        // Walking means moving without running.
        bool isWalkingNow =
            hasMovement && !isRunningNow;

        // Update the existing Animator parameters.
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isWalking", isWalkingNow);
            playerAnimator.SetBool("isRunning", isRunningNow);
        }

        // Preserve the existing movement speed behavior.
        float speed = Input.GetKey(KeyCode.LeftShift)
            ? runSpeed
            : walkSpeed;

        Vector3 move = transform.right * x
                     + transform.forward * z;

        // Preserve the existing ground handling.
        if (controller.isGrounded)
        {
            if (yVelocity < 0)
            {
                yVelocity = -2f;
            }
        }

        // Gravity.
        yVelocity += gravity * Time.deltaTime;
        move.y = yVelocity;

        // Move with CharacterController collision handling.
        controller.Move(move * speed * Time.deltaTime);
    }
}