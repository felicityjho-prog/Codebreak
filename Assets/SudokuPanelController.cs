using UnityEngine;

public class SudokuPanelController : MonoBehaviour
{
    [Header("Sudoku Panel")]
    public GameObject sudokuPanel;

    [Header("Player Control")]
    public PlayerMovement playerMovement;
    public PlayerController playerLook;

    [Header("Camera")]
    public Transform playerCamera;

    [Header("Interaction Prompt")]
    public GameObject interactPrompt;

    // ==========================================
    // SUDOKU STATE
    // ==========================================

    private bool sudokuOpen = false;

    public bool IsSudokuOpen
    {
        get { return sudokuOpen; }
    }

    // ==========================================
    // SAVED CAMERA ROTATION
    // ==========================================

    private Quaternion savedCameraRotation;

    // ==========================================
    // UPDATE
    // ==========================================

    private void Update()
    {
        if (!sudokuOpen)
            return;

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;

        if (playerCamera != null)
        {
            playerCamera.localRotation =
                savedCameraRotation;
        }
    }

    // ==========================================
    // OPEN SUDOKU
    // ==========================================

    public void OpenSudoku()
    {
        Debug.Log(
            "SUDOKU OPENED - PLAYER FROZEN"
        );

        sudokuOpen = true;

        if (playerCamera != null)
        {
            savedCameraRotation =
                playerCamera.localRotation;
        }

        if (sudokuPanel != null)
        {
            sudokuPanel.SetActive(true);
        }

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (playerLook != null)
        {
            playerLook.DisableLook();
        }

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;
    }

    // ==========================================
    // CLOSE SUDOKU
    // ==========================================

    public void CloseSudoku()
    {
        Debug.Log(
            "SUDOKU CLOSED - PLAYER CONTROL RESTORED"
        );

        sudokuOpen = false;

        if (sudokuPanel != null)
        {
            sudokuPanel.SetActive(false);
        }

        if (playerCamera != null)
        {
            playerCamera.localRotation =
                savedCameraRotation;
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        if (playerLook != null)
        {
            playerLook.EnableLook();
        }

        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;
    }

    // ==========================================
    // FORCE CLOSE
    // GAME OVER / TRY AGAIN
    // ==========================================

    public void ForceCloseSudoku()
    {
        Debug.Log(
            "FORCE CLOSING SUDOKU"
        );

        sudokuOpen = false;

        if (sudokuPanel != null)
        {
            sudokuPanel.SetActive(false);
        }

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        if (playerLook != null)
        {
            playerLook.EnableLook();
        }

        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;

        Debug.Log(
            "SUDOKU FORCE CLOSE COMPLETE"
        );
    }
}