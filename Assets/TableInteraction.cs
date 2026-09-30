using UnityEngine;
using System.Collections;

public class TableInteraction : MonoBehaviour
{
    [Header("Interaction")]
    public float interactionDistance = 3f;

    [Header("UI")]
    public GameObject interactPrompt;
    public CanvasGroup promptCanvasGroup;

    [Header("Fade Settings")]
    public float fadeDuration = 0.25f;

    [Header("Puzzle")]
    public GameObject puzzlePanel;

    [Header("Sudoku Controller")]
    public SudokuPanelController sudokuPanelController;

    private Transform player;
    private bool isNear = false;
    private Coroutine fadeCoroutine;

    void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        HidePrompt();
    }

    void Update()
    {
        // ==========================================
        // SUDOKU IS OPEN
        // ==========================================

        if (IsSudokuOpen())
        {
            // Completely hide E prompt
            HidePrompt();

            isNear = false;

            return;
        }

        // ==========================================
        // NO PLAYER
        // ==========================================

        if (player == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        // ==========================================
        // PLAYER NEAR TABLE
        // ==========================================

        if (distance <= interactionDistance)
        {
            if (!isNear)
            {
                isNear = true;
                FadeIn();
            }

            // Press E
            if (Input.GetKeyDown(KeyCode.E))
            {
                OpenPuzzle();
            }
        }

        // ==========================================
        // PLAYER FAR FROM TABLE
        // ==========================================

        else
        {
            if (isNear)
            {
                isNear = false;
                FadeOut();
            }
        }
    }

    // ==========================================
    // CHECK IF SUDOKU IS OPEN
    // ==========================================

    bool IsSudokuOpen()
    {
        if (sudokuPanelController != null)
        {
            return sudokuPanelController.IsSudokuOpen;
        }

        return false;
    }

    // ==========================================
    // FADE IN
    // ==========================================

    void FadeIn()
    {
        // Don't show while Sudoku is open
        if (IsSudokuOpen())
            return;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(true);
        }

        fadeCoroutine = StartCoroutine(
            FadePrompt(1f)
        );
    }

    // ==========================================
    // FADE OUT
    // ==========================================

    void FadeOut()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(
            FadePrompt(0f)
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

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float t =
                elapsed / fadeDuration;

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

        // Show prompt only if Sudoku is NOT open
        if (targetAlpha > 0f && !IsSudokuOpen())
        {
            promptCanvasGroup.interactable = true;
            promptCanvasGroup.blocksRaycasts = true;

            if (interactPrompt != null)
            {
                interactPrompt.SetActive(true);
            }
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
    // OPEN PUZZLE
    // ==========================================

    void OpenPuzzle()
    {
        // Immediately hide E prompt
        HidePrompt();

        isNear = false;

        if (sudokuPanelController != null)
        {
            sudokuPanelController.OpenSudoku();
        }
        else if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(true);

            Cursor.lockState =
                CursorLockMode.None;

            Cursor.visible = true;
        }
    }
}