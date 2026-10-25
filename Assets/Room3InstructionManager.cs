
using UnityEngine;

public class Room3InstructionManager : MonoBehaviour
{
    [Header("Instruction Panel")]
    public GameObject instructionPanel;

    [Header("Player Controls")]
    public PlayerMovement playerMovement;
    public PlayerController playerController;

    [Header("Room 3 Timer")]
    public Room3ChallengeTimer room3Timer;

    [Header("Gameplay UI Hidden During Instructions")]
    public GameObject centerDot;
    public GameObject controlsPanel;
    public GameObject pauseButton;
    public GameObject pauseOverlay;

    private void Awake()
    {
        if (instructionPanel != null)
            instructionPanel.SetActive(false);
    }

    public void ShowInstructions()
    {
        if (instructionPanel == null)
        {
            Debug.LogError(
                "Room3InstructionManager: Instruction Panel " +
                "is not assigned.",
                this
            );
            return;
        }

        // Stop the timer while instructions are open.
        if (room3Timer != null)
            room3Timer.StopTimer();

        // Hide the gameplay UI.
        SetGameplayUIVisible(false);

        // Keep the actual pause menu closed.
        if (pauseOverlay != null)
            pauseOverlay.SetActive(false);

        instructionPanel.SetActive(true);

        if (playerMovement != null)
            playerMovement.enabled = false;
        else
            Debug.LogError("PlayerMovement is not assigned.", this);

        if (playerController != null)
        {
            playerController.DisableLook();
            playerController.enabled = false;
        }
        else
        {
            Debug.LogError("PlayerController is not assigned.", this);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("ROOM 3 INSTRUCTIONS SHOWN.");
    }

    public void HideInstructions()
    {
        if (instructionPanel != null)
            instructionPanel.SetActive(false);

        // Restore the gameplay UI after GOT IT is clicked.
        SetGameplayUIVisible(true);

        // Do not open the pause menu automatically.
        if (pauseOverlay != null)
            pauseOverlay.SetActive(false);

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerController != null)
        {
            playerController.enabled = true;
            playerController.EnableLook();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Start the timer only after instructions are closed.
        if (room3Timer != null)
            room3Timer.StartTimer();
        else
            Debug.LogWarning(
                "Room3ChallengeTimer is not assigned.",
                this
            );

        Debug.Log("ROOM 3 INSTRUCTIONS CLOSED.");
    }

    private void SetGameplayUIVisible(bool visible)
    {
        if (centerDot != null)
            centerDot.SetActive(visible);

        if (controlsPanel != null)
            controlsPanel.SetActive(visible);

        if (pauseButton != null)
            pauseButton.SetActive(visible);
    }
}
