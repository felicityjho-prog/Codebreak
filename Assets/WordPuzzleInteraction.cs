
using UnityEngine;
using System.Collections;

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

    [Header("Interaction Prompt")]
    public GameObject interactPrompt;
    public CanvasGroup promptCanvasGroup;
    public float fadeDuration = 0.25f;

    private Transform player;
    private bool isNear;
    private bool isOpen;

    private bool movementWasEnabled;
    private bool movementStateSaved;

    private Coroutine fadeCoroutine;

    private void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;

            if (playerMovement == null)
                playerMovement =
                    playerObject.GetComponent<PlayerMovement>();

            if (playerController == null)
                playerController =
                    playerObject.GetComponent<PlayerController>();
        }

        if (wordScrabblePanel != null)
            wordScrabblePanel.SetActive(false);

        HidePromptImmediately();
    }

    private void Update()
    {
        if (IsTableCompleted())
        {
            isNear = false;
            HidePromptImmediately();
            return;
        }

        if (isOpen || player == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (distance <= interactionDistance)
        {
            if (!isNear)
            {
                isNear = true;
                ShowPrompt();
            }

            if (Input.GetKeyDown(interactionKey))
                OpenWordPuzzle();
        }
        else
        {
            if (isNear)
            {
                isNear = false;
                HidePrompt();
            }
        }
    }

    private bool IsTableCompleted()
    {
        if (ChallengeProgressManager.Instance == null)
            return false;

        return ChallengeProgressManager.Instance
            .IsTableCompleted(tableNumber);
    }

    // ==========================================
    // OPEN WORD PUZZLE
    // ==========================================

    public void OpenWordPuzzle()
    {
        if (isOpen || IsTableCompleted())
            return;

        if (wordScrabblePanel == null)
        {
            Debug.LogWarning(
                "WordPuzzleInteraction: Word Scrabble Panel is missing!"
            );
            return;
        }

        isOpen = true;
        isNear = false;

        HidePromptImmediately();

        // Hide the Room 2 instruction when opening the puzzle.
        Room2InstructionManager instructionManager =
            FindFirstObjectByType<Room2InstructionManager>();

        if (instructionManager != null)
            instructionManager.HideInstruction();

        // Remember the previous movement state.
        if (playerMovement != null)
        {
            movementWasEnabled = playerMovement.enabled;
            movementStateSaved = true;

            // Freeze WASD movement.
            playerMovement.enabled = false;
        }

        // Freeze camera look.
        if (playerController != null)
        {
            playerController.DisableLook();
        }

        // Open Word Scrabble.
        wordScrabblePanel.SetActive(true);

        // Allow clicking the puzzle UI.
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
            wordScrabblePanel.SetActive(false);

        // Restore previous movement state.
        if (playerMovement != null && movementStateSaved)
        {
            playerMovement.enabled = movementWasEnabled;
        }

        movementStateSaved = false;

        // Restore camera look.
        if (playerController != null)
            playerController.EnableLook();

        // Return to FPS gameplay controls.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log(
            "TABLE 3 CLOSED: Player controls restored."
        );
    }

    // ==========================================
    // PROMPT VISIBILITY
    // ==========================================

    private void ShowPrompt()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        if (interactPrompt != null)
            interactPrompt.SetActive(true);

        if (promptCanvasGroup != null)
        {
            promptCanvasGroup.interactable = false;
            promptCanvasGroup.blocksRaycasts = false;

            fadeCoroutine = StartCoroutine(FadePrompt(1f));
        }
    }

    private void HidePrompt()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        if (promptCanvasGroup != null)
        {
            promptCanvasGroup.interactable = false;
            promptCanvasGroup.blocksRaycasts = false;

            fadeCoroutine = StartCoroutine(FadePrompt(0f));
        }
        else if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
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
            interactPrompt.SetActive(false);
    }

    private IEnumerator FadePrompt(float targetAlpha)
    {
        if (promptCanvasGroup == null)
        {
            if (interactPrompt != null)
                interactPrompt.SetActive(targetAlpha > 0f);

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

        if (targetAlpha <= 0f && interactPrompt != null)
            interactPrompt.SetActive(false);

        promptCanvasGroup.interactable = false;
        promptCanvasGroup.blocksRaycasts = false;

        fadeCoroutine = null;
    }

    // ==========================================
    // RESET AFTER ROOM 2 TRY AGAIN
    // ==========================================

    public void ResetInteractionForRoomRestart()
    {
        // Reset the interaction state.
        isOpen = false;
        isNear = false;

        // Close the Word Scrabble panel.
        if (wordScrabblePanel != null)
        {
            wordScrabblePanel.SetActive(false);
        }

        // Reset the interaction prompt.
        HidePromptImmediately();

        Debug.Log(
            "TABLE 3 INTERACTION RESET - E PROMPT CAN APPEAR AGAIN."
        );
    }
}
