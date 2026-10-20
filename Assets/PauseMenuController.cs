
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenuController : MonoBehaviour
{
    [Header("Pause Menu UI")]
    [SerializeField] private GameObject pauseOverlay;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject pauseMenuCard;
    [SerializeField] private GameObject pauseSettingsPanel;

    [Header("Player Controls")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerController playerController;

    [Header("Room 1 Restart")]
    [SerializeField] private GameObject room1Root;
    [SerializeField] private Room1GameOverController room1GameOverController;

    [Header("Room 2 Restart")]
    [SerializeField] private GameObject room2Root;
    [SerializeField] private Room2GameOverController room2GameOverController;

    private bool isPaused;
    private bool isOpeningOptions;
    private bool isClosingOptions;
    private bool isReturningToMainMenu;

    private bool movementWasEnabled;
    private bool controllerWasEnabled;
    private bool lookWasEnabled;

    private float previousTimeScale = 1f;
    private bool audioWasPaused;

    private CursorLockMode previousCursorLockState;
    private bool previousCursorVisible;

    private const float OptionsButtonSoundDelay = 0.15f;
    private const float MainMenuButtonSoundDelay = 0.4f;

    private void Awake()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    private void Start()
    {
        if (pauseMenuCard != null)
            pauseMenuCard.SetActive(true);

        if (pauseSettingsPanel != null)
            pauseSettingsPanel.SetActive(false);

        if (pauseOverlay != null)
            pauseOverlay.SetActive(false);
        else
            Debug.LogError("PauseOverlay is not assigned!", this);

        isPaused = false;
    }

    // ==========================================
    // PAUSE GAME
    // ==========================================

    public void PauseGame()
    {
        if (isPaused || pauseOverlay == null)
            return;

        previousTimeScale =
            Time.timeScale > 0f ? Time.timeScale : 1f;

        audioWasPaused = AudioListener.pause;
        previousCursorLockState = Cursor.lockState;
        previousCursorVisible = Cursor.visible;

        movementWasEnabled =
            playerMovement != null && playerMovement.enabled;

        controllerWasEnabled =
            playerController != null && playerController.enabled;

        lookWasEnabled =
            playerController != null && playerController.IsLookEnabled;

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerController != null)
            playerController.enabled = false;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        if (pauseMenuCard != null)
            pauseMenuCard.SetActive(true);

        if (pauseSettingsPanel != null)
            pauseSettingsPanel.SetActive(false);

        isPaused = true;
        pauseOverlay.SetActive(true);

        Time.timeScale = 0f;
        AudioListener.pause = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("GAME PAUSED");
    }

    // ==========================================
    // OPEN OPTIONS
    // ==========================================

    public void OpenOptions()
    {
        if (!isPaused || isOpeningOptions)
            return;

        if (pauseMenuCard == null || pauseSettingsPanel == null)
        {
            Debug.LogError(
                "Assign PauseMenuCard and PauseSettingsPanel " +
                "in the PauseMenuController Inspector.",
                this
            );
            return;
        }

        isOpeningOptions = true;
        StartCoroutine(OpenOptionsAfterClickSound());
    }

    private IEnumerator OpenOptionsAfterClickSound()
    {
        yield return new WaitForSecondsRealtime(
            OptionsButtonSoundDelay
        );

        if (!isPaused)
        {
            isOpeningOptions = false;
            yield break;
        }

        pauseMenuCard.SetActive(false);
        pauseSettingsPanel.SetActive(true);

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        isOpeningOptions = false;

        Debug.Log("PAUSE MENU OPTIONS OPENED");
    }

    // ==========================================
    // CLOSE OPTIONS - RETURN TO PAUSE MENU
    // ==========================================

    public void CloseOptions()
    {
        if (!isPaused || isClosingOptions)
            return;

        if (pauseSettingsPanel == null || pauseMenuCard == null)
        {
            Debug.LogError(
                "Assign PauseSettingsPanel and PauseMenuCard " +
                "in the PauseMenuController Inspector.",
                this
            );
            return;
        }

        isClosingOptions = true;
        StartCoroutine(CloseOptionsAfterClickSound());
    }

    private IEnumerator CloseOptionsAfterClickSound()
    {
        yield return new WaitForSecondsRealtime(
            OptionsButtonSoundDelay
        );

        if (!isPaused)
        {
            isClosingOptions = false;
            yield break;
        }

        pauseSettingsPanel.SetActive(false);
        pauseMenuCard.SetActive(true);

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        // Keep the game and timer paused.
        // Do not modify Time.timeScale or AudioListener.pause.

        isClosingOptions = false;

        Debug.Log("RETURNED TO PAUSE MENU");
    }

    // ==========================================
    // RESUME GAME
    // ==========================================

    public void ResumeGame()
    {
        if (!isPaused)
            return;

        if (pauseOverlay != null)
            pauseOverlay.SetActive(false);

        Time.timeScale = previousTimeScale;
        AudioListener.pause = audioWasPaused;

        if (playerMovement != null)
            playerMovement.enabled = movementWasEnabled;

        if (playerController != null)
        {
            playerController.enabled = controllerWasEnabled;

            if (controllerWasEnabled)
            {
                if (lookWasEnabled)
                    playerController.EnableLook();
                else
                    playerController.DisableLook();
            }
        }

        if (controllerWasEnabled && lookWasEnabled)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = previousCursorLockState;
            Cursor.visible = previousCursorVisible;
        }

        isPaused = false;

        Debug.Log("GAME RESUMED");
    }

    // ==========================================
    // RESTART CURRENT CHALLENGE
    // ==========================================

    public void RestartChallenge()
    {
        Debug.Log("RESTARTING CURRENT CHALLENGE...");

        if (pauseOverlay != null)
            pauseOverlay.SetActive(false);

        isPaused = false;

        Time.timeScale = 1f;
        AudioListener.pause = false;

        // ROOM 2: reuse its existing TryAgain logic.
        if (room2Root != null && room2Root.activeInHierarchy)
        {
            if (room2GameOverController == null)
            {
                Debug.LogError(
                    "Room2GameOverController is not assigned!"
                );
                return;
            }

            room2GameOverController.TryAgain();

            Debug.Log("ROOM 2 RESTART COMPLETE!");
            return;
        }

        // ROOM 1: return to the hallway and reset the challenge.
        if (room1Root != null && room1Root.activeInHierarchy)
        {
            if (room1GameOverController == null)
            {
                Debug.LogError(
                    "Room1GameOverController is not assigned!"
                );
                return;
            }

            room1GameOverController.RestartFromHallway();

            Debug.Log("ROOM 1 RESTART COMPLETE!");
            return;
        }

        Debug.LogError(
            "Cannot determine the current room. " +
            "Check the Room 1 Root and Room 2 Root assignments."
        );
    }

    // ==========================================
    // HIDE PAUSE UI WHEN GAME OVER
    // ==========================================

    public void HidePauseMenuForGameOver()
    {
        if (pauseButton != null)
            pauseButton.SetActive(false);

        if (pauseOverlay != null)
            pauseOverlay.SetActive(false);

        if (isPaused)
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
            isPaused = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Pause UI hidden for Game Over");
    }

    // ==========================================
    // RESTORE PAUSE BUTTON
    // ==========================================

    public void ShowPauseButton()
    {
        if (pauseButton != null)
            pauseButton.SetActive(true);
    }

    // ==========================================
    // RETURN TO MAIN MENU
    // ==========================================

    public void ReturnToMainMenu()
    {
        if (isReturningToMainMenu)
            return;

        isReturningToMainMenu = true;

        StartCoroutine(ReturnToMainMenuAfterClickSound());
    }

    private IEnumerator ReturnToMainMenuAfterClickSound()
    {
        // Give the click sound time to start playing.
        yield return new WaitForSecondsRealtime(
            MainMenuButtonSoundDelay
        );

        // Restore normal game state before changing scenes.
        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("MainMenu");
    }

    // ==========================================
    // PAUSE STATE
    // ==========================================

    public bool IsPaused()
    {
        return isPaused;
    }
}