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
            Debug.LogError(
                "Sudoku Grid is NOT assigned!"
            );

            return;
        }

        if (sudokuCellPrefab == null)
        {
            Debug.LogError(
                "Sudoku Cell Prefab is NOT assigned!"
            );

            return;
        }

        // =====================================================
        // DESTROY OLD CELLS
        // =====================================================

        foreach (Transform child in sudokuGrid)
        {
            Destroy(child.gameObject);
        }

        // =====================================================
        // GENERATE FRESH 6x6 BOARD
        // =====================================================

        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 6; col++)
            {
                GameObject cellObject =
                    Instantiate(
                        sudokuCellPrefab,
                        sudokuGrid
                    );

                cellObject.name =
                    "Cell_" + (row * 6 + col);

                SudokuCell cell =
                    cellObject.GetComponent<SudokuCell>();

                if (cell != null)
                {
                    bool editable =
                        puzzle[row, col] == 0;

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
                        cellObject.name
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
        // Already completed
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

        // =====================================================
        // ALL CORRECT
        // =====================================================

        puzzleCompleted = true;

        ShowCompletion();
    }

    // =========================================================
    // SHOW COMPLETION
    // =========================================================

    private void ShowCompletion()
    {
        Debug.Log(
            "TABLE 1 CHALLENGE COMPLETED!"
        );

        // =====================================================
        // CLOSE SUDOKU
        // =====================================================

        if (sudokuPanelController != null)
        {
            sudokuPanelController.CloseSudoku();
        }
        else
        {
            if (sudokuPanel != null)
            {
                sudokuPanel.SetActive(false);
            }
        }

        // =====================================================
        // SHOW COMPLETION BANNER
        // =====================================================

        if (completionBanner != null)
        {
            completionBanner.SetActive(true);
        }

        // =====================================================
        // PLAY COMPLETION SOUND
        // =====================================================

        if (completionAudio != null)
        {
            completionAudio.Play();
        }

        // =====================================================
        // UNLOCK TABLE 2
        // =====================================================

        if (ChallengeProgressManager.Instance != null)
        {
            ChallengeProgressManager.Instance.CompleteTable(1);
        }

        // =====================================================
        // WAIT THEN SHOW INSTRUCTION
        // =====================================================

        StartCoroutine(
            HideCompletionBanner()
        );
    }

    // =========================================================
    // HIDE COMPLETION BANNER
    // =========================================================

    private IEnumerator HideCompletionBanner()
    {
        yield return new WaitForSeconds(2f);

        // =====================================================
        // HIDE COMPLETION BANNER
        // =====================================================

        if (completionBanner != null)
        {
            completionBanner.SetActive(false);
        }

        // =====================================================
        // SHOW NEXT INSTRUCTION
        // =====================================================

        if (instructionManager != null)
        {
            instructionManager.ShowChallengeCleared();
        }
    }

    // =========================================================
    // RESET PUZZLE
    // =========================================================
    //
    // Called by:
    // Room2GameOverController.TryAgain()
    //
    // PURPOSE:
    // - Remove all player-entered numbers
    // - Restore original Sudoku
    // - Keep given numbers
    // - Remove red/green feedback
    // - Reset completion state
    // - Stop old completion coroutine
    // =========================================================

    public void ResetPuzzle()
    {
        Debug.Log(
            "================================="
        );

        Debug.Log(
            "RESETTING SUDOKU"
        );

        Debug.Log(
            "================================="
        );

        // =====================================================
        // STOP OLD COROUTINES
        // =====================================================
        //
        // Important:
        // If the Sudoku was completed before Game Over,
        // HideCompletionBanner() could still be waiting.
        //
        // We don't want the old coroutine to execute after
        // Try Again and show the completion instruction again.
        // =====================================================

        StopAllCoroutines();

        // =====================================================
        // RESET COMPLETION STATE
        // =====================================================

        puzzleCompleted = false;

        // =====================================================
        // HIDE COMPLETION BANNER
        // =====================================================

        if (completionBanner != null)
        {
            completionBanner.SetActive(false);
        }

        // =====================================================
        // RESET COMPLETION AUDIO
        // =====================================================

        if (completionAudio != null)
        {
            completionAudio.Stop();
        }

        // =====================================================
        // CLOSE SUDOKU PANEL
        // =====================================================

        if (sudokuPanel != null)
        {
            sudokuPanel.SetActive(false);
        }

        // =====================================================
        // FORCE RESET PLAYER CONTROL
        // =====================================================
        //
        // This makes sure Sudoku cannot leave the player
        // frozen after Try Again.
        // =====================================================

        if (sudokuPanelController != null)
        {
            sudokuPanelController.ForceCloseSudoku();
        }

        // =====================================================
        // REGENERATE ENTIRE BOARD
        // =====================================================
        //
        // This destroys the old cells containing the player's
        // previous answers and creates completely new cells.
        //
        // Given numbers come from "puzzle".
        // Player answers start EMPTY.
        // =====================================================

        GenerateBoard();

        Debug.Log(
            "SUDOKU RESET COMPLETE!"
        );
    }
}