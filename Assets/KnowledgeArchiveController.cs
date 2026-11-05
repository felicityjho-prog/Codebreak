
using UnityEngine;
using System.Collections;

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

    [Header("Transition Settings")]
    [Min(0.01f)]
    public float fadeInDuration = 0.3f;

    [Min(0.01f)]
    public float fadeOutDuration = 0.25f;

    [Range(0.8f, 1f)]
    public float startingScaleMultiplier = 0.96f;

    private CanvasGroup archiveCanvasGroup;
    private RectTransform archiveRectTransform;

    private Vector3 originalScale;

    private Coroutine archiveTransitionCoroutine;

    private bool archiveOpen = false;
    private bool isTransitioning = false;
    private bool firstArchiveCloseCompleted = false;

    private bool previousMovementState;
    private bool previousLookState;
    private bool previousCenterDotState;

    private bool timerWasRunning;

    private Quaternion previousCameraRotation;

    // ==========================================
    // AWAKE
    // ==========================================

    private void Awake()
    {
        CacheArchiveComponents();
    }

    // ==========================================
    // CACHE ARCHIVE COMPONENTS
    // ==========================================

    private void CacheArchiveComponents()
    {
        if (knowledgeArchivePanel == null)
        {
            Debug.LogError(
                "KnowledgeArchivePanel is not assigned!",
                this
            );

            return;
        }

        archiveCanvasGroup =
            knowledgeArchivePanel.GetComponent<CanvasGroup>();

        // Automatically add CanvasGroup if missing.
        if (archiveCanvasGroup == null)
        {
            archiveCanvasGroup =
                knowledgeArchivePanel.AddComponent<CanvasGroup>();

            Debug.LogWarning(
                "CanvasGroup was automatically added to KnowledgeArchivePanel.",
                this
            );
        }

        archiveRectTransform =
            knowledgeArchivePanel.GetComponent<RectTransform>();

        if (archiveRectTransform != null)
        {
            originalScale = archiveRectTransform.localScale;
        }
    }

    // ==========================================
    // START
    // ==========================================

    private void Start()
    {
        archiveOpen = false;
        isTransitioning = false;
        firstArchiveCloseCompleted = false;

        if (knowledgeArchivePanel != null)
        {
            ResetArchiveVisuals();

            knowledgeArchivePanel.SetActive(false);
        }

        if (knowledgeArchiveButton != null)
            knowledgeArchiveButton.SetActive(false);

        Debug.Log("NEW KNOWLEDGE ARCHIVE CONTROLLER READY!");
    }

    // ==========================================
    // RESET ARCHIVE VISUALS
    // ==========================================

    private void ResetArchiveVisuals()
    {
        if (archiveCanvasGroup != null)
        {
            archiveCanvasGroup.alpha = 1f;
            archiveCanvasGroup.interactable = true;
            archiveCanvasGroup.blocksRaycasts = true;
        }

        if (archiveRectTransform != null)
        {
            archiveRectTransform.localScale = originalScale;
        }
    }

    // ==========================================
    // OPEN ARCHIVE
    // ==========================================

    public void OpenArchive()
    {
        if (archiveOpen || isTransitioning)
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
        {
            previousCameraRotation =
                playerCamera.localRotation;
        }

        // ======================================
        // PAUSE ROOM 2 TIMER
        // ======================================

        timerWasRunning = false;

        if (Room2ChallengeTimer.Instance != null)
        {
            timerWasRunning =
                Room2ChallengeTimer.Instance.IsTimerRunning();

            if (timerWasRunning)
            {
                Room2ChallengeTimer.Instance.PauseTimer();
            }
        }
        else
        {
            Debug.LogError(
                "Room2ChallengeTimer Instance is missing!",
                this
            );
        }

        // ======================================
        // HIDE ARCHIVE BUTTON
        // ======================================

        if (knowledgeArchiveButton != null)
            knowledgeArchiveButton.SetActive(false);

        // ======================================
        // HIDE CENTER DOT
        // ======================================

        if (centerDot != null)
            centerDot.SetActive(false);

        // ======================================
        // FREEZE PLAYER
        // ======================================

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerController != null)
            playerController.DisableLook();

        // ======================================
        // PREPARE ARCHIVE OPENING
        // ======================================

        archiveOpen = true;
        isTransitioning = true;

        knowledgeArchivePanel.SetActive(true);

        knowledgeArchivePanel.transform.SetAsLastSibling();

        if (archiveCanvasGroup != null)
        {
            archiveCanvasGroup.alpha = 0f;
            archiveCanvasGroup.interactable = false;
            archiveCanvasGroup.blocksRaycasts = true;
        }

        if (archiveRectTransform != null)
        {
            archiveRectTransform.localScale =
                originalScale * startingScaleMultiplier;
        }

        // Allow mouse interaction.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Start smooth fade-in.
        archiveTransitionCoroutine =
            StartCoroutine(AnimateArchive(true));

        Debug.Log(
            "KNOWLEDGE ARCHIVE OPENING ANIMATION STARTED!"
        );
    }

    // ==========================================
    // CLOSE ARCHIVE
    // ==========================================

    public void CloseArchive()
    {
        if (!archiveOpen || isTransitioning)
        {
            Debug.LogWarning(
                "Knowledge Archive is not open or is transitioning!",
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

        isTransitioning = true;

        // Prevent clicking buttons during fade-out.
        if (archiveCanvasGroup != null)
        {
            archiveCanvasGroup.interactable = false;
            archiveCanvasGroup.blocksRaycasts = true;
        }

        // Start smooth fade-out.
        archiveTransitionCoroutine =
            StartCoroutine(AnimateArchive(false));

        Debug.Log(
            "KNOWLEDGE ARCHIVE CLOSING ANIMATION STARTED!"
        );
    }

    // ==========================================
    // ARCHIVE ANIMATION
    // OPEN / CLOSE
    // ==========================================

    private IEnumerator AnimateArchive(bool opening)
    {
        float duration = opening
            ? fadeInDuration
            : fadeOutDuration;

        duration = Mathf.Max(0.01f, duration);

        float startAlpha =
            archiveCanvasGroup != null
                ? archiveCanvasGroup.alpha
                : 1f;

        float targetAlpha = opening ? 1f : 0f;

        Vector3 startScale =
            archiveRectTransform != null
                ? archiveRectTransform.localScale
                : originalScale;

        Vector3 targetScale = opening
            ? originalScale
            : originalScale * startingScaleMultiplier;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            // Smooth animation curve.
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // Fade animation.
            if (archiveCanvasGroup != null)
            {
                archiveCanvasGroup.alpha =
                    Mathf.Lerp(
                        startAlpha,
                        targetAlpha,
                        smoothT
                    );
            }

            // Scale animation.
            if (archiveRectTransform != null)
            {
                archiveRectTransform.localScale =
                    Vector3.Lerp(
                        startScale,
                        targetScale,
                        smoothT
                    );
            }

            yield return null;
        }

        // Apply exact final values.
        if (archiveCanvasGroup != null)
        {
            archiveCanvasGroup.alpha = targetAlpha;
        }

        if (archiveRectTransform != null)
        {
            archiveRectTransform.localScale = targetScale;
        }

        archiveTransitionCoroutine = null;
        isTransitioning = false;

        // ======================================
        // OPENING FINISHED
        // ======================================

        if (opening)
        {
            if (archiveCanvasGroup != null)
            {
                archiveCanvasGroup.interactable = true;
                archiveCanvasGroup.blocksRaycasts = true;
            }

            Debug.Log(
                "KNOWLEDGE ARCHIVE OPENING COMPLETE!"
            );
        }

        // ======================================
        // CLOSING FINISHED
        // ======================================

        else
        {
            // Only deactivate after fade-out completes.
            knowledgeArchivePanel.SetActive(false);

            archiveOpen = false;

            FinishArchiveClose();

            Debug.Log(
                "KNOWLEDGE ARCHIVE CLOSING COMPLETE!"
            );
        }
    }

    // ==========================================
    // FINISH ARCHIVE CLOSE
    // EXISTING GAMEPLAY LOGIC
    // ==========================================

    private void FinishArchiveClose()
    {
        Debug.Log("KNOWLEDGE ARCHIVE CLOSED!");

        // Resume only if the timer was running before opening.
        if (timerWasRunning)
        {
            if (Room2ChallengeTimer.Instance != null)
            {
                Room2ChallengeTimer.Instance.ResumeTimer();

                Debug.Log(
                    "ROOM 2 TIMER RESUMED AFTER ARCHIVE."
                );
            }
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

            Debug.Log(
                "FIRST ARCHIVE READING COMPLETED!"
            );
        }

        // ======================================
        // REUSABLE ARCHIVE CLOSE
        // ======================================

        else
        {
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
        {
            playerCamera.localRotation =
                previousCameraRotation;
        }

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
        {
            playerMovement.enabled =
                previousMovementState;
        }

        if (playerController != null)
        {
            playerController.enabled = true;

            if (previousLookState)
                playerController.EnableLook();
            else
                playerController.DisableLook();
        }

        if (centerDot != null)
        {
            centerDot.SetActive(previousCenterDotState);
        }

        if (playerCamera != null)
        {
            playerCamera.localRotation =
                previousCameraRotation;
        }

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
        Debug.Log("RESETTING KNOWLEDGE ARCHIVE...");

        if (archiveTransitionCoroutine != null)
        {
            StopCoroutine(archiveTransitionCoroutine);
            archiveTransitionCoroutine = null;
        }

        archiveOpen = false;
        isTransitioning = false;
        firstArchiveCloseCompleted = false;
        timerWasRunning = false;

        if (knowledgeArchivePanel != null)
        {
            ResetArchiveVisuals();

            knowledgeArchivePanel.SetActive(false);
        }

        if (knowledgeArchiveButton != null)
            knowledgeArchiveButton.SetActive(false);

        Debug.Log(
            "KNOWLEDGE ARCHIVE RESET - BUTTON HIDDEN!"
        );
    }
}
