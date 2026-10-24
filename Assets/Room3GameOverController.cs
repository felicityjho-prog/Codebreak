using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Room3GameOverController : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public PlayerMovement playerMovement;
    public PlayerController playerController;

    [Header("Room 3")]
    public GameObject room3Root;
    public Transform room3SpawnPoint;

    [Header("Room 3 Panels")]
    public GameObject gameOverPanel;
    public Room3CodingChallengeManager codingChallengeManager;
    public Room3InstructionManager instructionManager;
    public Room3ChallengeTimer room3Timer;

    [Header("Global UI")]
    public GameObject centerDot;
    public GameObject controlsPanel;
    public GameObject pauseButton;
    public GameObject pauseOverlay;

    [Header("Pause Menu Controller")]
    public PauseMenuController pauseMenuController;

    [Header("Game Over Buttons")]
    public Button tryAgainButton;
    public Button mainMenuButton;

    [Header("Main Menu")]
    public string mainMenuSceneName = "MainMenu";

    private bool gameOverShown;

    // =====================================================
    // INITIALIZATION
    // =====================================================

    private void Awake()
    {
        // Hide the panel at startup.
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (tryAgainButton != null)
        {
            tryAgainButton.onClick.RemoveListener(TryAgain);
            tryAgainButton.onClick.AddListener(TryAgain);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveListener(MainMenu);
            mainMenuButton.onClick.AddListener(MainMenu);
        }
    }

    private void OnDisable()
    {
        if (tryAgainButton != null)
        {
            tryAgainButton.onClick.RemoveListener(TryAgain);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveListener(MainMenu);
        }
    }

    // =====================================================
    // SHOW GAME OVER
    // =====================================================

    public void ShowGameOver()
    {
        if (gameOverShown)
            return;

        if (gameOverPanel == null)
        {
            Debug.LogError(
                "Room3GameOverController: Game Over Panel is not assigned!",
                this
            );
            return;
        }

        // Restore normal time/audio before showing the panel.
        Time.timeScale = 1f;
        AudioListener.pause = false;

        // Stop and hide the Room 3 timer.
        if (room3Timer != null)
        {
            room3Timer.StopTimer();
            room3Timer.HideTimerDisplay();
        }

        // Close the coding panel.
        if (codingChallengeManager != null &&
            codingChallengeManager.codingPanel != null)
        {
            codingChallengeManager.codingPanel.SetActive(false);
        }

        // Activate the panel FIRST.
        gameOverPanel.SetActive(true);
        gameOverPanel.transform.SetAsLastSibling();

        // Ensure the parent Canvas is enabled.
        Canvas parentCanvas =
            gameOverPanel.GetComponentInParent<Canvas>();

        if (parentCanvas != null)
        {
            parentCanvas.enabled = true;
        }

        // Give the Game Over panel its own sorting order.
        Canvas panelCanvas = gameOverPanel.GetComponent<Canvas>();

        if (panelCanvas == null)
        {
            panelCanvas = gameOverPanel.AddComponent<Canvas>();
        }

        panelCanvas.overrideSorting = true;
        panelCanvas.sortingOrder = 500;

        // Restore visibility and interaction for every CanvasGroup
        // inside the Game Over panel.
        CanvasGroup[] groups =
            gameOverPanel.GetComponentsInChildren<CanvasGroup>(true);

        foreach (CanvasGroup group in groups)
        {
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }

        // Make the panel background visible.
        UnityEngine.UI.Image panelImage =
            gameOverPanel.GetComponent<UnityEngine.UI.Image>();

        if (panelImage != null)
        {
            Color imageColor = panelImage.color;
            imageColor.a = 1f;
            panelImage.color = imageColor;
            panelImage.enabled = true;
            panelImage.raycastTarget = true;
        }

        // Verify the GameObject is active before hiding gameplay UI.
        if (!gameOverPanel.activeInHierarchy)
        {
            Debug.LogError(
                "Room 3 Game Over panel is still inactive in hierarchy.",
                this
            );
            return;
        }

        gameOverShown = true;

        // Hide the ordinary pause UI.
        if (pauseMenuController != null)
        {
            pauseMenuController.HidePauseMenuForGameOver();
        }

        if (pauseButton != null)
            pauseButton.SetActive(false);

        if (pauseOverlay != null)
            pauseOverlay.SetActive(false);

        // Hide gameplay controls ONLY after the Game Over panel
        // has been activated and its visibility settings restored.
        if (centerDot != null)
            centerDot.SetActive(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        // Freeze the player.
        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerController != null)
        {
            playerController.DisableLook();
            playerController.enabled = false;
        }

        // Unlock the cursor for the Game Over buttons.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log(
            "ROOM 3 GAME OVER PANEL ACTIVATED. " +
            "Active In Hierarchy: " +
            gameOverPanel.activeInHierarchy +
            ", Canvas Sorting Order: " +
            panelCanvas.sortingOrder,
            this
        );
    }

    // =====================================================
    // TRY AGAIN
    // =====================================================

    public void TryAgain()
    {
        RestartRoom3();
    }

    // =====================================================
    // RESTART ROOM 3 FROM THE BEGINNING
    // =====================================================

    public void RestartRoom3()
    {
        Debug.Log("RESTARTING ROOM 3 FROM THE BEGINNING.");

        // Validate the required objects before resetting.
        if (player == null)
        {
            Debug.LogError(
                "Room3GameOverController: Player is not assigned.",
                this
            );
            return;
        }

        if (room3SpawnPoint == null)
        {
            Debug.LogError(
                "Room3GameOverController: Room 3 Spawn Point " +
                "is not assigned.",
                this
            );
            return;
        }

        if (instructionManager == null)
        {
            Debug.LogError(
                "Room3GameOverController: Room 3 Instruction " +
                "Manager is not assigned.",
                this
            );
            return;
        }

        Time.timeScale = 1f;
        AudioListener.pause = false;

        // Hide pause and Game Over UI.
        if (pauseMenuController != null)
        {
            pauseMenuController.HidePauseMenuForGameOver();
        }

        if (pauseOverlay != null)
            pauseOverlay.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        gameOverShown = false;

        // Keep Room 3 active.
        if (room3Root != null &&
            !room3Root.activeSelf)
        {
            room3Root.SetActive(true);
        }

        // Teleport the player to the Room 3 spawn point.
        CharacterController characterController =
            player.GetComponent<CharacterController>();

        if (characterController != null)
            characterController.enabled = false;

        player.SetPositionAndRotation(
            room3SpawnPoint.position,
            room3SpawnPoint.rotation
        );

        if (characterController != null)
            characterController.enabled = true;

        // Reset answer, hints and completion state.
        if (codingChallengeManager != null)
        {
            codingChallengeManager.ResetForRoomRestart();
        }
        else
        {
            Debug.LogWarning(
                "Room3CodingChallengeManager is not assigned.",
                this
            );
        }

        // Reset the timer. It starts after the player clicks OKAY.
        if (room3Timer != null)
            room3Timer.ResetTimer();

        // Restore the gameplay HUD.
        if (centerDot != null)
            centerDot.SetActive(true);

        if (controlsPanel != null)
            controlsPanel.SetActive(true);

        if (pauseButton != null)
            pauseButton.SetActive(true);

        // Reopen the instruction panel and freeze controls.
        instructionManager.ShowInstructions();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log(
            "ROOM 3 RESTART COMPLETE. " +
            "Waiting for instruction OKAY.",
            this
        );
    }

    // =====================================================
    // MAIN MENU
    // =====================================================

    public void MainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (string.IsNullOrWhiteSpace(mainMenuSceneName))
        {
            Debug.LogError(
                "Room3GameOverController: Main Menu Scene Name " +
                "is empty.",
                this
            );
            return;
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }
}