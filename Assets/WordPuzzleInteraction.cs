
using UnityEngine;
using System.Collections;
using TMPro;

public class WordPuzzleInteraction : MonoBehaviour
{
    [Header("Interaction")]
    public float interactionDistance = 3f;
    public KeyCode interactionKey = KeyCode.E;
    public int tableNumber = 3;

    [Header("Player Controls")]
    public PlayerMovement playerMovement;
    public PlayerController playerController;

    [Header("Word Puzzle UI")]
    public GameObject wordScrabblePanel;

    [Header("Interaction Prompt - E Icon")]
    public GameObject interactPrompt;
    public CanvasGroup promptCanvasGroup;
    public float fadeDuration = 0.25f;

    [Header("Locked Message - Text Only")]
    public TMP_Text lockedMessageText;

    [TextArea(2, 3)]
    public string lockedMessage =
        "PREVIOUS CHALLENGE REQUIRED\nCOMPLETE TABLE 2 FIRST.";

    private Transform player;
    private bool isNear;
    private bool isOpen;

    private bool movementWasEnabled;
    private bool movementStateSaved;

    private bool hasPromptState;
    private bool lastKnownUnlocked;

    private Coroutine fadeCoroutine;

    // ==========================================
    // START
    // ==========================================

    private void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;

            if (playerMovement == null)
            {
                playerMovement =
                    playerObject.GetComponent<PlayerMovement>();
            }

            if (playerController == null)
            {
                playerController =
                    playerObject.GetComponent<PlayerController>();
            }
        }

        if (wordScrabblePanel != null)
        {
            wordScrabblePanel.SetActive(false);
        }

        HideAllPrompts();
    }

    // ==========================================
    // UPDATE
    // ==========================================

    private void Update()
    {
        if (IsTableCompleted())
        {
            isNear = false;
            hasPromptState = false;
            HideAllPrompts();
            return;
        }

        if (isOpen || player == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        // PLAYER FAR FROM TABLE
        if (distance > interactionDistance)
        {
            isNear = false;
            hasPromptState = false;
            HideAllPrompts();
            return;
        }

        // PLAYER NEAR TABLE
        bool unlocked = IsTableUnlocked();

        // Update UI when entering range or when
        // the table changes from locked to unlocked.
        if (!isNear ||
            !hasPromptState ||
            unlocked != lastKnownUnlocked)
        {
            isNear = true;
            hasPromptState = true;
            lastKnownUnlocked = unlocked;

            if (unlocked)
            {
                ShowUnlockedPrompt();
            }
            else
            {
                ShowLockedMessage();
            }
        }

        // PRESS INTERACTION KEY
        if (Input.GetKeyDown(interactionKey))
        {
            if (IsTableUnlocked())
            {
                OpenWordPuzzle();
            }
            else
            {
                Debug.Log(
                    "TABLE 3 LOCKED: Complete Table 2 first."
                );
            }
        }
    }

    // ==========================================
    // CHECK IF TABLE IS UNLOCKED
    // ==========================================

    private bool IsTableUnlocked()
    {
        if (ChallengeProgressManager.Instance == null)
            return false;

        return ChallengeProgressManager.Instance
            .IsTableUnlocked(tableNumber);
    }

    // ==========================================
    // CHECK IF TABLE IS COMPLETED
    // ==========================================

    private bool IsTableCompleted()
    {
        if (ChallengeProgressManager.Instance == null)
            return false;

        return ChallengeProgressManager.Instance
            .IsTableCompleted(tableNumber);
    }

    // ==========================================
    // SHOW E ICON - UNLOCKED
    // ==========================================

    private void ShowUnlockedPrompt()
    {
        HideLockedMessage();

        if (IsTableCompleted() ||
            !IsTableUnlocked() ||
            isOpen)
        {
            HidePromptImmediately();
            return;
        }

        if (interactPrompt == null)
            return;

        interactPrompt.SetActive(true);

        if (promptCanvasGroup != null)
        {
            promptCanvasGroup.interactable = false;
            promptCanvasGroup.blocksRaycasts = false;

            StartFade(1f);
        }
    }

    // ==========================================
    // SHOW TEXT ONLY - LOCKED
    // ==========================================

    private void ShowLockedMessage()
    {
        // The E icon must never appear while locked.
        HidePromptImmediately();

        if (lockedMessageText == null)
        {
            Debug.LogWarning(
                "Locked Message Text is not assigned on " +
                gameObject.name
            );

            return;
        }

        lockedMessageText.text = lockedMessage;
        lockedMessageText.gameObject.SetActive(true);
    }

    // ==========================================
    // HIDE LOCKED MESSAGE
    // ==========================================

    private void HideLockedMessage()
    {
        if (lockedMessageText != null)
        {
            lockedMessageText.gameObject.SetActive(false);
        }
    }

    // ==========================================
    // OPEN WORD PUZZLE
    // ==========================================

    public void OpenWordPuzzle()
    {
        if (isOpen || IsTableCompleted())
            return;

        if (!IsTableUnlocked())
        {
            Debug.Log(
                "TABLE 3 LOCKED: Complete Table 2 first."
            );

            return;
        }

        if (wordScrabblePanel == null)
        {
            Debug.LogWarning(
                "WordPuzzleInteraction: " +
                "Word Scrabble Panel is missing!"
            );

            return;
        }

        isOpen = true;
        isNear = false;
        hasPromptState = false;

        HideAllPrompts();

        // Hide the Room 2 instruction.
        Room2InstructionManager instructionManager =
            FindFirstObjectByType<Room2InstructionManager>();

        if (instructionManager != null)
        {
            instructionManager.HideInstruction();
        }

        // Remember and disable player movement.
        if (playerMovement != null)
        {
            movementWasEnabled = playerMovement.enabled;
            movementStateSaved = true;
            playerMovement.enabled = false;
        }

        // Freeze camera look while solving.
        if (playerController != null)
        {
            playerController.DisableLook();
        }

        // Open Word Scrabble.
        wordScrabblePanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log(
            "TABLE 3 OPENED: Player movement and camera frozen."
        );
    }

    // ==========================================
    // CLOSE WORD PUZZLE
    // ==========================================

    public void CloseWordPuzzle()
    {
        if (!isOpen)
            return;

        isOpen = false;

        if (wordScrabblePanel != null)
        {
            wordScrabblePanel.SetActive(false);
        }

        // Restore previous movement state.
        if (playerMovement != null && movementStateSaved)
        {
            playerMovement.enabled = movementWasEnabled;
        }

        movementStateSaved = false;

        // Restore camera look.
        if (playerController != null)
        {
            playerController.EnableLook();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // The next Update will determine which prompt
        // should be shown based on distance and progress.
        isNear = false;
        hasPromptState = false;

        Debug.Log(
            "TABLE 3 CLOSED: Player controls restored."
        );
    }

    // ==========================================
    // SHOW / HIDE PROMPT
    // ==========================================

    private void HideAllPrompts()
    {
        HidePromptImmediately();
        HideLockedMessage();
    }

    private void HidePromptImmediately()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        if (promptCanvasGroup != null)
        {
            promptCanvasGroup.alpha = 0f;
            promptCanvasGroup.interactable = false;
            promptCanvasGroup.blocksRaycasts = false;
        }

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    private void StartFade(float targetAlpha)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        if (promptCanvasGroup == null)
        {
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(targetAlpha > 0f);
            }

            return;
        }

        fadeCoroutine = StartCoroutine(
            FadePrompt(targetAlpha)
        );
    }

    private IEnumerator FadePrompt(float targetAlpha)
    {
        if (promptCanvasGroup == null)
        {
            fadeCoroutine = null;
            yield break;
        }

        float startAlpha = promptCanvasGroup.alpha;
        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, fadeDuration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            promptCanvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                t
            );

            yield return null;
        }

        promptCanvasGroup.alpha = targetAlpha;

        if (targetAlpha <= 0f)
        {
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
        }
        else
        {
            bool canRemainVisible =
                isNear &&
                !isOpen &&
                !IsTableCompleted() &&
                IsTableUnlocked();

            if (canRemainVisible)
            {
                promptCanvasGroup.interactable = false;
                promptCanvasGroup.blocksRaycasts = false;
            }
            else
            {
                HidePromptImmediately();
            }
        }

        fadeCoroutine = null;
    }

    // ==========================================
    // RESET AFTER ROOM 2 TRY AGAIN
    // ==========================================

    public void ResetInteractionForRoomRestart()
    {
        isOpen = false;
        isNear = false;
        hasPromptState = false;
        movementStateSaved = false;

        if (wordScrabblePanel != null)
        {
            wordScrabblePanel.SetActive(false);
        }

        HideAllPrompts();

        Debug.Log(
            "TABLE 3 INTERACTION RESET - " +
            "E PROMPT CAN APPEAR AGAIN."
        );
    }
}
