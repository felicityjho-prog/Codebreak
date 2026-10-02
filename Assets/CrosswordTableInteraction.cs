using UnityEngine;
using System.Collections;
using TMPro;

public class CrosswordTableInteraction : MonoBehaviour
{
    [Header("Interaction")]
    public float interactionDistance = 3f;

    [Header("Table Progress")]
    public int tableNumber = 2;

    [Header("UI")]
    public GameObject interactPrompt;
    public CanvasGroup promptCanvasGroup;

    [Header("Fade Settings")]
    public float fadeDuration = 0.25f;

    [Header("Crossword")]
    public CrosswordPanelController crosswordController;

    private Transform player;
    private bool isNear = false;
    private Coroutine fadeCoroutine;

    private TMP_Text promptText;

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

        if (interactPrompt != null)
        {
            promptText =
                interactPrompt.GetComponentInChildren<TMP_Text>(true);
        }

        HidePrompt();
    }

    // ==========================================
    // UPDATE
    // ==========================================

    void Update()
    {
        if (player == null)
            return;

        // ==========================================
        // TABLE ALREADY COMPLETED
        // ==========================================

        if (IsTableCompleted())
        {
            isNear = false;
            HidePrompt();
            return;
        }

        // ==========================================
        // CROSSWORD IS OPEN
        // ==========================================

        if (crosswordController != null &&
            crosswordController.IsCrosswordOpen)
        {
            isNear = false;
            HidePrompt();
            return;
        }

        float distance =
            Vector3.Distance(
                player.position,
                transform.position
            );

        // ==========================================
        // PLAYER NEAR TABLE
        // ==========================================

        if (distance <= interactionDistance)
        {
            if (!isNear)
            {
                isNear = true;

                if (IsTableUnlocked())
                {
                    SetPromptText(
                        "[E] TO INTERACT"
                    );
                }
                else
                {
                    SetPromptText(
                        "PREVIOUS CHALLENGE REQUIRED — COMPLETE TABLE 1 FIRST."
                    );
                }

                ShowPrompt();
            }

            // ==========================================
            // PRESS E
            // ==========================================

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (IsTableUnlocked())
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
        // PLAYER FAR
        // ==========================================

        else
        {
            if (isNear)
            {
                isNear = false;
                HidePrompt();
            }
        }
    }

    // ==========================================
    // CHECK IF TABLE IS UNLOCKED
    // ==========================================

    bool IsTableUnlocked()
    {
        if (ChallengeProgressManager.Instance == null)
        {
            Debug.LogWarning(
                "ChallengeProgressManager is missing!"
            );

            return false;
        }

        return ChallengeProgressManager.Instance
            .IsTableUnlocked(tableNumber);
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
    // OPEN CROSSWORD
    // ==========================================

    void OpenCrossword()
    {
        if (IsTableCompleted())
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

        // Hide E prompt
        HidePrompt();

        isNear = false;

        // ==========================================
        // HIDE ROOM INSTRUCTION
        // ==========================================

        Room2InstructionManager instructionManager =
            FindFirstObjectByType<Room2InstructionManager>();

        if (instructionManager != null)
        {
            instructionManager.HideInstruction();
        }

        // ==========================================
        // OPEN CROSSWORD
        // ==========================================

        crosswordController.OpenCrossword();
    }

    // ==========================================
    // SET PROMPT TEXT
    // ==========================================

    void SetPromptText(string message)
    {
        if (promptText != null)
        {
            promptText.text = message;
        }
    }

    // ==========================================
    // SHOW PROMPT
    // ==========================================

    void ShowPrompt()
    {
        if (IsTableCompleted())
            return;

        if (interactPrompt == null)
            return;

        if (promptCanvasGroup == null)
        {
            interactPrompt.SetActive(true);
            return;
        }

        interactPrompt.SetActive(true);

        StartFade(1f);
    }

    // ==========================================
    // HIDE PROMPT
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

        fadeCoroutine =
            StartCoroutine(
                FadePrompt(targetAlpha)
            );
    }

    // ==========================================
    // FADE PROMPT
    // ==========================================

    IEnumerator FadePrompt(float targetAlpha)
    {
        if (promptCanvasGroup == null)
            yield break;

        float startAlpha =
            promptCanvasGroup.alpha;

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            float t =
                time / fadeDuration;

            promptCanvasGroup.alpha =
                Mathf.Lerp(
                    startAlpha,
                    targetAlpha,
                    t
                );

            yield return null;
        }

        promptCanvasGroup.alpha =
            targetAlpha;

        if (targetAlpha > 0f &&
            !IsTableCompleted())
        {
            promptCanvasGroup.interactable = true;
            promptCanvasGroup.blocksRaycasts = true;
        }
        else
        {
            promptCanvasGroup.interactable = false;
            promptCanvasGroup.blocksRaycasts = false;

            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
        }
    }
}