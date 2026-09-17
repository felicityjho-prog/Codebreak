using UnityEngine;

public class TableSudokuTrigger : MonoBehaviour
{
    public GameObject sudokuPanel;

    public void OpenSudoku()
    {
        if (sudokuPanel != null)
        {
            sudokuPanel.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}