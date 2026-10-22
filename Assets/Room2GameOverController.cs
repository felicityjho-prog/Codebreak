
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public GameObject wordScrabblePanel;

    [Header("Puzzle Controllers")]
    public SudokuPanelController sudokuPanelController;
    public CrosswordPanelController crosswordPanelController;

    [Header("Puzzle Managers")]
    public SudokuManager sudokuManager;
    public CrosswordManager crosswordManager;
    public WordPuzzleManager wordPuzzleManager;

    [Header("Room 2 Instructions")]
    public Room2InstructionManager instructionManager;

    [Header("Room 2 Voice")]
    public Room2VoiceSubtitleManager room2VoiceManager;

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

        if (room2Timer != null)
            room2Timer.StopTimer();

        // Close Sudoku.
        if (sudokuPanelController != null)
        {
            sudokuPanelController.ForceCloseSudoku();
        }
        else if (sudokuPanel != null)
        {
            sudokuPanel.SetActive(false);
        }

        // Close Crossword.
        if (crosswordPanelController != null)
        {
            crosswordPanelController.ForceCloseCrossword();
        }
        else if (crosswordPanel != null)
        {
            crosswordPanel.SetActive(false);
        }

        // Hide interaction prompts.
        if (sudokuInteractPrompt != null)
            sudokuInteractPrompt.SetActive(false);

        if (crosswordInteractPrompt != null)
            crosswordInteractPrompt.SetActive(false);

        // Ensure Room 2 canvas is visible.
        if (room2Canvas != null)
            room2Canvas.SetActive(true);

        // Show Game Over panel.
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
            Debug.LogError("GAME OVER PANEL IS NOT ASSIGNED!");
        }

        // Freeze player movement.
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Disable camera look.
        if (playerLook != null)
            playerLook.DisableLook();

        // Unlock cursor for Game Over buttons.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("ROOM 2 GAME OVER PANEL ACTIVATED!");
    }

    // =========================================================
    // TRY AGAIN / RESTART ROOM 2
    // =========================================================

    public void TryAgain()
    {
        Debug.Log("=================================");
        Debug.Log("ROOM 2 TRY AGAIN / RESTART");
        Debug.Log("=================================");

        // Restore game time and audio.
        Time.timeScale = 1f;
        AudioListener.pause = false;

        // Reset Game Over state.
        gameOverShown = false;

        // Hide Game Over panel.
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Reset Room 2 challenge progress.
        if (ChallengeProgressManager.Instance != null)
        {
            ChallengeProgressManager.Instance.ResetRoom2Progress();
        }

        // Close Sudoku.
        if (sudokuPanelController != null)
            sudokuPanelController.ForceCloseSudoku();

        if (sudokuPanel != null)
            sudokuPanel.SetActive(false);

        // Close Crossword.
        if (crosswordPanelController != null)
            crosswordPanelController.ForceCloseCrossword();

        if (crosswordPanel != null)
            crosswordPanel.SetActive(false);

        // Reset Word Scrabble interaction after Try Again.
        WordPuzzleInteraction wordInteraction =
            FindFirstObjectByType<WordPuzzleInteraction>();

        if (wordInteraction != null)
        {
            wordInteraction.ResetInteractionForRoomRestart();
        }
        else if (wordScrabblePanel != null)
        {
            wordScrabblePanel.SetActive(false);

            Debug.LogWarning(
                "WordPuzzleInteraction was not found in the scene."
            );
        }

        // Hide interaction prompts.
        if (sudokuInteractPrompt != null)
            sudokuInteractPrompt.SetActive(false);

        if (crosswordInteractPrompt != null)
            crosswordInteractPrompt.SetActive(false);

        // Reset Sudoku.
        if (sudokuManager != null)
        {
            sudokuManager.ResetPuzzle();
        }
        else
        {
            Debug.LogWarning("SudokuManager is NOT assigned!");
        }

        // Reset Crossword.
        if (crosswordManager != null)
        {
            crosswordManager.ResetCrossword();
        }
        else
        {
            Debug.LogWarning("CrosswordManager is NOT assigned!");
        }

        // Reset Word Scrabble with a new random round.
        if (wordPuzzleManager != null)
        {
            wordPuzzleManager.RestartPuzzle();

            Debug.Log(
                "WORD SCRABBLE RESET: New random questions generated."
            );
        }
        else
        {
            Debug.LogWarning(
                "WordPuzzleManager is NOT assigned! " +
                "Assign it in the Inspector."
            );
        }

        // Return player to Room 2 spawn.
        if (player != null && room2SpawnPoint != null)
        {
            CharacterController controller =
                player.GetComponent<CharacterController>();

            if (controller != null)
                controller.enabled = false;

            player.SetPositionAndRotation(
                room2SpawnPoint.position,
                room2SpawnPoint.rotation
            );

            if (controller != null)
                controller.enabled = true;

            Debug.Log("Player returned to Room 2 spawn.");
        }
        else
        {
            Debug.LogError(
                "Player or Room 2 Spawn Point is NOT assigned!"
            );
        }

        // Restore player movement.
        if (playerMovement != null)
            playerMovement.enabled = true;

        // Restore camera look.
        if (playerLook != null)
        {
            playerLook.enabled = true;
            playerLook.EnableLook();
        }

        // Reset timer but do NOT start it yet.
        // GOT IT starts it through OnGotItClicked().
        if (room2Timer != null)
        {
            room2Timer.ResetTimer();

            Debug.Log(
                "ROOM 2 TIMER RESET. Waiting for GOT IT."
            );
        }

        // Reset and show the main instruction panel again.
        if (instructionManager != null)
        {
            instructionManager.ResetForTryAgain();
            instructionManager.ShowInstruction();

            Debug.Log("ROOM 2 INSTRUCTION PANEL SHOWN AGAIN.");
        }
        else
        {
            Debug.LogWarning(
                "Room2InstructionManager is NOT assigned!"
            );
        }

        // Replay the complete Room 2 voice sequence.
        if (room2VoiceManager != null)
        {
            room2VoiceManager.RestartVoiceSequence();

            Debug.Log("ROOM 2 VOICE SEQUENCE RESTARTED.");
        }
        else
        {
            Debug.LogWarning(
                "Room2VoiceSubtitleManager is NOT assigned!"
            );
        }

        // Keep cursor visible so the player can click GOT IT.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("=================================");
        Debug.Log("ROOM 2 RESTART COMPLETE!");
        Debug.Log("=================================");
    }

    // =========================================================
    // MAIN MENU
    // =========================================================

    public void MainMenu()
    {
        Debug.Log("RETURNING TO CODEBREAK MAIN MENU...");

        Time.timeScale = 1f;
        AudioListener.pause = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("MainMenu");
    }
}
