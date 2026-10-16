
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenuController : MonoBehaviour
{
    [Header("Pause Menu UI")]
    [SerializeField] private GameObject pauseOverlay;

    [Header("Player Controls")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerController playerController;

    private bool isPaused;

    private bool movementWasEnabled;
    private bool controllerWasEnabled;
    private bool lookWasEnabled;

    private float previousTimeScale = 1f;
    private bool audioWasPaused;

    private CursorLockMode previousCursorLockState;
    private bool previousCursorVisible;

    private void Awake()
    {
        // Restore normal game time when MainGame loads.
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    private void Start()
    {
        if (pauseOverlay != null)
        {
            pauseOverlay.SetActive(false);
        }
        else
        {
            Debug.LogError(
                "PauseOverlay is not assigned!",
                this
            );
        }

        isPaused = false;
    }

    // ==========================================
    // PAUSE GAME
    // ==========================================

    public void PauseGame()
    {
        if (isPaused)
            return;

        if (pauseOverlay == null)
        {
            Debug.LogError(
                "Cannot pause: PauseOverlay is not assigned!",
                this
            );
            return;
        }

        // Remember current global states.
        previousTimeScale =
            Time.timeScale > 0f ? Time.timeScale : 1f;

        audioWasPaused = AudioListener.pause;

        previousCursorLockState = Cursor.lockState;
        previousCursorVisible = Cursor.visible;

        // Remember movement component state.
        movementWasEnabled =
            playerMovement != null &&
            playerMovement.enabled;

        // Remember camera component AND camera-look states.
        controllerWasEnabled =
            playerController != null &&
            playerController.enabled;

        lookWasEnabled =
            playerController != null &&
            playerController.IsLookEnabled;

        // Stop player movement.
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Stop camera input while the pause menu is open.
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Deselect active Sudoku/Crossword input.
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        // Show the pause menu.
        isPaused = true;
        pauseOverlay.SetActive(true);

        // Freeze gameplay, timers and audio.
        Time.timeScale = 0f;
        AudioListener.pause = true;

        // Allow clicking the pause menu.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("GAME PAUSED");
    }

    // ==========================================
    // RESUME GAME
    // ==========================================

    public void ResumeGame()
    {
        if (!isPaused)
            return;

        // Hide the pause menu.
        if (pauseOverlay != null)
        {
            pauseOverlay.SetActive(false);
        }

        // Restore global states.
        Time.timeScale = previousTimeScale;
        AudioListener.pause = audioWasPaused;

        // Restore movement to its previous state.
        if (playerMovement != null)
        {
            playerMovement.enabled = movementWasEnabled;
        }

        // Restore both camera component and look state.
        if (playerController != null)
        {
            playerController.enabled = controllerWasEnabled;

            if (controllerWasEnabled)
            {
                if (lookWasEnabled)
                {
                    // Normal gameplay: immediately restore camera look.
                    playerController.EnableLook();
                }
                else
                {
                    // Puzzle was open: keep camera look disabled.
                    playerController.DisableLook();
                }
            }
        }

        // Restore cursor appropriately.
        if (controllerWasEnabled && lookWasEnabled)
        {
            // Normal gameplay: hide cursor and lock it.
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            // Puzzle UI or previously disabled controls:
            // preserve the cursor state from before pausing.
            Cursor.lockState = previousCursorLockState;
            Cursor.visible = previousCursorVisible;
        }

        isPaused = false;

        Debug.Log("GAME RESUMED");
    }

    // ==========================================
    // PAUSE STATE
    // ==========================================

    public bool IsPaused()
    {
        return isPaused;
    }
}
