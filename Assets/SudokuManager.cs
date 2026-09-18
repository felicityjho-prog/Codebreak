using UnityEngine;

public class SudokuManager : MonoBehaviour
{
    [Header("Sudoku Setup")]
    public Transform sudokuGrid;
    public GameObject sudokuCellPrefab;

    // =========================================================
    // 6 x 6 SUDOKU PUZZLE
    // 0 = EMPTY CELL
    // Number = GIVEN NUMBER
    // =========================================================

    private int[,] puzzle =
    {
        { 1, 0, 0, 4, 0, 6 },
        { 0, 5, 6, 0, 2, 0 },

        { 2, 0, 4, 0, 6, 1 },
        { 0, 6, 0, 2, 0, 4 },

        { 3, 0, 5, 0, 1, 0 },
        { 0, 1, 0, 3, 4, 0 }
    };

    // =========================================================
    // COMPLETE SOLUTION
    // =========================================================

    private int[,] solution =
    {
        { 1, 2, 3, 4, 5, 6 },
        { 4, 5, 6, 1, 2, 3 },

        { 2, 3, 4, 5, 6, 1 },
        { 5, 6, 1, 2, 3, 4 },

        { 3, 4, 5, 6, 1, 2 },
        { 6, 1, 2, 3, 4, 5 }
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