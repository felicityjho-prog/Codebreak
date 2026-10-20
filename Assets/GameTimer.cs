using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    [Header("Timer")]
    public float timeRemaining = 300f;
    private bool timerRunning = false;
    private bool gameOverTriggered = false;
    private float initialTimeRemaining;

    [Header("UI")]
    public TMP_Text timerText;
    public GameObject gameOverPanel;
    public GameObject instructionPanel;

    [Header("Pause Menu")]
    [SerializeField] private PauseMenuController pauseMenuController;

    [Header("Player")]
    public MonoBehaviour playerMove;
    public MonoBehaviour playerLook;

    [Header("Timer Sound")]
    private AudioSource tickSound;

    [Header("Game Over Audio")]
    public AudioSource gameOverAudioSource;
    public AudioClip gameOverSound;

    [Range(0.5f, 1.5f)]
    public float gameOverPitch = 1f;

    [Header("Room 1 Background Music")]
    public AudioSource room1MusicSource;

    private void Start()
    {
        initialTimeRemaining = timeRemaining;

        // Hide Game Over at the beginning.
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Get AudioSource used for ticking.
        tickSound = GetComponent<AudioSource>();

        if (tickSound != null)
        {
            tickSound.playOnAwake = false;
            tickSound.loop = true;
            tickSound.pitch = 1f;
            tickSound.Stop();
        }

        // Prepare Game Over audio.
        if (gameOverAudioSource != null)
        {
            gameOverAudioSource.playOnAwake = false;
            gameOverAudioSource.pitch = gameOverPitch;
            gameOverAudioSource.Stop();
        }

        // Timer is NOT running yet.
        timerRunning = false;

        // Hide timer at the beginning.
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }

        // Hide Room 1 instruction at the beginning.
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (!timerRunning || gameOverTriggered)
            return;

        if (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;

            // Prevent negative timer values.
            if (timeRemaining < 0f)
            {
                timeRemaining = 0f;
            }

            // Red when 30 seconds or less.
            if (timeRemaining <= 30f && timerText != null)
            {
                timerText.color = Color.red;
            }

            // Fast ticking during the last 10 seconds.
            if (timeRemaining <= 10f && tickSound != null)
            {
                tickSound.pitch = 1.5f;
            }

            UpdateTimerDisplay(timeRemaining);

            // Trigger Game Over when time reaches zero.
            if (timeRemaining <= 0f)
            {
                TimeUp();
            }
        }
    }

    // ==========================================
    // START TIMER
    // ==========================================

    public void StartTimer()
    {
        if (timerRunning || gameOverTriggered)
            return;

        timerRunning = true;

        // Start Room 1 background music again if stopped.
        PlayRoom1Music();

        // Show timer.
        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
            timerText.color = Color.white;
        }

        // Start ticking sound.
        if (tickSound != null)
        {
            tickSound.pitch = 1f;
            tickSound.Play();
        }

        // Display timer immediately.
        UpdateTimerDisplay(timeRemaining);
    }

    // ==========================================
    // ROOM 1 BACKGROUND MUSIC
    // ==========================================

    public void PlayRoom1Music()
    {
        if (room1MusicSource == null)
        {
            Debug.LogWarning(
                "GameTimer: Room 1 Music Source is not assigned!"
            );
            return;
        }

        room1MusicSource.loop = true;
        room1MusicSource.spatialBlend = 0f;

        // Play only if it is not already playing.
        if (!room1MusicSource.isPlaying)
        {
            room1MusicSource.Play();
            Debug.Log("ROOM 1 BACKGROUND MUSIC STARTED!");
        }
    }

    private void StopRoom1Music()
    {
        if (room1MusicSource != null &&
            room1MusicSource.isPlaying)
        {
            room1MusicSource.Stop();

            Debug.Log("ROOM 1 BACKGROUND MUSIC STOPPED!");
        }
    }

    // ==========================================
    // STOP TIMER
    // ==========================================

    public void StopTimer()
    {
        timerRunning = false;

        // Stop ticking and reset its speed.
        if (tickSound != null)
        {
            tickSound.Stop();
            tickSound.pitch = 1f;
        }

        // Hide timer.
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }
    }

    // ==========================================
    // TIME UP / GAME OVER
    // ==========================================

    private void TimeUp()
    {
        // Prevent Game Over from triggering more than once.
        if (gameOverTriggered)
            return;

        gameOverTriggered = true;

        // Stop timer and reset ticking pitch.
        StopTimer();

        // Stop Room 1 background music.
        StopRoom1Music();

        // Hide Room 1 instruction.
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
        }

        // Hide Pause button and Pause Menu overlay.
        if (pauseMenuController != null)
        {
            pauseMenuController.HidePauseMenuForGameOver();
        }
        else
        {
            Debug.LogWarning(
                "PauseMenuController is not assigned in GameTimer Inspector!"
            );
        }

        // Play Game Over sound at the configured pitch.
        if (gameOverAudioSource != null && gameOverSound != null)
        {
            gameOverAudioSource.Stop();
            gameOverAudioSource.pitch = gameOverPitch;
            gameOverAudioSource.PlayOneShot(gameOverSound);

            Debug.Log("ROOM 1 GAME OVER SOUND PLAYED!");
        }
        else
        {
            Debug.LogWarning(
                "Assign the Game Over AudioSource and AudioClip in the Inspector!"
            );
        }

        // Show Game Over Panel.
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Stop player movement.
        if (playerMove != null)
        {
            playerMove.enabled = false;
        }

        // Stop player camera look.
        if (playerLook != null)
        {
            playerLook.enabled = false;
        }

        // Unlock cursor for Game Over buttons.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("ROOM 1 GAME OVER!");
    }

    // ==========================================
    // TIMER DISPLAY
    // ==========================================

    private void UpdateTimerDisplay(float timeToDisplay)
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(timeToDisplay / 60f);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // ==========================================
    // RESTART
    // ==========================================

    public void RestartTimerForChallenge()
    {
        timerRunning = false;
        gameOverTriggered = false;
        timeRemaining = initialTimeRemaining;

        if (tickSound != null)
        {
            tickSound.Stop();
            tickSound.pitch = 1f;
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (timerText != null)
        {
            timerText.color = Color.white;
            timerText.gameObject.SetActive(false);
        }

        // StartTimer also restarts Room 1 music.
        StartTimer();

        Debug.Log("ROOM 1 TIMER RESET AND RESTARTED!");
    }
}