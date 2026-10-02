using UnityEngine;

public class Room2GameOverController : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Player Script")]
    public MonoBehaviour playerMovement;

    [Header("Player Camera Controller")]
    public PlayerController playerLook;

    [Header("Room 2 Spawn")]
    public Transform room2SpawnPoint;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;

    [Header("Room 2 Canvas")]
    public GameObject room2Canvas;

    [Header("Room 2 Timer")]
    public Room2ChallengeTimer room2Timer;

    [Header("Puzzle Panels")]
    public GameObject sudokuPanel;
    public GameObject crosswordPanel;

    [Header("Puzzle Controllers")]
    public SudokuPanelController sudokuPanelController;
    public CrosswordPanelController crosswordPanelController;

    [Header("Puzzle Managers")]
    public SudokuManager sudokuManager;
    public CrosswordManager crosswordManager;

    [Header("Room 2 Instructions")]
    public Room2InstructionManager instructionManager;

    [Header("Interaction Prompts")]
    public GameObject sudokuInteractPrompt;
    public GameObject crosswordInteractPrompt;

    private bool gameOverShown = false;


    // =========================================================
    // GAME OVER
    // =========================================================

    public void ShowGameOver()
    {
        if (gameOverShown)
            return;

        gameOverShown = true;

        Debug.Log("=================================");
        Debug.Log("ROOM 2 GAME OVER");
        Debug.Log("=================================");

        // =====================================================
        // STOP TIMER
        // =====================================================

        if (room2Timer != null)
        {
            room2Timer.StopTimer();
        }

        // =====================================================
        // FORCE CLOSE SUDOKU
        // =====================================================

        if (sudokuPanelController != null)
        {
            sudokuPanelController.ForceCloseSudoku();
        }
        else if (sudokuPanel != null)
        {
            sudokuPanel.SetActive(false);
        }

        // =====================================================
        // FORCE CLOSE CROSSWORD
        // =====================================================

        if (crosswordPanelController != null)
        {
            crosswordPanelController.ForceCloseCrossword();
        }
        else if (crosswordPanel != null)
        {
            crosswordPanel.SetActive(false);
        }

        // =====================================================
        // HIDE INTERACTION PROMPTS
        // =====================================================

        if (sudokuInteractPrompt != null)
        {
            sudokuInteractPrompt.SetActive(false);
        }

        if (crosswordInteractPrompt != null)
        {
            crosswordInteractPrompt.SetActive(false);
        }

        // =====================================================
        // ENABLE ROOM 2 CANVAS
        // =====================================================

        if (room2Canvas != null)
        {
            room2Canvas.SetActive(true);
        }

        // =====================================================
        // SHOW GAME OVER PANEL
        // =====================================================

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            gameOverPanel.transform.SetAsLastSibling();

            CanvasGroup canvasGroup =
                gameOverPanel.GetComponent<CanvasGroup>();

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }
        else
        {
            Debug.LogError(
                "GAME OVER PANEL IS NOT ASSIGNED!"
            );
        }

        // =====================================================
        // FREEZE PLAYER
        // =====================================================

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // =====================================================
        // DISABLE CAMERA LOOK
        // =====================================================

        if (playerLook != null)
        {
            playerLook.DisableLook();
        }

        // =====================================================
        // UNLOCK CURSOR
        // =====================================================

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;

        Debug.Log(
            "ROOM 2 GAME OVER PANEL ACTIVATED!"
        );
    }


    // =========================================================
    // TRY AGAIN
    // =========================================================

    public void TryAgain()
    {
        Debug.Log(
            "================================="
        );

        Debug.Log(
            "ROOM 2 TRY AGAIN"
        );

        Debug.Log(
            "================================="
        );

        // =====================================================
        // RESET GAME OVER STATE
        // =====================================================

        gameOverShown = false;

        // =====================================================
        // HIDE GAME OVER
        // =====================================================

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // =====================================================
        // RESET ROOM 2 PROGRESS
        // =====================================================

        if (ChallengeProgressManager.Instance != null)
        {
            ChallengeProgressManager.Instance
                .ResetRoom2Progress();
        }

        // =====================================================
        // FORCE CLOSE SUDOKU
        // =====================================================

        if (sudokuPanelController != null)
        {
            sudokuPanelController.ForceCloseSudoku();
        }

        if (sudokuPanel != null)
        {
            sudokuPanel.SetActive(false);
        }

        // =====================================================
        // FORCE CLOSE CROSSWORD
        // =====================================================

        if (crosswordPanelController != null)
        {
            crosswordPanelController.ForceCloseCrossword();
        }

        if (crosswordPanel != null)
        {
            crosswordPanel.SetActive(false);
        }

        // =====================================================
        // HIDE INTERACTION PROMPTS
        // =====================================================

        if (sudokuInteractPrompt != null)
        {
            sudokuInteractPrompt.SetActive(false);
        }

        if (crosswordInteractPrompt != null)
        {
            crosswordInteractPrompt.SetActive(false);
        }

        // =====================================================
        // RESET SUDOKU
        // =====================================================

        if (sudokuManager != null)
        {
            sudokuManager.ResetPuzzle();
        }
        else
        {
            Debug.LogWarning(
                "SudokuManager is NOT assigned!"
            );
        }

        // =====================================================
        // RESET CROSSWORD
        // =====================================================

        if (crosswordManager != null)
        {
            crosswordManager.ResetCrossword();
        }
        else
        {
            Debug.LogWarning(
                "CrosswordManager is NOT assigned!"
            );
        }

        // =====================================================
        // MOVE PLAYER TO ROOM 2 SPAWN
        // =====================================================

        if (player != null &&
            room2SpawnPoint != null)
        {
            CharacterController controller =
                player.GetComponent<CharacterController>();

            if (controller != null)
            {
                controller.enabled = false;
            }

            player.position =
                room2SpawnPoint.position;

            player.rotation =
                room2SpawnPoint.rotation;

            if (controller != null)
            {
                controller.enabled = true;
            }

            Debug.Log(
                "Player returned to Room 2 spawn."
            );
        }

        // =====================================================
        // RESTORE PLAYER MOVEMENT
        // =====================================================

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        // =====================================================
        // RESTORE CAMERA LOOK
        // =====================================================

        if (playerLook != null)
        {
            playerLook.EnableLook();
        }

        // =====================================================
        // RESET TIMER AND START AGAIN
        // =====================================================

        if (room2Timer != null)
        {
            room2Timer.ResetTimer();
            room2Timer.StartTimer();

            Debug.Log(
                "ROOM 2 TIMER RESET AND STARTED."
            );
        }

        // =====================================================
        // RESET ROOM 2 INSTRUCTION
        // =====================================================

        if (instructionManager != null)
        {
            instructionManager.ResetForTryAgain();

            Debug.Log(
                "ROOM 2 INSTRUCTION RESET."
            );
        }
        else
        {
            Debug.LogWarning(
                "Room2InstructionManager is NOT assigned!"
            );
        }

        // =====================================================
        // LOCK CURSOR
        // =====================================================

        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;

        Debug.Log(
            "================================="
        );

        Debug.Log(
            "ROOM 2 TRY AGAIN COMPLETE!"
        );

        Debug.Log(
            "================================="
        );
    }


    // =========================================================
    // MAIN MENU
    // =========================================================

    public void MainMenu()
    {
        Debug.Log(
            "RETURNING TO CODEBREAK MAIN MENU..."
        );

        // Reset time scale
        Time.timeScale = 1f;

        // Unlock cursor
        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;

        // =====================================================
        // OPEN WEB MAIN MENU
        // =====================================================

        Application.OpenURL(
            "http://127.0.0.1:5500/index.html"
        );

        Debug.Log(
            "MAIN MENU URL OPENED:"
        );

        Debug.Log(
            "http://127.0.0.1:5500/index.html"
        );
    }
}