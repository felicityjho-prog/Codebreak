
using UnityEngine;
using TMPro;

public class Room2ChallengeTimer : MonoBehaviour
{
    public static Room2ChallengeTimer Instance;

    [Header("Timer Settings")]
    public float startingTime = 900f;
    public float wrongAnswerPenalty = 10f;

    [Header("Timer Warning Settings")]
    public float redTimeThreshold = 30f;
    public float fastTickThreshold = 10f;

    [Header("UI")]
    public TMP_Text timerText;

    [Header("Timer Sound")]
    public AudioSource timerAudioSource;
    public AudioClip tickingSound;

    [Header("Timer Sound Speed")]
    public float normalTickPitch = 1f;
    public float fastTickPitch = 1.8f;

    [Header("Game Over")]
    public GameObject gameOverPanel;

    [Header("Pause Menu")]
    [SerializeField] private PauseMenuController pauseMenuController;

    [Header("Puzzle Panels")]
    public GameObject sudokuPanel;
    public GameObject crosswordPanel;

    [Header("Game Over Sound")]
    public AudioSource gameOverAudioSource;
    public AudioClip gameOverSound;

    private float timeRemaining;
    private bool timerRunning = false;
    private bool timerFinished = false;

    private Color normalTimerColor;

    // ==========================================
    // AWAKE
    // ==========================================

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // ==========================================
    // START
    // ==========================================

    private void Start()
    {
        timeRemaining = startingTime;

        if (timerText != null)
        {
            normalTimerColor = timerText.color;
            timerText.gameObject.SetActive(false);
        }

        if (timerAudioSource != null)
        {
            timerAudioSource.Stop();
            timerAudioSource.loop = true;
            timerAudioSource.playOnAwake = false;
            timerAudioSource.pitch = normalTickPitch;
        }

        if (gameOverAudioSource != null)
        {
            gameOverAudioSource.Stop();
            gameOverAudioSource.playOnAwake = false;
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (sudokuPanel != null)
        {
            sudokuPanel.SetActive(false);
        }

        if (crosswordPanel != null)
        {
            crosswordPanel.SetActive(false);
        }

        UpdateTimerDisplay();
    }

    // ==========================================
    // UPDATE
    // ==========================================

    private void Update()
    {
        if (!timerRunning || timerFinished)
            return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            UpdateTimerDisplay();
            TimeUp();
            return;
        }

        UpdateTimerDisplay();
        UpdateTimerWarning();
    }

    // ==========================================
    // START TIMER
    // ==========================================

    public void StartTimer()
    {
        if (timerFinished)
            return;

        timerRunning = true;

        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
            timerText.color = normalTimerColor;
        }

        if (timerAudioSource != null && tickingSound != null)
        {
            timerAudioSource.clip = tickingSound;
            timerAudioSource.loop = true;
            timerAudioSource.pitch = normalTickPitch;

            if (!timerAudioSource.isPlaying)
            {
                timerAudioSource.Play();
            }
        }

        UpdateTimerDisplay();

        Debug.Log("ROOM 2 TIMER STARTED!");
    }

    // ==========================================
    // STOP TIMER
    // ==========================================

    public void StopTimer()
    {
        timerRunning = false;

        if (timerAudioSource != null)
        {
            timerAudioSource.Stop();
        }

        Debug.Log("ROOM 2 TIMER STOPPED!");
    }

    // ==========================================
    // WRONG ANSWER
    // ==========================================

    public void WrongAnswer()
    {
        if (!timerRunning || timerFinished)
            return;

        timeRemaining -= wrongAnswerPenalty;

        if (timeRemaining < 0f)
        {
            timeRemaining = 0f;
        }

        UpdateTimerDisplay();
        UpdateTimerWarning();

        Debug.Log(
            "WRONG ANSWER! -" +
            wrongAnswerPenalty +
            " seconds."
        );

        if (timeRemaining <= 0f)
        {
            TimeUp();
        }
    }

    // ==========================================
    // TIMER WARNING
    // ==========================================

    private void UpdateTimerWarning()
    {
        if (timerText != null)
        {
            if (timeRemaining <= redTimeThreshold)
            {
                timerText.color = Color.red;
            }
            else
            {
                timerText.color = normalTimerColor;
            }
        }

        if (timerAudioSource != null)
        {
            if (timeRemaining <= fastTickThreshold)
            {
                timerAudioSource.pitch = fastTickPitch;
            }
            else
            {
                timerAudioSource.pitch = normalTickPitch;
            }
        }
    }

    // ==========================================
    // UPDATE TIMER TEXT
    // ==========================================

    private void UpdateTimerDisplay()
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

        timerText.text = string.Format(
            "{0:00}:{1:00}",
            minutes,
            seconds
        );
    }

    // ==========================================
    // TIME UP / GAME OVER
    // ==========================================

    private void TimeUp()
    {
        if (timerFinished)
            return;

        timerFinished = true;
        timerRunning = false;

        // Stop ticking sound.
        if (timerAudioSource != null)
        {
            timerAudioSource.Stop();
            timerAudioSource.pitch = normalTickPitch;
        }

        // Make timer red.
        if (timerText != null)
        {
            timerText.color = Color.red;
        }

        Debug.Log("ROOM 2 TIME UP!");

        // Close Sudoku.
        if (sudokuPanel != null)
        {
            sudokuPanel.SetActive(false);
            Debug.Log("SUDOKU PANEL CLOSED!");
        }

        // Close Crossword.
        if (crosswordPanel != null)
        {
            crosswordPanel.SetActive(false);
            Debug.Log("CROSSWORD PANEL CLOSED!");
        }

        // Hide Pause button and Pause Menu.
        if (pauseMenuController != null)
        {
            pauseMenuController.HidePauseMenuForGameOver();
        }
        else
        {
            Debug.LogWarning(
                "Pause Menu Controller is not assigned " +
                "in Room2ChallengeTimer Inspector!"
            );
        }

        // Play Game Over sound.
        if (gameOverAudioSource != null && gameOverSound != null)
        {
            gameOverAudioSource.PlayOneShot(gameOverSound);
            Debug.Log("GAME OVER SOUND PLAYED!");
        }

        // Show Game Over panel.
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Debug.Log("ROOM 2 GAME OVER PANEL ACTIVATED!");
        }
        else
        {
            Debug.LogError(
                "ROOM 2 GAME OVER PANEL IS NOT ASSIGNED!"
            );
        }

        // Unlock cursor for Game Over buttons.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ==========================================
    // GET TIME
    // ==========================================

    public float GetRemainingTime()
    {
        return timeRemaining;
    }

    // ==========================================
    // CHECK RUNNING
    // ==========================================

    public bool IsTimerRunning()
    {
        return timerRunning;
    }

    // ==========================================
    // RESET TIMER / TRY AGAIN
    // ==========================================

    public void ResetTimer()
    {
        // Reset timer state.
        timeRemaining = startingTime;
        timerRunning = false;
        timerFinished = false;

        // Reset timer text.
        if (timerText != null)
        {
            timerText.color = normalTimerColor;
            timerText.gameObject.SetActive(false);
        }

        // Stop ticking and restore normal pitch.
        if (timerAudioSource != null)
        {
            timerAudioSource.Stop();
            timerAudioSource.pitch = normalTickPitch;
        }

        // Hide Game Over.
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Close Sudoku.
        if (sudokuPanel != null)
        {
            sudokuPanel.SetActive(false);
        }

        // Close Crossword.
        if (crosswordPanel != null)
        {
            crosswordPanel.SetActive(false);
        }

        // Restore Pause button after restart.
        if (pauseMenuController != null)
        {
            pauseMenuController.ShowPauseButton();
        }

        UpdateTimerDisplay();

        Debug.Log("ROOM 2 TIMER RESET!");
    }
}
