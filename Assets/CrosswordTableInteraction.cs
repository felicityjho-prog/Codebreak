using UnityEngine;
using System.Collections;

public class CrosswordTableInteraction : MonoBehaviour
{
    [Header("Interaction")]
    public float interactionDistance = 3f;

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

    // ==========================================
    // START
    // ==========================================

    void Start()
    {
        // Find Player
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        // Hide prompt at start
        HidePrompt();
    }

    // ==========================================
    // UPDATE
    // ==========================================

    void Update()
    {
        // No player
        if (player == null)
            return;

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

        // ==========================================
        // CHECK DISTANCE
        // ==========================================

        float distance = Vector3.Distance(
            player.position,
            transform.position
        );

        // ==========================================
        // PLAYER NEAR CROSSWORD TABLE
        // ==========================================

        if (distance <= interactionDistance)
        {
            if (!isNear)
            {
                isNear = true;
                ShowPrompt();
            }

            // Press E
            if (Input.GetKeyDown(KeyCode.E))
            {
                OpenCrossword();
            }
        }

        // ==========================================
        // PLAYER FAR FROM CROSSWORD TABLE
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
    // OPEN CROSSWORD
    // ==========================================

    void OpenCrossword()
    {
        // Check controller
        if (crosswordController == null)
        {
            Debug.LogError(
                "CrosswordTableInteraction: CrosswordPanelController is NOT assigned!"
            );

            return;
        }

        Debug.Log("OPENING CROSSWORD...");

        // Hide interaction prompt
        HidePrompt();

        isNear = false;

        // Open crossword through controller
        crosswordController.OpenCrossword();
    }

    // ==========================================
    // SHOW PROMPT
    // ==========================================

    void ShowPrompt()
    {
        if (interactPrompt == null)
            return;

        if (promptCanvasGroup == null)
        {
            interactPrompt.SetActive(true);
            return;
        }

        // Make object active first
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

        // ==========================================
        // ENABLE / DISABLE INTERACTION
        // ==========================================

        if (targetAlpha > 0f)
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