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

        // The timer must not run while instructions are open.
        if (room3Timer != null)
            room3Timer.StopTimer();

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

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerController != null)
        {
            playerController.enabled = true;
            playerController.EnableLook();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Start the 3-minute timer only after OKAY.
        if (room3Timer != null)
            room3Timer.StartTimer();
        else
            Debug.LogWarning(
                "Room3ChallengeTimer is not assigned.",
                this
            );

        Debug.Log("ROOM 3 INSTRUCTIONS CLOSED.");
    }
}