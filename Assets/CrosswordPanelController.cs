
using UnityEngine;
using System.Collections;

public class CrosswordPanelController : MonoBehaviour
{
    [Header("Crossword Panel")]
    public GameObject crosswordPanel;

    [Header("Knowledge Archive")]
    public GameObject knowledgeArchivePanel;

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
    // FIND KNOWLEDGE ARCHIVE AUTOMATICALLY
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
                    "Knowledge Archive found automatically: " +
                    obj.name
                );

                return;
            }
        }

        Debug.LogError(
            "KnowledgeArchivePanel could not be found in the scene!"
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

        FindKnowledgeArchive();

        if (completionBanner != null)
            completionBanner.SetActive(false);

        if (knowledgeArchivePanel != null)
            knowledgeArchivePanel.SetActive(false);
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
        {
            playerCamera.localRotation =
                savedCameraRotation;
        }

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerLook != null)
            playerLook.EnableLook();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        {
            savedCameraRotation =
                playerCamera.localRotation;
        }

        if (crosswordPanel != null)
            crosswordPanel.SetActive(true);

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

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

        // Close crossword but keep player frozen.
        crosswordOpen = false;

        if (crosswordPanel != null)
            crosswordPanel.SetActive(false);

        FreezePlayer();

        // Play success audio.
        if (successAudio != null)
            successAudio.Play();

        // Show completion banner.
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

        Debug.Log("OPENING KNOWLEDGE ARCHIVE");

        OpenKnowledgeArchive();
    }

    // ==========================================
    // OPEN KNOWLEDGE ARCHIVE
    // ==========================================

    public void OpenKnowledgeArchive()
    {
        if (!challengeFinished)
            return;

        // Try finding the panel again.
        FindKnowledgeArchive();

        if (knowledgeArchivePanel == null)
        {
            Debug.LogError(
                "Knowledge Archive is still missing! " +
                "Table 3 will NOT be unlocked."
            );

            return;
        }

        knowledgeArchiveOpen = true;

        knowledgeArchivePanel.SetActive(true);

        FreezePlayer();

        Debug.Log(
            "KNOWLEDGE ARCHIVE OPENED - PLAYER MUST READ"
        );
    }

    // ==========================================
    // CLOSE KNOWLEDGE ARCHIVE
    // CONNECT THIS TO CLOSE / CONTINUE BUTTON
    // ==========================================

    public void CloseKnowledgeArchive()
    {
        Debug.Log("CLOSE KNOWLEDGE ARCHIVE BUTTON CLICKED!");

        // Close the archive regardless of the current state.
        knowledgeArchiveOpen = false;

        if (knowledgeArchivePanel != null)
        {
            knowledgeArchivePanel.SetActive(false);
            Debug.Log("KNOWLEDGE ARCHIVE PANEL CLOSED!");
        }
        else
        {
            Debug.LogError("Knowledge Archive Panel reference is NULL!");
            return;
        }

        FinishArchiveAndUnlockNextTable();
    }

    // ==========================================
    // FINISH ARCHIVE AND UNLOCK NEXT TABLE
    // ==========================================

    private void FinishArchiveAndUnlockNextTable()
    {
        RestorePlayer();

        if (ChallengeProgressManager.Instance != null)
        {
            ChallengeProgressManager.Instance.CompleteTable(2);

            Debug.Log("TABLE 2 COMPLETED - NEXT TABLE UNLOCKED");
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
    }
}
