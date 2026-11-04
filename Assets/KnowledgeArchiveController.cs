
using UnityEngine;

public class KnowledgeArchiveController : MonoBehaviour
{
    [Header("Archive UI")]
    public GameObject knowledgeArchivePanel;
    public GameObject knowledgeArchiveButton;
    public GameObject centerDot;

    [Header("Player Controls")]
    public PlayerMovement playerMovement;
    public PlayerController playerController;
    public Transform playerCamera;

    [Header("Room 2 Instructions")]
    public Room2InstructionManager instructionManager;

    private bool archiveOpen = false;
    private bool firstArchiveCloseCompleted = false;

    private bool previousMovementState;
    private bool previousLookState;
    private bool previousCenterDotState;

    private bool timerWasRunning;

    private Quaternion previousCameraRotation;

    // ==========================================
    // START
    // ==========================================

    private void Start()
    {
        archiveOpen = false;
        firstArchiveCloseCompleted = false;

        if (knowledgeArchivePanel != null)
            knowledgeArchivePanel.SetActive(false);

        if (knowledgeArchiveButton != null)
            knowledgeArchiveButton.SetActive(false);

        Debug.Log("NEW KNOWLEDGE ARCHIVE CONTROLLER READY!");
    }

    // ==========================================
    // OPEN ARCHIVE
    // ==========================================

    public void OpenArchive()
    {
        if (archiveOpen)
            return;

        if (knowledgeArchivePanel == null)
        {
            Debug.LogError(
                "KnowledgeArchivePanel is not assigned!",
                this
            );
            return;
        }

        // Save previous gameplay states.
        previousMovementState =
            playerMovement != null &&
            playerMovement.enabled;

        previousLookState =
            playerController != null &&
            playerController.IsLookEnabled;

        previousCenterDotState =
            centerDot != null &&
            centerDot.activeSelf;

        if (playerCamera != null)
            previousCameraRotation = playerCamera.localRotation;

        // Pause Room 2 timer only if currently running.
        timerWasRunning = false;

        if (Room2ChallengeTimer.Instance != null)
        {
            timerWasRunning =
                Room2ChallengeTimer.Instance.IsTimerRunning();

            if (timerWasRunning)
                Room2ChallengeTimer.Instance.PauseTimer();
        }
        else
        {
            Debug.LogError(
                "Room2ChallengeTimer Instance is missing!",
                this
            );
        }

        // Hide persistent Archive button.
        if (knowledgeArchiveButton != null)
            knowledgeArchiveButton.SetActive(false);

        // Hide center dot while Archive is open.
        if (centerDot != null)
            centerDot.SetActive(false);

        // Freeze player movement.
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Freeze camera look.
        if (playerController != null)
            playerController.DisableLook();

        // Show Archive.
        archiveOpen = true;

        knowledgeArchivePanel.SetActive(true);
        knowledgeArchivePanel.transform.SetAsLastSibling();

        // Allow mouse interaction.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("KNOWLEDGE ARCHIVE OPENED!");
    }

    // ==========================================
    // CLOSE ARCHIVE
    // ==========================================

    public void CloseArchive()
    {
        if (!archiveOpen)
        {
            Debug.LogWarning(
                "Knowledge Archive is not currently open!",
                this
            );
            return;
        }

        if (knowledgeArchivePanel == null)
        {
            Debug.LogError(
                "KnowledgeArchivePanel is missing!",
                this
            );
            return;
        }

        archiveOpen = false;

        knowledgeArchivePanel.SetActive(false);

        Debug.Log("KNOWLEDGE ARCHIVE CLOSED!");

        // Resume only if the timer was running before opening.
        if (timerWasRunning)
        {
            if (Room2ChallengeTimer.Instance != null)
                Room2ChallengeTimer.Instance.ResumeTimer();
        }

        timerWasRunning = false;

        // ======================================
        // FIRST CLOSE ONLY
        // ======================================

        if (!firstArchiveCloseCompleted)
        {
            firstArchiveCloseCompleted = true;

            if (ChallengeProgressManager.Instance != null)
            {
                ChallengeProgressManager.Instance.CompleteTable(2);
            }
            else
            {
                Debug.LogWarning(
                    "ChallengeProgressManager Instance is missing!"
                );
            }

            // Restore normal gameplay controls.
            RestoreNormalGameplay();

            // Show Table 3 instruction.
            if (instructionManager != null)
            {
                instructionManager.ShowFinalChallengeInstruction();
            }
            else
            {
                Debug.LogWarning(
                    "Room2InstructionManager is not assigned!"
                );
            }

            Debug.Log("FIRST ARCHIVE READING COMPLETED!");
        }
        else
        {
            // Reopening Archive must preserve Word Shuffle.
            RestorePreviousGameplayState();

            Debug.Log(
                "ARCHIVE CLOSED - PREVIOUS GAMEPLAY STATE RESTORED!"
            );
        }

        // Show reusable Archive button.
        if (knowledgeArchiveButton != null)
        {
            knowledgeArchiveButton.SetActive(true);
            knowledgeArchiveButton.transform.SetAsLastSibling();
        }

        // Always return to normal cursor mode.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("ARCHIVE CLOSE COMPLETE!");
    }

    // ==========================================
    // RESTORE NORMAL GAMEPLAY
    // FIRST CLOSE
    // ==========================================

    private void RestoreNormalGameplay()
    {
        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerController != null)
        {
            playerController.enabled = true;
            playerController.EnableLook();
        }

        if (centerDot != null)
            centerDot.SetActive(true);

        if (playerCamera != null)
            playerCamera.localRotation = previousCameraRotation;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("NORMAL ROOM 2 GAMEPLAY RESTORED!");
    }

    // ==========================================
    // RESTORE PREVIOUS GAMEPLAY STATE
    // REUSABLE ARCHIVE
    // ==========================================

    private void RestorePreviousGameplayState()
    {
        if (playerMovement != null)
            playerMovement.enabled = previousMovementState;

        if (playerController != null)
        {
            playerController.enabled = true;

            if (previousLookState)
                playerController.EnableLook();
            else
                playerController.DisableLook();
        }

        if (centerDot != null)
            centerDot.SetActive(previousCenterDotState);

        if (playerCamera != null)
            playerCamera.localRotation = previousCameraRotation;

        // Hide the cursor after closing Archive.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ==========================================
    // CHECK ARCHIVE STATE
    // ==========================================

    public bool IsArchiveOpen()
    {
        return archiveOpen;
    }

    public bool HasCompletedFirstArchiveReading()
    {
        return firstArchiveCloseCompleted;
    }


    // ==========================================
    // RESET ARCHIVE FOR ROOM 2 RESTART
    // ==========================================

    public void ResetForRoomRestart()
    {
        archiveOpen = false;
        firstArchiveCloseCompleted = false;
        timerWasRunning = false;

        if (knowledgeArchivePanel != null)
            knowledgeArchivePanel.SetActive(false);

        if (knowledgeArchiveButton != null)
            knowledgeArchiveButton.SetActive(false);

        Debug.Log(
            "KNOWLEDGE ARCHIVE RESET - BUTTON HIDDEN!"
        );
    }

}
