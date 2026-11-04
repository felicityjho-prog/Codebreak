
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
    public GameObject wordScrabblePanel;

    [Header("Game Over Sound")]
    public AudioSource gameOverAudioSource;
    public AudioClip gameOverSound;

    [Header("Room 2 Completion Sound")]
    public GameObject taskCompletePanel;
    public AudioSource completionAudioSource;
    public AudioClip completionSound;

    private float timeRemaining;

    private bool timerRunning = false;
    private bool timerFinished = false;
    private bool completionSoundPlayed = false;

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

        if (completionAudioSource != null)
        {
            completionAudioSource.playOnAwake = false;
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (sudokuPanel != null)
            sudokuPanel.SetActive(false);

        if (crosswordPanel != null)
            crosswordPanel.SetActive(false);

        if (wordScrabblePanel != null)
            wordScrabblePanel.SetActive(false);

        UpdateTimerDisplay();
    }

    // ==========================================
    // UPDATE
    // ==========================================

    private void Update()
    {
        CheckCompletionPanelSound();

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
    // ROOM 2 COMPLETION SOUND
    // ==========================================

    private void CheckCompletionPanelSound()
    {
        if (taskCompletePanel == null)
            return;

        bool panelIsVisible = taskCompletePanel.activeInHierarchy;

        if (panelIsVisible && !completionSoundPlayed)
        {
            completionSoundPlayed = true;

            AudioSource soundSource =
                completionAudioSource != null
                ? completionAudioSource
                : gameOverAudioSource;

            if (soundSource != null && completionSound != null)
            {
                soundSource.PlayOneShot(completionSound);

                Debug.Log("ROOM 2 COMPLETION SOUND PLAYED!");
            }
            else
            {
                Debug.LogWarning(
                    "Room 2 completion sound is not fully assigned. " +
                    "Check Task Complete Panel, Completion Sound, " +
                    "and Audio Source in the Inspector."
                );
            }
        }
        else if (!panelIsVisible)
        {
            completionSoundPlayed = false;
        }
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
                timerAudioSource.Play();
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
            timerAudioSource.Stop();

        Debug.Log("ROOM 2 TIMER STOPPED!");
    }

    // ==========================================
    // PAUSE TIMER
    // KNOWLEDGE ARCHIVE
    // ==========================================

    public void PauseTimer()
    {
        if (timerFinished)
        {
            Debug.Log(
                "TIMER CANNOT PAUSE - TIMER ALREADY FINISHED!"
            );

            return;
        }

        timerRunning = false;

        if (timerAudioSource != null)
            timerAudioSource.Stop();

        Debug.Log(
            "ROOM 2 TIMER PAUSED! REMAINING TIME: " +
            timeRemaining
        );
    }

    // ==========================================
    // RESUME TIMER
    // AFTER CLOSING KNOWLEDGE ARCHIVE
    // ==========================================

    public void ResumeTimer()
    {
        if (timerFinished)
        {
            Debug.Log(
                "TIMER CANNOT RESUME - GAME OVER ALREADY OCCURRED!"
            );

            return;
        }

        timerRunning = true;

        if (timerText != null)
            timerText.gameObject.SetActive(true);

        UpdateTimerWarning();
        UpdateTimerDisplay();

        if (timerAudioSource != null && tickingSound != null)
        {
            timerAudioSource.clip = tickingSound;
            timerAudioSource.loop = true;

            if (!timerAudioSource.isPlaying)
                timerAudioSource.Play();
        }

        Debug.Log(
            "ROOM 2 TIMER RESUMED! REMAINING TIME: " +
            timeRemaining
        );
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
            timeRemaining = 0f;

        UpdateTimerDisplay();
        UpdateTimerWarning();

        Debug.Log(
            "WRONG ANSWER! -" +
            wrongAnswerPenalty +
            " seconds."
        );

        if (timeRemaining <= 0f)
            TimeUp();
    }

    // ==========================================
    // TIMER WARNING
    // ==========================================

    private void UpdateTimerWarning()
    {
        if (timerText != null)
        {
            if (timeRemaining <= redTimeThreshold)
                timerText.color = Color.red;
            else
                timerText.color = normalTimerColor;
        }

        if (timerAudioSource != null)
        {
            if (timeRemaining <= fastTickThreshold)
                timerAudioSource.pitch = fastTickPitch;
            else
                timerAudioSource.pitch = normalTickPitch;
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
    // CLOSE ALL PUZZLE PANELS
    // ==========================================

    private void ClosePuzzlePanels()
    {
        if (sudokuPanel != null)
        {
            sudokuPanel.SetActive(false);
            Debug.Log("SUDOKU PANEL CLOSED!");
        }

        if (crosswordPanel != null)
        {
            crosswordPanel.SetActive(false);
            Debug.Log("CROSSWORD PANEL CLOSED!");
        }

        if (wordScrabblePanel != null)
        {
            wordScrabblePanel.SetActive(false);
            Debug.Log("WORD SCRABBLE PANEL CLOSED!");
        }
        else
        {
            Debug.LogWarning(
                "Word Scrabble Panel is not assigned " +
                "in Room2ChallengeTimer Inspector!"
            );
        }
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

        if (timerAudioSource != null)
        {
            timerAudioSource.Stop();
            timerAudioSource.pitch = normalTickPitch;
        }

        if (timerText != null)
            timerText.color = Color.red;

        Debug.Log("ROOM 2 TIME UP!");

        ClosePuzzlePanels();

        if (pauseMenuController != null)
        {
            pauseMenuController.HidePauseMenuForGameOver();
        }
        else
        {
            Debug.LogWarning(
                "Pause Menu Controller is not assigned!"
            );
        }

        if (gameOverAudioSource != null && gameOverSound != null)
        {
            gameOverAudioSource.PlayOneShot(gameOverSound);

            Debug.Log("GAME OVER SOUND PLAYED!");
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            gameOverPanel.transform.SetAsLastSibling();

            Debug.Log("ROOM 2 GAME OVER PANEL ACTIVATED!");
        }
        else
        {
            Debug.LogError(
                "ROOM 2 GAME OVER PANEL IS NOT ASSIGNED!"
            );
        }

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
        timeRemaining = startingTime;
        timerRunning = false;
        timerFinished = false;

        if (timerText != null)
        {
            timerText.color = normalTimerColor;
            timerText.gameObject.SetActive(false);
        }

        if (timerAudioSource != null)
        {
            timerAudioSource.Stop();
            timerAudioSource.pitch = normalTickPitch;
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        ClosePuzzlePanels();

        if (pauseMenuController != null)
            pauseMenuController.ShowPauseButton();

        UpdateTimerDisplay();

        Debug.Log("ROOM 2 TIMER RESET!");
    }
}
