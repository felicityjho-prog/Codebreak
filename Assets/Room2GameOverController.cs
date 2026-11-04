
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

    [Header("Knowledge Archive")]
    public KnowledgeArchiveController knowledgeArchiveController;

    [Header("Puzzle Managers")]
    public SudokuManager sudokuManager;
    public CrosswordManager crosswordManager;
    public WordPuzzleManager wordPuzzleManager;

    [Header("Room 2 Instructions")]
    public Room2InstructionManager instructionManager;

    [Header("Room 2 Instruction Panel Controller")]
    public Room2InstructionPanel instructionPanelController;

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

        if (sudokuPanelController != null)
        {
            sudokuPanelController.ForceCloseSudoku();
        }
        else if (sudokuPanel != null)
        {
            sudokuPanel.SetActive(false);
        }

        if (crosswordPanelController != null)
        {
            crosswordPanelController.ForceCloseCrossword();
        }
        else if (crosswordPanel != null)
        {
            crosswordPanel.SetActive(false);
        }

        if (sudokuInteractPrompt != null)
            sudokuInteractPrompt.SetActive(false);

        if (crosswordInteractPrompt != null)
            crosswordInteractPrompt.SetActive(false);

        if (room2Canvas != null)
            room2Canvas.SetActive(true);

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

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerLook != null)
            playerLook.DisableLook();

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

        gameOverShown = false;

        // Hide Game Over panel.
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Reset Room 2 challenge progress.
        if (ChallengeProgressManager.Instance != null)
        {
            ChallengeProgressManager.Instance.ResetRoom2Progress();
        }

        // RESET NEW KNOWLEDGE ARCHIVE.
        if (knowledgeArchiveController != null)
        {
            knowledgeArchiveController.ResetForRoomRestart();

            Debug.Log(
                "KNOWLEDGE ARCHIVE RESET FOR ROOM RESTART!"
            );
        }
        else
        {
            Debug.LogWarning(
                "KnowledgeArchiveController is NOT assigned!"
            );
        }

        // Close and reset Sudoku.
        if (sudokuPanelController != null)
            sudokuPanelController.ForceCloseSudoku();

        if (sudokuPanel != null)
            sudokuPanel.SetActive(false);

        // Close and reset Crossword.
        if (crosswordPanelController != null)
        {
            crosswordPanelController.ForceCloseCrossword();

            // Reset completion state so Crossword can be solved again.
            crosswordPanelController.ResetForRoomRestart();

            Debug.Log("CROSSWORD CONTROLLER RESET!");
        }

        if (crosswordPanel != null)
            crosswordPanel.SetActive(false);

        // Reset Word Shuffle interaction.
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

        // Reset Sudoku puzzle.
        if (sudokuManager != null)
        {
            sudokuManager.ResetPuzzle();
        }
        else
        {
            Debug.LogWarning("SudokuManager is NOT assigned!");
        }

        // Reset Crossword answers and hints.
        if (crosswordManager != null)
        {
            crosswordManager.ResetCrossword();
        }
        else
        {
            Debug.LogWarning("CrosswordManager is NOT assigned!");
        }

        // Reset Word Shuffle with a new random round.
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

        // Reset timer, but do NOT start it yet.
        // START / GOT IT will start it through OnGotItClicked().
        if (room2Timer != null)
        {
            room2Timer.ResetTimer();

            Debug.Log(
                "ROOM 2 TIMER RESET. Waiting for GOT IT."
            );
        }

        // Reset instruction text and hide persistent top message.
        if (instructionManager != null)
        {
            instructionManager.ResetForTryAgain();
        }
        else
        {
            Debug.LogWarning(
                "Room2InstructionManager is NOT assigned!"
            );
        }

        // Freeze player movement and camera for the instruction panel.
        if (instructionPanelController != null)
        {
            instructionPanelController.ShowInstruction();

            Debug.Log(
                "ROOM 2 INSTRUCTION SHOWN - PLAYER MOVEMENT AND CAMERA FROZEN!"
            );
        }
        else
        {
            Debug.LogError(
                "Room2InstructionPanel Controller is NOT assigned!"
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

        // Show cursor for the Start / Got It button.
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
