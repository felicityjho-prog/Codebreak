using UnityEngine;
using System.Collections;

public class TableInteraction : MonoBehaviour
{
    [Header("Interaction")]
    public float interactionDistance = 3f;

    [Header("Table Progress")]
    public int tableNumber = 1;

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

        HidePrompt();
    }

    // ==========================================
    // UPDATE
    // ==========================================

    void Update()
    {
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
        // SUDOKU IS OPEN
        // ==========================================

        if (IsSudokuOpen())
        {
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
        if (IsTableCompleted())
            return;

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

        if (targetAlpha > 0f &&
            !IsSudokuOpen() &&
            !IsTableCompleted())
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
        if (IsTableCompleted())
            return;

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
        // OPEN SUDOKU
        // ==========================================

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