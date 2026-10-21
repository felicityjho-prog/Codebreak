
using System.Collections;
using System.Text;
using UnityEngine;

public class SudokuManager : MonoBehaviour
{
    private const int GridSize = 6;

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
    // ORIGINAL PUZZLE
    // These arrays are the source template.
    // They are never modified during randomization.
    // =========================================================

    private readonly int[,] basePuzzle =
    {
        { 0, 0, 0, 0, 5, 2 },
        { 2, 0, 5, 0, 6, 0 },
        { 1, 0, 0, 0, 0, 0 },

        { 4, 6, 2, 3, 0, 5 },
        { 0, 4, 0, 0, 3, 0 },
        { 0, 2, 0, 0, 4, 1 }
    };

    private readonly int[,] baseSolution =
    {
        { 6, 3, 4, 1, 5, 2 },
        { 2, 1, 5, 4, 6, 3 },
        { 1, 5, 3, 6, 2, 4 },

        { 4, 6, 2, 3, 1, 5 },
        { 5, 4, 1, 2, 3, 6 },
        { 3, 2, 6, 5, 4, 1 }
    };

    // Generated puzzle and matching answer key.
    private int[,] puzzle = new int[GridSize, GridSize];
    private int[,] solution = new int[GridSize, GridSize];

    // Prevent the same visible puzzle from being generated twice
    // in a row during the current game session.
    private string lastPuzzleSignature = string.Empty;

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

        // Generate a randomized puzzle on initial play.
        GenerateRandomPuzzle();
        GenerateBoard();
    }

    // =========================================================
    // GENERATE A RANDOM VALID SUDOKU
    // =========================================================

    private void GenerateRandomPuzzle()
    {
        int[,] candidatePuzzle = null;
        int[,] candidateSolution = null;
        string candidateSignature = string.Empty;

        // Retry if the visible puzzle matches the previous one.
        for (int attempt = 0; attempt < 50; attempt++)
        {
            CreateRandomizedArrays(
                out candidatePuzzle,
                out candidateSolution
            );

            candidateSignature =
                GetPuzzleSignature(candidatePuzzle);

            if (candidateSignature != lastPuzzleSignature)
            {
                puzzle = candidatePuzzle;
                solution = candidateSolution;

                lastPuzzleSignature = candidateSignature;

                Debug.Log(
                    "New randomized Sudoku generated."
                );

                return;
            }
        }

        // Fallback: use the last valid generated candidate.
        // Each candidate is still a valid transformation.
        puzzle = candidatePuzzle;
        solution = candidateSolution;

        lastPuzzleSignature = candidateSignature;

        Debug.Log(
            "Sudoku generated using fallback candidate."
        );
    }

    // =========================================================
    // CREATE RANDOMIZED PUZZLE AND SOLUTION
    // =========================================================

    private void CreateRandomizedArrays(
        out int[,] randomizedPuzzle,
        out int[,] randomizedSolution
    )
    {
        // These transformations preserve Sudoku rules.
        int[] rowOrder = CreateRowOrder();
        int[] columnOrder = CreateColumnOrder();
        int[] digitMap = CreateDigitMap();

        randomizedPuzzle =
            new int[GridSize, GridSize];

        randomizedSolution =
            new int[GridSize, GridSize];

        for (int row = 0; row < GridSize; row++)
        {
            for (int col = 0; col < GridSize; col++)
            {
                int originalRow = rowOrder[row];
                int originalCol = columnOrder[col];

                int originalGiven =
                    basePuzzle[originalRow, originalCol];

                int originalAnswer =
                    baseSolution[originalRow, originalCol];

                // Preserve blank cells.
                randomizedPuzzle[row, col] =
                    originalGiven == 0
                    ? 0
                    : digitMap[originalGiven];

                // Update the answer key using the same
                // row, column, and digit transformations.
                randomizedSolution[row, col] =
                    digitMap[originalAnswer];
            }
        }
    }

    // =========================================================
    // RANDOMIZE ROWS
    // A valid 6x6 Sudoku has three bands of two rows.
    // Rows can be reordered within each band, and the
    // three bands can be reordered.
    // =========================================================

    private int[] CreateRowOrder()
    {
        int[] bandOrder = { 0, 1, 2 };

        Shuffle(bandOrder);

        int[] rowOrder = new int[GridSize];
        int index = 0;

        for (int band = 0; band < 3; band++)
        {
            int[] rowsWithinBand = { 0, 1 };

            Shuffle(rowsWithinBand);

            for (int i = 0; i < rowsWithinBand.Length; i++)
            {
                rowOrder[index] =
                    bandOrder[band] * 2 + rowsWithinBand[i];

                index++;
            }
        }

        return rowOrder;
    }

    // =========================================================
    // RANDOMIZE COLUMNS
    // A valid 6x6 Sudoku has two stacks of three columns.
    // Columns can be reordered within each stack, and the
    // two stacks can be swapped.
    // =========================================================

    private int[] CreateColumnOrder()
    {
        int[] stackOrder = { 0, 1 };

        Shuffle(stackOrder);

        int[] columnOrder = new int[GridSize];
        int index = 0;

        for (int stack = 0; stack < 2; stack++)
        {
            int[] columnsWithinStack = { 0, 1, 2 };

            Shuffle(columnsWithinStack);

            for (int i = 0; i < columnsWithinStack.Length; i++)
            {
                columnOrder[index] =
                    stackOrder[stack] * 3
                    + columnsWithinStack[i];

                index++;
            }
        }

        return columnOrder;
    }

    // =========================================================
    // RANDOMIZE DIGITS 1 TO 6
    // Zero remains zero so blank cells stay blank.
    // =========================================================

    private int[] CreateDigitMap()
    {
        int[] digits = { 1, 2, 3, 4, 5, 6 };

        Shuffle(digits);

        int[] digitMap = new int[GridSize + 1];

        digitMap[0] = 0;

        for (int i = 0; i < digits.Length; i++)
        {
            digitMap[i + 1] = digits[i];
        }

        return digitMap;
    }

    // =========================================================
    // FISHER-YATES SHUFFLE
    // =========================================================

    private void Shuffle(int[] values)
    {
        for (int i = values.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            int temporary = values[i];
            values[i] = values[j];
            values[j] = temporary;
        }
    }

    // =========================================================
    // PUZZLE SIGNATURE
    // Used to compare visible puzzle arrangements.
    // =========================================================

    private string GetPuzzleSignature(int[,] board)
    {
        StringBuilder builder = new StringBuilder();

        for (int row = 0; row < GridSize; row++)
        {
            for (int col = 0; col < GridSize; col++)
            {
                builder.Append(board[row, col]);
            }
        }

        return builder.ToString();
    }

    // =========================================================
    // GENERATE BOARD
    // =========================================================

    private void GenerateBoard()
    {
        if (sudokuGrid == null)
        {
            Debug.LogError(
                "Sudoku Grid is NOT assigned!",
                this
            );
            return;
        }

        if (sudokuCellPrefab == null)
        {
            Debug.LogError(
                "Sudoku Cell Prefab is NOT assigned!",
                this
            );
            return;
        }

        // Remove previous generated cells.
        for (int i = sudokuGrid.childCount - 1; i >= 0; i--)
        {
            Destroy(sudokuGrid.GetChild(i).gameObject);
        }

        // Generate the randomized 6x6 board.
        for (int row = 0; row < GridSize; row++)
        {
            for (int col = 0; col < GridSize; col++)
            {
                GameObject cellObject =
                    Instantiate(sudokuCellPrefab, sudokuGrid);

                cellObject.name =
                    "Cell_" + (row * GridSize + col);

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
                        "SudokuCell component NOT FOUND on "
                        + cellObject.name,
                        cellObject
                    );
                }
            }
        }

        Debug.Log(
            "Sudoku board generated successfully."
        );
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

        // Hide banner and restore the top instruction.
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
            // Update the persistent top text only.
            // Do not reopen the large instruction panel.
            instructionManager.ShowChallengeCleared();
        }
        else
        {
            Debug.LogWarning(
                "SudokuManager: Room2InstructionManager "
                + "is not assigned!",
                this
            );
        }
    }

    // =========================================================
    // RESET PUZZLE
    // Called by Room2GameOverController.TryAgain()
    // Also used when restarting Room 2 from the Pause Menu.
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

        // Close Sudoku panel and restore player control
        // through the existing controller.
        if (sudokuPanelController != null)
        {
            sudokuPanelController.ForceCloseSudoku();
        }
        else if (sudokuPanel != null)
        {
            sudokuPanel.SetActive(false);
        }

        // Generate a different randomized puzzle and
        // its matching solution before creating the board.
        GenerateRandomPuzzle();
        GenerateBoard();

        Debug.Log("SUDOKU RESET COMPLETE!");
    }
}
