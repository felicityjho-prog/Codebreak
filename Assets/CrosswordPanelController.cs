
using UnityEngine;
using System.Collections;

public class CrosswordPanelController : MonoBehaviour
{
    [Header("Crossword Panel")]
    public GameObject crosswordPanel;

    [Header("Knowledge Archive")]
    public GameObject knowledgeArchivePanel;
    public GameObject knowledgeArchiveButton;

    [Header("New Knowledge Archive System")]
    public KnowledgeArchiveController archiveController;

    [Header("Completion Banner")]
    public GameObject completionBanner;

    [Header("Success Audio")]
    public AudioSource successAudio;

    [Header("Banner Settings")]
    public float bannerDuration = 2f;

    [Header("Player Control")]
    public PlayerMovement playerMovement;
    public PlayerController playerLook;

    [Header("Camera")]
    public Transform playerCamera;

    [Header("Interaction Prompt")]
    public GameObject interactPrompt;

    [Header("Room 2 Instruction")]
    public Room2InstructionManager instructionManager;

    private bool crosswordOpen = false;
    private bool knowledgeArchiveOpen = false;
    private bool challengeFinished = false;
    private bool table3Unlocked = false;

    private bool playerMovementWasEnabled;
    private bool playerLookWasEnabled;
    private bool timerWasRunningBeforeArchive;

    private Quaternion savedCameraRotation;

    public bool IsCrosswordOpen
    {
        get { return crosswordOpen; }
    }

    public bool IsKnowledgeArchiveOpen
    {
        get { return knowledgeArchiveOpen; }
    }

    // ==========================================
    // FIND KNOWLEDGE ARCHIVE
    // ==========================================

    private void FindKnowledgeArchive()
    {
        if (knowledgeArchivePanel != null)
            return;

        GameObject[] allObjects =
            FindObjectsByType<GameObject>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "KnowledgeArchivePanel")
            {
                knowledgeArchivePanel = obj;

                Debug.Log(
                    "Knowledge Archive found: " + obj.name
                );

                return;
            }
        }

        Debug.LogError(
            "KnowledgeArchivePanel could not be found!"
        );
    }

    // ==========================================
    // START
    // ==========================================

    private void Start()
    {
        crosswordOpen = false;
        knowledgeArchiveOpen = false;
        challengeFinished = false;
        table3Unlocked = false;

        FindKnowledgeArchive();

        if (completionBanner != null)
            completionBanner.SetActive(false);

        if (knowledgeArchivePanel != null)
            knowledgeArchivePanel.SetActive(false);

        if (knowledgeArchiveButton != null)
            knowledgeArchiveButton.SetActive(false);
    }

    // ==========================================
    // UPDATE
    // ==========================================

    private void Update()
    {
        if (crosswordOpen || knowledgeArchiveOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (playerCamera != null)
            {
                playerCamera.localRotation =
                    savedCameraRotation;
            }
        }
    }

    // ==========================================
    // FREEZE PLAYER
    // ==========================================

    private void FreezePlayer()
    {
        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerLook != null)
            playerLook.DisableLook();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ==========================================
    // RESTORE PLAYER
    // ==========================================

    private void RestorePlayer()
    {
        if (playerCamera != null)
            playerCamera.localRotation = savedCameraRotation;

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerLook != null)
            playerLook.EnableLook();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("PLAYER RESTORED - CURSOR LOCKED AND HIDDEN!");
    }

    // ==========================================
    // RESTORE PREVIOUS PLAYER STATE
    // KEEP WORD SHUFFLE STATE
    // ==========================================

    private void RestorePreviousPlayerState()
    {
        if (playerCamera != null)
            playerCamera.localRotation = savedCameraRotation;

        if (playerMovement != null)
        {
            playerMovement.enabled =
                playerMovementWasEnabled;
        }

        if (playerLook != null)
        {
            if (playerLookWasEnabled)
                playerLook.EnableLook();
            else
                playerLook.DisableLook();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log(
            "PREVIOUS PLAYER STATE RESTORED - " +
            "CURSOR LOCKED AND HIDDEN!"
        );
    }

    // ==========================================
    // OPEN CROSSWORD
    // ==========================================

    public void OpenCrossword()
    {
        if (challengeFinished)
            return;

        Debug.Log("CROSSWORD OPENED - PLAYER FROZEN");

        crosswordOpen = true;

        if (playerCamera != null)
            savedCameraRotation = playerCamera.localRotation;

        if (crosswordPanel != null)
            crosswordPanel.SetActive(true);

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        // Hide the large instruction panel and
        // fade out the persistent top instruction.
        if (instructionManager != null)
        {
            instructionManager.HideInstruction();
            instructionManager.HidePersistentInstruction();

            Debug.Log(
                "ROOM 2 INSTRUCTION HIDDEN FOR CROSSWORD."
            );
        }

        FreezePlayer();
    }

    // ==========================================
    // CLOSE CROSSWORD
    // ==========================================

    public void CloseCrossword()
    {
        Debug.Log("CROSSWORD CLOSED");

        crosswordOpen = false;

        if (crosswordPanel != null)
            crosswordPanel.SetActive(false);

        RestorePlayer();

        // Show the previous persistent status again
        // only when the Crossword is not completed.
        if (!challengeFinished &&
            instructionManager != null)
        {
            instructionManager.ShowPersistentInstruction();

            Debug.Log(
                "CROSSWORD CLOSED - STATUS INSTRUCTION RESTORED."
            );
        }
    }

    // ==========================================
    // FORCE CLOSE
    // FOR GAME OVER / TRY AGAIN
    // ==========================================

    public void ForceCloseCrossword()
    {
        Debug.Log("FORCE CLOSING CROSSWORD");

        StopAllCoroutines();

        crosswordOpen = false;
        knowledgeArchiveOpen = false;

        if (crosswordPanel != null)
            crosswordPanel.SetActive(false);

        if (knowledgeArchivePanel != null)
            knowledgeArchivePanel.SetActive(false);

        if (completionBanner != null)
            completionBanner.SetActive(false);

        if (knowledgeArchiveButton != null)
            knowledgeArchiveButton.SetActive(false);

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        RestorePlayer();

        Debug.Log("CROSSWORD FORCE CLOSE COMPLETE");
    }

    // ==========================================
    // COMPLETE CROSSWORD
    // ==========================================

    public void CompleteCrossword()
    {
        if (challengeFinished)
            return;

        challengeFinished = true;

        Debug.Log("CROSSWORD COMPLETED!");

        crosswordOpen = false;

        if (crosswordPanel != null)
            crosswordPanel.SetActive(false);

        FreezePlayer();

        if (successAudio != null)
            successAudio.Play();

        ShowCompletionBanner();
    }

    // ==========================================
    // SHOW COMPLETION BANNER
    // ==========================================

    public void ShowCompletionBanner()
    {
        if (completionBanner != null)
            completionBanner.SetActive(true);

        Debug.Log("CROSSWORD COMPLETED! SHOWING BANNER.");

        StartCoroutine(HideCompletionBanner());
    }

    // ==========================================
    // HIDE BANNER AND OPEN ARCHIVE
    // ==========================================

    private IEnumerator HideCompletionBanner()
    {
        yield return new WaitForSeconds(bannerDuration);

        if (completionBanner != null)
            completionBanner.SetActive(false);

        Debug.Log("CROSSWORD COMPLETION BANNER FINISHED.");

        if (archiveController != null)
        {
            Debug.Log("OPENING NEW KNOWLEDGE ARCHIVE SYSTEM!");

            archiveController.OpenArchive();
        }
        else
        {
            Debug.Log("USING EXISTING KNOWLEDGE ARCHIVE SYSTEM!");

            OpenKnowledgeArchive();
        }
    }

    // ==========================================
    // OPEN KNOWLEDGE ARCHIVE
    // ==========================================

    public void OpenKnowledgeArchive()
    {
        Debug.Log("KNOWLEDGE ARCHIVE OPEN REQUEST RECEIVED!");

        if (knowledgeArchiveOpen)
        {
            Debug.Log("Knowledge Archive is already open.");
            return;
        }

        FindKnowledgeArchive();

        if (knowledgeArchivePanel == null)
        {
            Debug.LogError(
                "Knowledge Archive Panel is missing!"
            );

            return;
        }

        if (playerMovement != null)
        {
            playerMovementWasEnabled =
                playerMovement.enabled;
        }

        if (playerLook != null)
        {
            playerLookWasEnabled =
                playerLook.IsLookEnabled;
        }

        if (playerCamera != null)
            savedCameraRotation = playerCamera.localRotation;

        timerWasRunningBeforeArchive = false;

        if (Room2ChallengeTimer.Instance != null)
        {
            timerWasRunningBeforeArchive =
                Room2ChallengeTimer.Instance.IsTimerRunning();

            Debug.Log(
                "TIMER BEFORE ARCHIVE: " +
                timerWasRunningBeforeArchive
            );

            if (timerWasRunningBeforeArchive)
                Room2ChallengeTimer.Instance.PauseTimer();
        }
        else
        {
            Debug.LogError(
                "Room2ChallengeTimer Instance is NULL!"
            );
        }

        knowledgeArchiveOpen = true;

        if (knowledgeArchiveButton != null)
            knowledgeArchiveButton.SetActive(false);

        knowledgeArchivePanel.SetActive(true);
        knowledgeArchivePanel.transform.SetAsLastSibling();

        FreezePlayer();

        Debug.Log(
            "KNOWLEDGE ARCHIVE SUCCESSFULLY OPENED!"
        );
    }

    // ==========================================
    // CLOSE KNOWLEDGE ARCHIVE
    // ==========================================

    public void CloseKnowledgeArchive()
    {
        Debug.Log("CLOSE KNOWLEDGE ARCHIVE BUTTON CLICKED!");

        if (knowledgeArchivePanel == null)
        {
            Debug.LogError(
                "Knowledge Archive Panel reference is NULL!"
            );
            return;
        }

        if (!knowledgeArchivePanel.activeInHierarchy)
        {
            Debug.LogWarning(
                "Knowledge Archive Panel is already closed!"
            );
            return;
        }

        knowledgeArchiveOpen = false;
        knowledgeArchivePanel.SetActive(false);

        Debug.Log("KNOWLEDGE ARCHIVE PANEL CLOSED!");

        if (Room2ChallengeTimer.Instance != null)
        {
            Room2ChallengeTimer.Instance.ResumeTimer();

            Debug.Log(
                "TIMER RESUME CHECK: " +
                Room2ChallengeTimer.Instance.IsTimerRunning()
            );
        }
        else
        {
            Debug.LogError(
                "ROOM 2 TIMER INSTANCE IS NULL!"
            );
        }

        if (!table3Unlocked)
        {
            table3Unlocked = true;

            FinishArchiveAndUnlockNextTable();

            Debug.Log("FIRST ARCHIVE CLOSE - TABLE 3 UNLOCKED!");
        }
        else
        {
            RestorePreviousPlayerState();

            Debug.Log(
                "ARCHIVE CLOSED - GAMEPLAY CONTROLS RESTORED!"
            );
        }

        if (knowledgeArchiveButton != null)
        {
            knowledgeArchiveButton.SetActive(true);
            knowledgeArchiveButton.transform.SetAsLastSibling();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("ARCHIVE CLOSE FINISHED - CURSOR HIDDEN!");
    }

    // ==========================================
    // FIRST CLOSE ONLY
    // UNLOCK NEXT TABLE
    // ==========================================

    private void FinishArchiveAndUnlockNextTable()
    {
        RestorePlayer();

        if (ChallengeProgressManager.Instance != null)
        {
            ChallengeProgressManager.Instance.CompleteTable(2);

            Debug.Log(
                "TABLE 2 COMPLETED - NEXT TABLE UNLOCKED!"
            );
        }
        else
        {
            Debug.LogWarning(
                "ChallengeProgressManager Instance is NULL!"
            );
        }

        if (instructionManager != null)
        {
            instructionManager.ShowFinalChallengeInstruction();
        }
        else
        {
            Debug.LogWarning(
                "Room2InstructionManager is NOT assigned!"
            );
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log(
            "FIRST ARCHIVE CLOSE - CURSOR LOCKED AND HIDDEN!"
        );
    }

    // ==========================================
    // RESET FOR ROOM 2 TRY AGAIN
    // ==========================================

    public void ResetForRoomRestart()
    {
        Debug.Log("RESETTING CROSSWORD PANEL CONTROLLER...");

        StopAllCoroutines();

        crosswordOpen = false;
        knowledgeArchiveOpen = false;
        challengeFinished = false;
        table3Unlocked = false;

        if (crosswordPanel != null)
            crosswordPanel.SetActive(false);

        if (completionBanner != null)
            completionBanner.SetActive(false);

        if (knowledgeArchivePanel != null)
            knowledgeArchivePanel.SetActive(false);

        if (knowledgeArchiveButton != null)
            knowledgeArchiveButton.SetActive(false);

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        Debug.Log(
            "CROSSWORD PANEL CONTROLLER RESET COMPLETE!"
        );
    }
}
