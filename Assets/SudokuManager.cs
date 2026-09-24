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

    // =========================================================
    // 6 x 6 SUDOKU PUZZLE
    // 0 = EMPTY CELL
    // Number = GIVEN NUMBER
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

    // =========================================================
    // COMPLETE SOLUTION
    // =========================================================

    private int[,] solution =
    {
        { 5, 3, 2, 8, 6, 7 },
        { 9, 7, 8, 3, 1, 4 },
        { 1, 4, 6, 2, 9, 5 },

        { 8, 5, 3, 1, 2, 9 },
        { 4, 9, 7, 5, 8, 6 },
        { 6, 2, 1, 7, 4, 3 }
    };

    private bool puzzleCompleted = false;

    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        // Hide completion banner at the beginning
        if (completionBanner != null)
        {
            completionBanner.SetActive(false);
        }

        GenerateBoard();
    }

    // =========================================================
    // GENERATE 6 x 6 BOARD
    // =========================================================

    void GenerateBoard()
    {
        // Remove old cells
        foreach (Transform child in sudokuGrid)
        {
            Destroy(child.gameObject);
        }

        // Create 36 cells
        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 6; col++)
            {
                GameObject cellObject = Instantiate(
                    sudokuCellPrefab,
                    sudokuGrid
                );

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
                        "SudokuCell script is missing on SudokuCell prefab!"
                    );
                }
            }
        }
    }

    // =========================================================
    // CHECK IF WHOLE SUDOKU IS COMPLETE
    // =========================================================

    public void CheckSudokuComplete()
    {
        // Prevent this from running again
        if (puzzleCompleted)
            return;

        SudokuCell[] cells =
            sudokuGrid.GetComponentsInChildren<SudokuCell>();

        // Check every cell
        foreach (SudokuCell cell in cells)
        {
            if (!cell.IsCorrect())
            {
                return;
            }
        }

        // =====================================================
        // EVERYTHING IS CORRECT
        // =====================================================

        puzzleCompleted = true;

        ShowCompletion();
    }

    // =========================================================
    // SUDOKU COMPLETED
    // =========================================================

    private void ShowCompletion()
    {
        Debug.Log("TABLE 1 CHALLENGE COMPLETED!");

        // -----------------------------------------------------
        // CLOSE SUDOKU PANEL
        // -----------------------------------------------------

        if (sudokuPanel != null)
        {
            sudokuPanel.SetActive(false);
        }

        // -----------------------------------------------------
        // SHOW COMPLETION BANNER
        // -----------------------------------------------------

        if (completionBanner != null)
        {
            completionBanner.SetActive(true);
        }

        // -----------------------------------------------------
        // PLAY COMPLETION SOUND
        // -----------------------------------------------------

        if (completionAudio != null)
        {
            completionAudio.Play();
        }

        // -----------------------------------------------------
        // UNLOCK CURSOR
        // -----------------------------------------------------

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // -----------------------------------------------------
        // HIDE BANNER AFTER 2 SECONDS
        // -----------------------------------------------------

        StartCoroutine(HideCompletionBanner());
    }

    // =========================================================
    // HIDE COMPLETION BANNER
    // =========================================================

    private IEnumerator HideCompletionBanner()
    {
        yield return new WaitForSeconds(2f);

        if (completionBanner != null)
        {
            completionBanner.SetActive(false);
        }
    }
}