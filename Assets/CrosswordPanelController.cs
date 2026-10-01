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
        // Make sure banner is hidden when game starts
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
            // Keep cursor available
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Keep camera completely still
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
        Debug.Log("CROSSWORD OPENED - PLAYER FROZEN");

        crosswordOpen = true;

        // --------------------------------------
        // SAVE CAMERA ROTATION
        // --------------------------------------

        if (playerCamera != null)
        {
            savedCameraRotation =
                playerCamera.localRotation;
        }

        // --------------------------------------
        // SHOW CROSSWORD PANEL
        // --------------------------------------

        if (crosswordPanel != null)
        {
            crosswordPanel.SetActive(true);
        }

        // --------------------------------------
        // HIDE INTERACTION PROMPT
        // --------------------------------------

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }

        // --------------------------------------
        // DISABLE WASD
        // --------------------------------------

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // --------------------------------------
        // DISABLE CAMERA LOOK
        // --------------------------------------

        if (playerLook != null)
        {
            playerLook.DisableLook();
        }

        // --------------------------------------
        // UNLOCK CURSOR
        // --------------------------------------

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ==========================================
    // CLOSE CROSSWORD
    // ==========================================

    public void CloseCrossword()
    {
        Debug.Log("CROSSWORD CLOSED - PLAYER CONTROL RESTORED");

        crosswordOpen = false;

        // --------------------------------------
        // HIDE CROSSWORD PANEL
        // --------------------------------------

        if (crosswordPanel != null)
        {
            crosswordPanel.SetActive(false);
        }

        // --------------------------------------
        // RESTORE CAMERA
        // --------------------------------------

        if (playerCamera != null)
        {
            playerCamera.localRotation =
                savedCameraRotation;
        }

        // --------------------------------------
        // ENABLE WASD
        // --------------------------------------

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        // --------------------------------------
        // ENABLE CAMERA LOOK
        // --------------------------------------

        if (playerLook != null)
        {
            playerLook.EnableLook();
        }

        // --------------------------------------
        // LOCK CURSOR
        // --------------------------------------

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ==========================================
    // COMPLETE CROSSWORD
    // ==========================================

    public void CompleteCrossword()
    {
        Debug.Log("CROSSWORD COMPLETED!");

        // --------------------------------------
        // CLOSE CROSSWORD
        // --------------------------------------

        CloseCrossword();

        // --------------------------------------
        // PLAY SUCCESS AUDIO
        // --------------------------------------

        if (successAudio != null)
        {
            successAudio.Play();
        }

        // --------------------------------------
        // SHOW COMPLETION BANNER
        // --------------------------------------

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

        // Show banner
        completionBanner.SetActive(true);

        Debug.Log(
            "COMPLETION BANNER SHOWN FOR "
            + bannerDuration
            + " SECONDS"
        );

        // Start timer from THIS active controller
        StartCoroutine(HideCompletionBanner());
    }

    // ==========================================
    // HIDE COMPLETION BANNER
    // ==========================================

    private IEnumerator HideCompletionBanner()
    {
        yield return new WaitForSeconds(bannerDuration);

        if (completionBanner != null)
        {
            completionBanner.SetActive(false);
        }

        Debug.Log("COMPLETION BANNER HIDDEN");
    }
}