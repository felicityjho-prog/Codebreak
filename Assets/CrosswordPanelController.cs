using UnityEngine;
using System.Collections;

public class CrosswordPanelController : MonoBehaviour
{
    [Header("Crossword Panel")]
    public GameObject crosswordPanel;

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

    // ==========================================
    // CROSSWORD STATE
    // ==========================================

    private bool crosswordOpen = false;

    public bool IsCrosswordOpen
    {
        get { return crosswordOpen; }
    }

    // ==========================================
    // SAVED CAMERA ROTATION
    // ==========================================

    private Quaternion savedCameraRotation;

    // ==========================================
    // START
    // ==========================================

    private void Start()
    {
        crosswordOpen = false;

        if (completionBanner != null)
        {
            completionBanner.SetActive(false);
        }
    }

    // ==========================================
    // UPDATE
    // ==========================================

    private void Update()
    {
        if (crosswordOpen)
        {
            Cursor.lockState =
                CursorLockMode.None;

            Cursor.visible = true;

            if (playerCamera != null)
            {
                playerCamera.localRotation =
                    savedCameraRotation;
            }
        }
    }

    // ==========================================
    // OPEN CROSSWORD
    // ==========================================

    public void OpenCrossword()
    {
        Debug.Log(
            "CROSSWORD OPENED - PLAYER FROZEN"
        );

        crosswordOpen = true;

        // SAVE CAMERA ROTATION
        if (playerCamera != null)
        {
            savedCameraRotation =
                playerCamera.localRotation;
        }

        // SHOW PANEL
        if (crosswordPanel != null)
        {
            crosswordPanel.SetActive(true);
        }

        // HIDE PROMPT
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }

        // FREEZE WASD
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // FREEZE CAMERA
        if (playerLook != null)
        {
            playerLook.DisableLook();
        }

        // UNLOCK CURSOR
        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;
    }

    // ==========================================
    // CLOSE CROSSWORD
    // ==========================================

    public void CloseCrossword()
    {
        Debug.Log(
            "CROSSWORD CLOSED - PLAYER CONTROL RESTORED"
        );

        crosswordOpen = false;

        if (crosswordPanel != null)
        {
            crosswordPanel.SetActive(false);
        }

        if (playerCamera != null)
        {
            playerCamera.localRotation =
                savedCameraRotation;
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        if (playerLook != null)
        {
            playerLook.EnableLook();
        }

        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;
    }

    // ==========================================
    // FORCE CLOSE CROSSWORD
    // IMPORTANT FOR GAME OVER / TRY AGAIN
    // ==========================================

    public void ForceCloseCrossword()
    {
        Debug.Log(
            "FORCE CLOSING CROSSWORD"
        );

        // IMPORTANT:
        // This resets the internal state.
        crosswordOpen = false;

        // CLOSE PANEL
        if (crosswordPanel != null)
        {
            crosswordPanel.SetActive(false);
        }

        // HIDE PROMPT
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }

        // RESTORE WASD
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        // RESTORE CAMERA
        if (playerLook != null)
        {
            playerLook.EnableLook();
        }

        // LOCK CURSOR
        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;

        Debug.Log(
            "CROSSWORD FORCE CLOSE COMPLETE"
        );
    }

    // ==========================================
    // COMPLETE CROSSWORD
    // ==========================================

    public void CompleteCrossword()
    {
        Debug.Log(
            "CROSSWORD COMPLETED!"
        );

        CloseCrossword();

        if (successAudio != null)
        {
            successAudio.Play();
        }

        if (ChallengeProgressManager.Instance != null)
        {
            ChallengeProgressManager.Instance
                .CompleteTable(2);
        }

        ShowCompletionBanner();
    }

    // ==========================================
    // SHOW COMPLETION BANNER
    // ==========================================

    public void ShowCompletionBanner()
    {
        if (completionBanner == null)
        {
            Debug.LogWarning(
                "Completion Banner is NOT assigned!"
            );

            return;
        }

        completionBanner.SetActive(true);

        Debug.Log(
            "COMPLETION BANNER SHOWN FOR "
            + bannerDuration
            + " SECONDS"
        );

        StartCoroutine(
            HideCompletionBanner()
        );
    }

    // ==========================================
    // HIDE BANNER
    // ==========================================

    private IEnumerator HideCompletionBanner()
    {
        yield return new WaitForSeconds(
            bannerDuration
        );

        if (completionBanner != null)
        {
            completionBanner.SetActive(false);
        }

        Debug.Log(
            "COMPLETION BANNER HIDDEN"
        );

        if (instructionManager != null)
        {
            instructionManager
                .ShowFinalChallengeInstruction();
        }
    }
}