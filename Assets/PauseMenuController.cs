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

    [Header("Room 3 Restart")]
    [SerializeField] private GameObject room3Root;
    [SerializeField] private Room3GameOverController room3GameOverController;

    [Header("Current Room Detection")]
    [SerializeField] private GameObject room1GameManager;
    [SerializeField] private GameObject room2GameManager;
    [SerializeField] private GameObject room3GameManager;

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

    [Header("Cinematic Lock")]
    private bool pauseLockedByCinematic;
    private bool pauseButtonOriginalState;
    private bool pauseButtonStateCached;
    private bool cinematicLockClosedPause;

    private const float OptionsButtonSoundDelay = 0.15f;
    private const float MainMenuButtonSoundDelay = 0.4f;

    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    // =====================================================
    // START
    // =====================================================

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

    // =====================================================
    // CINEMATIC LOCK
    // =====================================================

    public void SetCinematicLock(bool locked)
    {
        if (pauseLockedByCinematic == locked)
            return;

        pauseLockedByCinematic = locked;

        if (locked)
        {
            // Remember the pause button's current visibility.
            if (pauseButton != null)
            {
                pauseButtonOriginalState = pauseButton.activeSelf;
                pauseButtonStateCached = true;

                pauseButton.SetActive(false);
            }

            // Hide the complete pause overlay.
            if (pauseOverlay != null)
                pauseOverlay.SetActive(false);

            // If paused, restore time/audio so the sequence can run.
            if (isPaused)
            {
                cinematicLockClosedPause = true;

                Time.timeScale =
                    previousTimeScale > 0f ? previousTimeScale : 1f;

                AudioListener.pause = audioWasPaused;

                isPaused = false;
                isOpeningOptions = false;
                isClosingOptions = false;

                if (EventSystem.current != null)
                    EventSystem.current.SetSelectedGameObject(null);
            }

            Debug.Log("Cinematic lock enabled. Pause menu hidden.");
        }
        else
        {
            // Restore pause-button visibility.
            if (pauseButton != null && pauseButtonStateCached)
                pauseButton.SetActive(pauseButtonOriginalState);

            pauseButtonStateCached = false;

            // Restore state saved by PauseGame if needed.
            if (cinematicLockClosedPause)
            {
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

                Cursor.lockState = previousCursorLockState;
                Cursor.visible = previousCursorVisible;

                cinematicLockClosedPause = false;
            }

            Debug.Log("Cinematic lock disabled. Pause menu available.");
        }
    }

    // =====================================================
    // PAUSE GAME
    // =====================================================

    public void PauseGame()
    {
        if (pauseLockedByCinematic ||
            isPaused ||
            pauseOverlay == null)
        {
            return;
        }

        previousTimeScale =
            Time.timeScale > 0f ? Time.timeScale : 1f;

        audioWasPaused = AudioListener.pause;

        previousCursorLockState = Cursor.lockState;
        previousCursorVisible = Cursor.visible;

        movementWasEnabled =
            playerMovement != null &&
            playerMovement.enabled;

        controllerWasEnabled =
            playerController != null &&
            playerController.enabled;

        lookWasEnabled =
            playerController != null &&
            playerController.IsLookEnabled;

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

    // =====================================================
    // OPEN OPTIONS
    // =====================================================

    public void OpenOptions()
    {
        if (pauseLockedByCinematic ||
            !isPaused ||
            isOpeningOptions)
        {
            return;
        }

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

        if (!isPaused || pauseLockedByCinematic)
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

    // =====================================================
    // CLOSE OPTIONS
    // =====================================================

    public void CloseOptions()
    {
        if (pauseLockedByCinematic ||
            !isPaused ||
            isClosingOptions)
        {
            return;
        }

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

        if (!isPaused || pauseLockedByCinematic)
        {
            isClosingOptions = false;
            yield break;
        }

        pauseSettingsPanel.SetActive(false);
        pauseMenuCard.SetActive(true);

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        // Keep the game and timer paused.
        isClosingOptions = false;

        Debug.Log("RETURNED TO PAUSE MENU");
    }

    // =====================================================
    // RESUME GAME
    // =====================================================

    public void ResumeGame()
    {
        if (pauseLockedByCinematic || !isPaused)
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

    // =====================================================
    // RESTART CURRENT CHALLENGE
    // =====================================================

    public void RestartChallenge()
    {
        if (pauseLockedByCinematic)
            return;

        // =============================================
        // DETERMINE CURRENT ROOM USING GAME MANAGERS
        // =============================================

        bool isRoom1Active =
            room1GameManager != null &&
            room1GameManager.activeInHierarchy;

        bool isRoom2Active =
            room2GameManager != null &&
            room2GameManager.activeInHierarchy;

        bool isRoom3Active =
            room3GameManager != null &&
            room3GameManager.activeInHierarchy;

        Debug.Log(
            $"RESTART MANAGER CHECK | " +
            $"Room1: {isRoom1Active} | " +
            $"Room2: {isRoom2Active} | " +
            $"Room3: {isRoom3Active}",
            this
        );

        int activeManagerCount = 0;

        if (isRoom1Active) activeManagerCount++;
        if (isRoom2Active) activeManagerCount++;
        if (isRoom3Active) activeManagerCount++;

        // Do not restart the wrong room if the state is ambiguous.
        if (activeManagerCount != 1)
        {
            Debug.LogError(
                "RESTART CANCELLED: Exactly one Room Game Manager " +
                "must be active. Check the Game Manager assignments " +
                "and active states in the Inspector.",
                this
            );

            return;
        }

        // =============================================
        // RESTORE TIME AND AUDIO
        // =============================================

        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (pauseOverlay != null)
            pauseOverlay.SetActive(false);

        isPaused = false;
        isOpeningOptions = false;
        isClosingOptions = false;

        // =============================================
        // ROOM 1 RESTART
        // =============================================

        if (isRoom1Active)
        {
            if (room1GameOverController == null)
            {
                Debug.LogError(
                    "Room1GameOverController is not assigned!",
                    this
                );
                return;
            }

            Debug.Log("RESTART ROUTED TO ROOM 1.", this);

            room1GameOverController.RestartFromHallway();
            return;
        }

        // =============================================
        // ROOM 2 RESTART
        // =============================================

        if (isRoom2Active)
        {
            if (room2GameOverController == null)
            {
                Debug.LogError(
                    "Room2GameOverController is not assigned!",
                    this
                );
                return;
            }

            Debug.Log("RESTART ROUTED TO ROOM 2.", this);

            room2GameOverController.TryAgain();
            return;
        }

        // =============================================
        // ROOM 3 RESTART
        // =============================================

        if (isRoom3Active)
        {
            if (room3GameOverController == null)
            {
                Debug.LogError(
                    "Room3GameOverController is not assigned!",
                    this
                );
                return;
            }

            Debug.Log("RESTART ROUTED TO ROOM 3.", this);

            room3GameOverController.RestartRoom3();
            return;
        }
    }

    // =====================================================
    // HIDE PAUSE UI WHEN GAME OVER
    // =====================================================

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

    // =====================================================
    // RESTORE PAUSE BUTTON
    // =====================================================

    public void ShowPauseButton()
    {
        if (pauseButton != null && !pauseLockedByCinematic)
            pauseButton.SetActive(true);
    }

    // =====================================================
    // RETURN TO MAIN MENU
    // =====================================================

    public void ReturnToMainMenu()
    {
        if (pauseLockedByCinematic || isReturningToMainMenu)
            return;

        isReturningToMainMenu = true;

        StartCoroutine(ReturnToMainMenuAfterClickSound());
    }

    private IEnumerator ReturnToMainMenuAfterClickSound()
    {
        yield return new WaitForSecondsRealtime(
            MainMenuButtonSoundDelay
        );

        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("MainMenu");
    }

    // =====================================================
    // PAUSE STATE
    // =====================================================

    public bool IsPaused()
    {
        return isPaused;
    }

    // Optional: lets other scripts check if pause is locked.
    public bool IsCinematicLocked()
    {
        return pauseLockedByCinematic;
    }
}