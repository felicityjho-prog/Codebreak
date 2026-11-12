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

    [Header("Game Over Audio")]
    public AudioSource gameOverAudioSource;
    public AudioClip gameOverSound;
    [Range(0f, 1f)]
    public float gameOverVolume = 1f;

    [Header("Main Menu")]
    public string mainMenuSceneName = "MainMenu";

    private bool gameOverShown;

    private void Awake()
    {
        if (gameOverAudioSource == null)
            gameOverAudioSource = GetComponent<AudioSource>();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
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
            tryAgainButton.onClick.RemoveListener(TryAgain);

        if (mainMenuButton != null)
            mainMenuButton.onClick.RemoveListener(MainMenu);
    }

    // SHOW GAME OVER

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

        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (room3Timer != null)
        {
            room3Timer.StopTimer();
            room3Timer.HideTimerDisplay();
        }

        if (codingChallengeManager != null &&
            codingChallengeManager.codingPanel != null)
        {
            codingChallengeManager.codingPanel.SetActive(false);
        }

        gameOverPanel.SetActive(true);
        gameOverPanel.transform.SetAsLastSibling();

        Canvas parentCanvas =
            gameOverPanel.GetComponentInParent<Canvas>();

        if (parentCanvas != null)
            parentCanvas.enabled = true;

        Canvas panelCanvas = gameOverPanel.GetComponent<Canvas>();

        if (panelCanvas == null)
            panelCanvas = gameOverPanel.AddComponent<Canvas>();

        panelCanvas.overrideSorting = true;
        panelCanvas.sortingOrder = 500;

        CanvasGroup[] groups =
            gameOverPanel.GetComponentsInChildren<CanvasGroup>(true);

        foreach (CanvasGroup group in groups)
        {
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }

        Image panelImage = gameOverPanel.GetComponent<Image>();

        if (panelImage != null)
        {
            Color imageColor = panelImage.color;
            imageColor.a = 1f;
            panelImage.color = imageColor;
            panelImage.enabled = true;
            panelImage.raycastTarget = true;
        }

        if (!gameOverPanel.activeInHierarchy)
        {
            Debug.LogError(
                "Room 3 Game Over panel is still inactive in hierarchy.",
                this
            );
            return;
        }

        gameOverShown = true;

        PlayGameOverSound();

        if (pauseMenuController != null)
            pauseMenuController.HidePauseMenuForGameOver();

        if (pauseButton != null)
            pauseButton.SetActive(false);

        if (pauseOverlay != null)
            pauseOverlay.SetActive(false);

        if (centerDot != null)
            centerDot.SetActive(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerController != null)
        {
            playerController.DisableLook();
            playerController.enabled = false;
        }

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

    // GAME OVER SOUND

    private void PlayGameOverSound()
    {
        if (gameOverSound == null)
        {
            Debug.LogWarning(
                "Room 3 Game Over Sound is not assigned.",
                this
            );
            return;
        }

        if (gameOverAudioSource == null)
            gameOverAudioSource = GetComponent<AudioSource>();

        if (gameOverAudioSource == null)
        {
            Debug.LogWarning(
                "Game Over Audio Source is not assigned.",
                this
            );
            return;
        }

        gameOverAudioSource.PlayOneShot(
            gameOverSound,
            gameOverVolume
        );

        Debug.Log("ROOM 3 GAME OVER SOUND PLAYED.", this);
    }

    // TRY AGAIN

    public void TryAgain()
    {
        RestartRoom3();
    }

    // RESTART ROOM 3

    public void RestartRoom3()
    {
        Debug.Log("RESTARTING ROOM 3 FROM THE BEGINNING.");

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

        if (gameOverAudioSource != null)
            gameOverAudioSource.Stop();

        if (pauseMenuController != null)
            pauseMenuController.HidePauseMenuForGameOver();

        if (pauseOverlay != null)
            pauseOverlay.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        gameOverShown = false;

        if (room3Root != null && !room3Root.activeSelf)
            room3Root.SetActive(true);

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

        if (room3Timer != null)
            room3Timer.ResetTimer();

        if (centerDot != null)
            centerDot.SetActive(true);

        if (controlsPanel != null)
            controlsPanel.SetActive(true);

        if (pauseButton != null)
            pauseButton.SetActive(true);

        instructionManager.ShowInstructions();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log(
            "ROOM 3 RESTART COMPLETE. " +
            "Waiting for instruction OKAY.",
            this
        );
    }

    // MAIN MENU

    public void MainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (gameOverAudioSource != null)
            gameOverAudioSource.Stop();

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