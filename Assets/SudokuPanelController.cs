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

    // IMPORTANT:
    // TableInteraction can check this
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
        // While Sudoku is open:
        // FORCE CAMERA TO STAY STILL
        if (sudokuOpen)
        {
            // Keep cursor available
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Prevent camera rotation
            if (playerCamera != null)
            {
                playerCamera.localRotation =
                    savedCameraRotation;
            }
        }
    }

    // ==========================================
    // OPEN SUDOKU
    // ==========================================

    public void OpenSudoku()
    {
        Debug.Log("SUDOKU OPENED - PLAYER FROZEN");

        sudokuOpen = true;

        // --------------------------------------
        // SAVE CAMERA ROTATION
        // --------------------------------------

        if (playerCamera != null)
        {
            savedCameraRotation =
                playerCamera.localRotation;
        }

        // --------------------------------------
        // SHOW SUDOKU PANEL
        // --------------------------------------

        if (sudokuPanel != null)
        {
            sudokuPanel.SetActive(true);
        }

        // --------------------------------------
        // HIDE INTERACTION PROMPT
        // --------------------------------------

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }

        // --------------------------------------
        // DISABLE WASD MOVEMENT
        // --------------------------------------

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // --------------------------------------
        // DISABLE CAMERA LOOK
        // --------------------------------------

        if (playerLook != null)
        {
            playerLook.DisableLook();
        }

        // --------------------------------------
        // UNLOCK CURSOR
        // --------------------------------------

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ==========================================
    // CLOSE SUDOKU
    // ==========================================

    public void CloseSudoku()
    {
        Debug.Log("SUDOKU CLOSED - PLAYER CONTROL RESTORED");

        sudokuOpen = false;

        // --------------------------------------
        // HIDE SUDOKU PANEL
        // --------------------------------------

        if (sudokuPanel != null)
        {
            sudokuPanel.SetActive(false);
        }

        // --------------------------------------
        // RESTORE CAMERA
        // --------------------------------------

        if (playerCamera != null)
        {
            playerCamera.localRotation =
                savedCameraRotation;
        }

        // --------------------------------------
        // ENABLE WASD MOVEMENT
        // --------------------------------------

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        // --------------------------------------
        // ENABLE CAMERA LOOK
        // --------------------------------------

        if (playerLook != null)
        {
            playerLook.EnableLook();
        }

        // --------------------------------------
        // LOCK CURSOR
        // --------------------------------------

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}