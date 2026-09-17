using UnityEngine;

public class SudokuManager : MonoBehaviour
{
    [Header("Sudoku Setup")]
    public Transform sudokuGrid;
    public GameObject sudokuCellPrefab;

    void Start()
    {
        GenerateBoard();
    }

    void GenerateBoard()
    {
        // Remove the manually created cell
        foreach (Transform child in sudokuGrid)
        {
            Destroy(child.gameObject);
        }

        // Create 81 cells
        for (int i = 0; i < 81; i++)
        {
            GameObject cell = Instantiate(
                sudokuCellPrefab,
                sudokuGrid
            );

            cell.name = "Cell_" + i;
        }
    }
}