using UnityEngine;

public class SudokuUIController : MonoBehaviour
{
    [Header("Sudoku UI")]
    public GameObject sudokuPanel;

    [Header("Completion UI")]
    public GameObject completionBanner;

    private void Start()
    {
        if (completionBanner != null)
            completionBanner.SetActive(false);
    }

    public void ShowCompletion()
    {
        // Close Sudoku
        if (sudokuPanel != null)
            sudokuPanel.SetActive(false);

        // Show completion message
        if (completionBanner != null)
            completionBanner.SetActive(true);
    }
}