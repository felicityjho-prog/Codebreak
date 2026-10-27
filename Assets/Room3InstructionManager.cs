using UnityEngine;

public class Room3InstructionManager : MonoBehaviour
{
    [Header("Instruction Panel")]
    public GameObject instructionPanel;

    [Header("Instruction AI Voice")]
    public AudioSource instructionVoice;

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

    private bool voicePlayedForCurrentInstructions = false;

    private void Awake()
    {
        if (instructionPanel != null)
            instructionPanel.SetActive(false);

        if (instructionVoice == null)
            instructionVoice = GetComponent<AudioSource>();

        if (instructionVoice != null)
            instructionVoice.Stop();
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

        // Hide gameplay UI.
        SetGameplayUIVisible(false);

        // Keep pause menu closed.
        if (pauseOverlay != null)
            pauseOverlay.SetActive(false);

        instructionPanel.SetActive(true);

        // Play the instruction voice once per opening.
        if (!voicePlayedForCurrentInstructions)
        {
            if (instructionVoice != null &&
                instructionVoice.clip != null)
            {
                instructionVoice.Stop();
                instructionVoice.Play();

                voicePlayedForCurrentInstructions = true;

                Debug.Log("ROOM 3 INSTRUCTION VOICE PLAYING.");
            }
            else
            {
                Debug.LogWarning(
                    "Room 3 instruction voice or AudioClip " +
                    "is not assigned.",
                    this
                );
            }
        }

        // Freeze player movement.
        if (playerMovement != null)
            playerMovement.enabled = false;
        else
            Debug.LogError("PlayerMovement is not assigned.", this);

        // Freeze camera look.
        if (playerController != null)
        {
            playerController.DisableLook();
            playerController.enabled = false;
        }
        else
        {
            Debug.LogError("PlayerController is not assigned.", this);
        }

        // Allow cursor movement and UI interaction.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("ROOM 3 INSTRUCTIONS SHOWN.");
    }

    public void HideInstructions()
    {
        if (instructionPanel != null)
            instructionPanel.SetActive(false);

        // IMPORTANT:
        // Do not stop the AudioSource here.
        // The AI voice will continue playing after
        // the instruction panel is closed.

        // Allow the voice to play again if instructions
        // are opened another time.
        voicePlayedForCurrentInstructions = false;

        // Restore gameplay UI.
        SetGameplayUIVisible(true);

        // Keep pause menu closed.
        if (pauseOverlay != null)
            pauseOverlay.SetActive(false);

        // Restore player movement.
        if (playerMovement != null)
            playerMovement.enabled = true;

        // Restore camera look.
        if (playerController != null)
        {
            playerController.enabled = true;
            playerController.EnableLook();
        }

        // Lock cursor for gameplay.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Start timer after instructions are closed.
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