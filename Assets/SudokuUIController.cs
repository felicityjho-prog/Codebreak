using UnityEngine;

public class SudokuUIController : MonoBehaviour
{
    [Header("Sudoku UI")]
    public GameObject sudokuPanel;

    [Header("Completion UI")]
    public GameObject completionBanner;

    [Header("Sudoku Controller")]
    public SudokuPanelController sudokuPanelController;

    private void Start()
    {
        if (completionBanner != null)
        {
            completionBanner.SetActive(false);
        }
    }

    public void ShowCompletion()
    {
        // ==============================
        // CLOSE SUDOKU AND RESTORE PLAYER
        // ==============================

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

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // ==============================
        // SHOW COMPLETION BANNER
        // ==============================

        if (completionBanner != null)
        {
            completionBanner.SetActive(true);
        }
    }
}