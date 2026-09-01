using UnityEngine;

public class SudokuBoardTrigger : MonoBehaviour
{
    [Header("Sudoku Board")]
    [SerializeField] private GameObject sudokuBoard;

    private void Start()
    {
        if (sudokuBoard != null)
        {
            sudokuBoard.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (sudokuBoard != null)
        {
            sudokuBoard.SetActive(true);
        }

        Debug.Log("Player reached the Sudoku table. Board activated!");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (sudokuBoard != null)
        {
            sudokuBoard.SetActive(false);
        }

        Debug.Log("Player left the Sudoku table. Board hidden!");
    }
}