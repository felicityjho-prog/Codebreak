using UnityEngine;
using TMPro;

public class Room2ChallengeTimer : MonoBehaviour
{
    public static Room2ChallengeTimer Instance;

    [Header("Timer Settings")]
    public float startingTime = 900f; // 15 minutes
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


    // =========================================================
    // AWAKE
    // =========================================================

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


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        timeRemaining = startingTime;

        // -----------------------------------------------------
        // TIMER TEXT
        // -----------------------------------------------------

        if (timerText != null)
        {
            normalTimerColor = timerText.color;

            timerText.gameObject.SetActive(false);
        }


        // -----------------------------------------------------
        // TICKING AUDIO
        // -----------------------------------------------------

        if (timerAudioSource != null)
        {
            timerAudioSource.Stop();
            timerAudioSource.loop = true;
            timerAudioSource.playOnAwake = false;
            timerAudioSource.pitch = normalTickPitch;
        }


        // -----------------------------------------------------
        // GAME OVER AUDIO
        // -----------------------------------------------------

        if (gameOverAudioSource != null)
        {
            gameOverAudioSource.Stop();
            gameOverAudioSource.playOnAwake = false;
        }


        // -----------------------------------------------------
        // HIDE GAME OVER
        // -----------------------------------------------------

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }


        // -----------------------------------------------------
        // CLOSE PUZZLES
        // -----------------------------------------------------

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


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (!timerRunning)
            return;

        if (timerFinished)
            return;


        timeRemaining -= Time.deltaTime;


        // -----------------------------------------------------
        // TIME REACHED ZERO
        // -----------------------------------------------------

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


    // =========================================================
    // START TIMER
    // =========================================================

    public void StartTimer()
    {
        if (timerFinished)
            return;

        timerRunning = true;


        // -----------------------------------------------------
        // SHOW TIMER
        // -----------------------------------------------------

        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);

            timerText.color = normalTimerColor;
        }


        // -----------------------------------------------------
        // START TICKING
        // -----------------------------------------------------

        if (timerAudioSource != null &&
            tickingSound != null)
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


    // =========================================================
    // STOP TIMER
    // =========================================================

    public void StopTimer()
    {
        timerRunning = false;

        if (timerAudioSource != null)
        {
            timerAudioSource.Stop();
        }

        Debug.Log("ROOM 2 TIMER STOPPED!");
    }


    // =========================================================
    // WRONG ANSWER
    // =========================================================

    public void WrongAnswer()
    {
        if (!timerRunning)
            return;

        if (timerFinished)
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


    // =========================================================
    // TIMER WARNING
    // =========================================================

    private void UpdateTimerWarning()
    {
        // -----------------------------------------------------
        // RED TIMER
        // -----------------------------------------------------

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


        // -----------------------------------------------------
        // FAST TICK
        // -----------------------------------------------------

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


    // =========================================================
    // UPDATE TIMER TEXT
    // =========================================================

    private void UpdateTimerDisplay()
    {
        if (timerText == null)
            return;


        int minutes =
            Mathf.FloorToInt(
                timeRemaining / 60f
            );

        int seconds =
            Mathf.FloorToInt(
                timeRemaining % 60f
            );


        timerText.text =
            string.Format(
                "{0:00}:{1:00}",
                minutes,
                seconds
            );
    }


    // =========================================================
    // TIME UP
    // =========================================================

    private void TimeUp()
    {
        if (timerFinished)
            return;


        timerFinished = true;
        timerRunning = false;


        // -----------------------------------------------------
        // STOP TICKING
        // -----------------------------------------------------

        if (timerAudioSource != null)
        {
            timerAudioSource.Stop();
        }


        // -----------------------------------------------------
        // MAKE TIMER RED
        // -----------------------------------------------------

        if (timerText != null)
        {
            timerText.color = Color.red;
        }


        Debug.Log("ROOM 2 TIME UP!");


        // -----------------------------------------------------
        // CLOSE SUDOKU
        // -----------------------------------------------------

        if (sudokuPanel != null)
        {
            sudokuPanel.SetActive(false);

            Debug.Log("SUDOKU PANEL CLOSED!");
        }


        // -----------------------------------------------------
        // CLOSE CROSSWORD
        // -----------------------------------------------------

        if (crosswordPanel != null)
        {
            crosswordPanel.SetActive(false);

            Debug.Log("CROSSWORD PANEL CLOSED!");
        }


        // -----------------------------------------------------
        // PLAY GAME OVER SOUND
        // -----------------------------------------------------

        if (gameOverAudioSource != null &&
            gameOverSound != null)
        {
            gameOverAudioSource.PlayOneShot(gameOverSound);

            Debug.Log("GAME OVER SOUND PLAYED!");
        }


        // -----------------------------------------------------
        // SHOW GAME OVER PANEL
        // -----------------------------------------------------

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


        // -----------------------------------------------------
        // UNLOCK CURSOR
        // -----------------------------------------------------

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    // =========================================================
    // GET TIME
    // =========================================================

    public float GetRemainingTime()
    {
        return timeRemaining;
    }


    // =========================================================
    // CHECK RUNNING
    // =========================================================

    public bool IsTimerRunning()
    {
        return timerRunning;
    }


    // =========================================================
    // RESET TIMER
    // =========================================================

    public void ResetTimer()
    {
        // -----------------------------------------------------
        // RESET TIME
        // -----------------------------------------------------

        timeRemaining = startingTime;

        timerRunning = false;
        timerFinished = false;


        // -----------------------------------------------------
        // RESET TIMER TEXT
        // -----------------------------------------------------

        if (timerText != null)
        {
            timerText.color = normalTimerColor;

            timerText.gameObject.SetActive(false);
        }


        // -----------------------------------------------------
        // STOP TICKING
        // -----------------------------------------------------

        if (timerAudioSource != null)
        {
            timerAudioSource.Stop();

            timerAudioSource.pitch = normalTickPitch;
        }


        // -----------------------------------------------------
        // HIDE GAME OVER
        // -----------------------------------------------------

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }


        // -----------------------------------------------------
        // CLOSE SUDOKU
        // -----------------------------------------------------

        if (sudokuPanel != null)
        {
            sudokuPanel.SetActive(false);
        }


        // -----------------------------------------------------
        // CLOSE CROSSWORD
        // -----------------------------------------------------

        if (crosswordPanel != null)
        {
            crosswordPanel.SetActive(false);
        }


        UpdateTimerDisplay();


        Debug.Log("ROOM 2 TIMER RESET!");
    }
}