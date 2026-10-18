
using System.Collections;
using UnityEngine;

public class SudokuManager : MonoBehaviour
{
    [Header("Sudoku Setup")]
    public Transform sudokuGrid;
    public GameObject sudokuCellPrefab;

    [Header("Sudoku Panel")]
    public GameObject sudokuPanel;

    [Header("Completion")]
    public GameObject completionBanner;

    [Header("Completion Audio")]
    public AudioSource completionAudio;

    [Header("Wrong Answer Audio")]
    public AudioSource wrongAnswerAudio;
    public AudioClip wrongAnswerSound;

    [Header("Sudoku Controller")]
    public SudokuPanelController sudokuPanelController;

    [Header("Room 2 Instruction")]
    public Room2InstructionManager instructionManager;

    // =========================================================
    // PUZZLE
    // =========================================================

    private int[,] puzzle =
    {
        { 5, 0, 2, 0, 6, 0 },
        { 0, 7, 0, 3, 0, 4 },
        { 1, 0, 6, 0, 9, 0 },

        { 0, 5, 0, 1, 0, 9 },
        { 4, 0, 7, 0, 8, 0 },
        { 0, 2, 0, 7, 0, 3 }
    };

    private int[,] solution =
    {
        { 5, 3, 2, 8, 6, 7 },
        { 9, 7, 8, 3, 1, 4 },
        { 1, 4, 6, 2, 9, 5 },

        { 8, 5, 3, 1, 2, 9 },
        { 4, 9, 7, 5, 8, 6 },
        { 6, 2, 1, 7, 4, 3 }
    };

    // =========================================================
    // STATE
    // =========================================================

    private bool puzzleCompleted = false;

    public bool IsPuzzleCompleted()
    {
        return puzzleCompleted;
    }

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        puzzleCompleted = false;

        if (completionBanner != null)
        {
            completionBanner.SetActive(false);
        }

        GenerateBoard();
    }

    // =========================================================
    // GENERATE BOARD
    // =========================================================

    private void GenerateBoard()
    {
        if (sudokuGrid == null)
        {
            Debug.LogError("Sudoku Grid is NOT assigned!", this);
            return;
        }

        if (sudokuCellPrefab == null)
        {
            Debug.LogError("Sudoku Cell Prefab is NOT assigned!", this);
            return;
        }

        // Remove previously generated cells.
        foreach (Transform child in sudokuGrid)
        {
            Destroy(child.gameObject);
        }

        // Generate a fresh 6x6 board.
        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 6; col++)
            {
                GameObject cellObject =
                    Instantiate(sudokuCellPrefab, sudokuGrid);

                cellObject.name = "Cell_" + (row * 6 + col);

                SudokuCell cell =
                    cellObject.GetComponent<SudokuCell>();

                if (cell != null)
                {
                    bool editable = puzzle[row, col] == 0;

                    cell.Setup(
                        puzzle[row, col],
                        solution[row, col],
                        editable,
                        this
                    );
                }
                else
                {
                    Debug.LogError(
                        "SudokuCell component NOT FOUND on " +
                        cellObject.name,
                        cellObject
                    );
                }
            }
        }
    }

    // =========================================================
    // CHECK SUDOKU COMPLETE
    // =========================================================

    public void CheckSudokuComplete()
    {
        if (puzzleCompleted)
            return;

        if (sudokuGrid == null)
            return;

        SudokuCell[] cells =
            sudokuGrid.GetComponentsInChildren<SudokuCell>();

        foreach (SudokuCell cell in cells)
        {
            if (!cell.IsCorrect())
            {
                return;
            }
        }

        puzzleCompleted = true;
        ShowCompletion();
    }

    // =========================================================
    // WRONG ANSWER SOUND
    // =========================================================

    public void PlayWrongAnswerSound()
    {
        if (wrongAnswerAudio != null && wrongAnswerSound != null)
        {
            wrongAnswerAudio.PlayOneShot(wrongAnswerSound);
        }
    }

    // =========================================================
    // SHOW COMPLETION
    // =========================================================

    private void ShowCompletion()
    {
        Debug.Log("TABLE 1 CHALLENGE COMPLETED!");

        // Close Sudoku.
        if (sudokuPanelController != null)
        {
            sudokuPanelController.CloseSudoku();
        }
        else if (sudokuPanel != null)
        {
            sudokuPanel.SetActive(false);
        }

        // Show completion banner.
        if (completionBanner != null)
        {
            completionBanner.SetActive(true);
        }

        // Play completion sound.
        if (completionAudio != null)
        {
            completionAudio.Play();
        }

        // Unlock Table 2.
        if (ChallengeProgressManager.Instance != null)
        {
            ChallengeProgressManager.Instance.CompleteTable(1);
        }

        // Hide the banner and restore the top instruction afterward.
        StartCoroutine(HideCompletionBanner());
    }

    // =========================================================
    // HIDE COMPLETION BANNER AND RESTORE TOP INSTRUCTION
    // =========================================================

    private IEnumerator HideCompletionBanner()
    {
        yield return new WaitForSeconds(2f);

        if (completionBanner != null)
        {
            completionBanner.SetActive(false);
        }

        if (instructionManager != null)
        {
            // Updates the persistent top text only.
            // It must not reopen the large instruction panel.
            instructionManager.ShowChallengeCleared();
        }
        else
        {
            Debug.LogWarning(
                "SudokuManager: Room2InstructionManager is not assigned!",
                this
            );
        }
    }

    // =========================================================
    // RESET PUZZLE
    // Called by Room2GameOverController.TryAgain()
    // =========================================================

    public void ResetPuzzle()
    {
        Debug.Log("=================================");
        Debug.Log("RESETTING SUDOKU");
        Debug.Log("=================================");

        // Stop the old completion coroutine.
        StopAllCoroutines();

        // Reset completion state.
        puzzleCompleted = false;

        // Hide completion banner.
        if (completionBanner != null)
        {
            completionBanner.SetActive(false);
        }

        // Stop completion audio.
        if (completionAudio != null)
        {
            completionAudio.Stop();
        }

        // Close Sudoku panel.
        if (sudokuPanel != null)
        {
            sudokuPanel.SetActive(false);
        }

        // Restore player control through the existing controller.
        if (sudokuPanelController != null)
        {
            sudokuPanelController.ForceCloseSudoku();
        }

        // Regenerate the Sudoku board.
        GenerateBoard();

        Debug.Log("SUDOKU RESET COMPLETE!");
    }
}
