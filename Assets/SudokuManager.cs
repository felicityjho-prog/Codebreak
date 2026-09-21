using UnityEngine;

public class SudokuManager : MonoBehaviour
{
    [Header("Sudoku Setup")]
    public Transform sudokuGrid;
    public GameObject sudokuCellPrefab;

    // =========================================================
    // 6 x 6 SUDOKU
    // 0 = EMPTY / PLAYER INPUT
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
    // COMPLETE ANSWER
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

    void Start()
    {
        GenerateBoard();
    }

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
                        editable
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
}