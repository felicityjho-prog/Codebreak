
using UnityEngine;
using System.Collections;
using TMPro;

public class CrosswordTableInteraction : MonoBehaviour
{
    [Header("Interaction")]
    public float interactionDistance = 3f;

    [Header("Table Progress")]
    public int tableNumber = 2;

    [Header("Unlocked UI - E Icon")]
    public GameObject interactPrompt;
    public CanvasGroup promptCanvasGroup;

    [Header("Locked UI - Text Only")]
    public TMP_Text lockedMessageText;

    [TextArea(2, 3)]
    public string lockedMessage =
        "PREVIOUS CHALLENGE REQUIRED\nCOMPLETE TABLE 1 FIRST.";

    [Header("Fade Settings")]
    public float fadeDuration = 0.25f;

    [Header("Crossword")]
    public CrosswordPanelController crosswordController;

    private Transform player;
    private bool isNear = false;
    private bool hasPromptState = false;
    private bool lastKnownUnlocked = false;

    private Coroutine fadeCoroutine;
    private bool warnedMissingManager = false;

    // ==========================================
    // START
    // ==========================================

    void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        HideAllPrompts();
    }

    // ==========================================
    // UPDATE
    // ==========================================

    void Update()
    {
        if (player == null)
            return;

        // TABLE ALREADY COMPLETED
        if (IsTableCompleted())
        {
            isNear = false;
            hasPromptState = false;
            HideAllPrompts();
            return;
        }

        // CROSSWORD IS OPEN
        if (IsCrosswordOpen())
        {
            isNear = false;
            hasPromptState = false;
            HideAllPrompts();
            return;
        }

        float distance = Vector3.Distance(
            player.position,
            transform.position
        );

        // PLAYER FAR FROM TABLE
        if (distance > interactionDistance)
        {
            isNear = false;
            hasPromptState = false;
            HideAllPrompts();
            return;
        }

        // PLAYER IS NEAR THE TABLE
        bool unlocked = IsTableUnlocked();

        // Update the UI when entering range
        // or when the table's lock state changes.
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

        // PRESS E
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (unlocked)
            {
                OpenCrossword();
            }
            else
            {
                Debug.Log(
                    "Table " + tableNumber +
                    " is locked. Complete the previous challenge first."
                );
            }
        }
    }

    // ==========================================
    // CHECK IF TABLE IS UNLOCKED
    // ==========================================

    bool IsTableUnlocked()
    {
        ChallengeProgressManager manager =
            ChallengeProgressManager.Instance;

        if (manager == null)
        {
            if (!warnedMissingManager)
            {
                Debug.LogWarning(
                    "ChallengeProgressManager is missing!"
                );

                warnedMissingManager = true;
            }

            return false;
        }

        warnedMissingManager = false;

        return manager.IsTableUnlocked(tableNumber);
    }

    // ==========================================
    // CHECK IF TABLE IS COMPLETED
    // ==========================================

    bool IsTableCompleted()
    {
        if (ChallengeProgressManager.Instance == null)
            return false;

        return ChallengeProgressManager.Instance
            .IsTableCompleted(tableNumber);
    }

    // ==========================================
    // CHECK IF CROSSWORD IS OPEN
    // ==========================================

    bool IsCrosswordOpen()
    {
        return crosswordController != null &&
               crosswordController.IsCrosswordOpen;
    }

    // ==========================================
    // SHOW E ICON - UNLOCKED TABLE
    // ==========================================

    void ShowUnlockedPrompt()
    {
        HideLockedMessage();

        if (IsTableCompleted() ||
            !IsTableUnlocked() ||
            IsCrosswordOpen())
        {
            HidePrompt();
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
    // SHOW WARNING TEXT ONLY - LOCKED TABLE
    // ==========================================

    void ShowLockedMessage()
    {
        // Never show the E icon while locked.
        HidePrompt();

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
    // HIDE LOCKED WARNING
    // ==========================================

    void HideLockedMessage()
    {
        if (lockedMessageText != null)
        {
            lockedMessageText.gameObject.SetActive(false);
        }
    }

    // ==========================================
    // HIDE E PROMPT
    // ==========================================

    void HidePrompt()
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

    // ==========================================
    // HIDE BOTH UI ELEMENTS
    // ==========================================

    void HideAllPrompts()
    {
        HidePrompt();
        HideLockedMessage();
    }

    // ==========================================
    // FADE
    // ==========================================

    void StartFade(float targetAlpha)
    {
        if (promptCanvasGroup == null)
            return;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(
            FadePrompt(targetAlpha)
        );
    }

    IEnumerator FadePrompt(float targetAlpha)
    {
        if (promptCanvasGroup == null)
        {
            fadeCoroutine = null;
            yield break;
        }

        float startAlpha = promptCanvasGroup.alpha;
        float time = 0f;

        if (fadeDuration <= 0f)
        {
            promptCanvasGroup.alpha = targetAlpha;
        }
        else
        {
            while (time < fadeDuration)
            {
                time += Time.deltaTime;

                float t = Mathf.Clamp01(
                    time / fadeDuration
                );

                promptCanvasGroup.alpha = Mathf.Lerp(
                    startAlpha,
                    targetAlpha,
                    t
                );

                yield return null;
            }

            promptCanvasGroup.alpha = targetAlpha;
        }

        fadeCoroutine = null;

        bool canRemainVisible =
            targetAlpha > 0f &&
            isNear &&
            !IsTableCompleted() &&
            IsTableUnlocked() &&
            !IsCrosswordOpen();

        if (canRemainVisible)
        {
            promptCanvasGroup.interactable = true;
            promptCanvasGroup.blocksRaycasts = true;

            if (interactPrompt != null)
                interactPrompt.SetActive(true);
        }
        else
        {
            HidePrompt();
        }
    }

    // ==========================================
    // OPEN CROSSWORD
    // ==========================================

    void OpenCrossword()
    {
        // Prevent opening a locked or completed table.
        if (IsTableCompleted() || !IsTableUnlocked())
            return;

        if (crosswordController == null)
        {
            Debug.LogError(
                "CrosswordTableInteraction: " +
                "CrosswordPanelController is NOT assigned!"
            );

            return;
        }

        Debug.Log("OPENING CROSSWORD...");

        HideAllPrompts();

        isNear = false;
        hasPromptState = false;

        // HIDE ROOM INSTRUCTION
        Room2InstructionManager instructionManager =
            FindFirstObjectByType<Room2InstructionManager>();

        if (instructionManager != null)
        {
            instructionManager.HideInstruction();
        }

        // OPEN CROSSWORD
        crosswordController.OpenCrossword();
    }
}
