using TMPro;
using UnityEngine;

public class Room3ChallengeTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float startingTime = 180f;

    [Header("Timer UI")]
    public TMP_Text timerText;

    [Header("Timer Warning Settings")]
    public float redTimeThreshold = 30f;
    public Color warningTimerColor = Color.red;

    [Header("Timer Sound")]
    public AudioSource timerAudioSource;
    public AudioClip tickingSound;

    [Header("Timer Sound Speed")]
    public float fastTickThreshold = 10f;
    public float normalTickPitch = 1f;
    public float fastTickPitch = 1.8f;

    [Header("Game Over")]
    public Room3GameOverController gameOverController;

    private float timeRemaining;
    private bool timerRunning;
    private bool timerFinished;
    private Color normalTimerColor = Color.white;

    private void Awake()
    {
        timeRemaining = startingTime;

        if (timerText != null)
        {
            normalTimerColor = timerText.color;
            timerText.text = FormatTime(timeRemaining);
            timerText.gameObject.SetActive(false);
        }

        if (timerAudioSource != null)
        {
            timerAudioSource.Stop();
            timerAudioSource.loop = true;
            timerAudioSource.playOnAwake = false;
            timerAudioSource.pitch = normalTickPitch;
        }
    }

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
        UpdateTimerSoundSpeed();
    }

    // START TIMER
    public void StartTimer()
    {
        if (timerFinished)
            return;

        timerRunning = true;

        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
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

            Debug.Log("ROOM 3 TICKING SOUND STARTED.");
        }

        UpdateTimerDisplay();

        Debug.Log("ROOM 3 TIMER STARTED.");
    }

    // STOP TIMER
    public void StopTimer()
    {
        timerRunning = false;

        if (timerAudioSource != null)
        {
            timerAudioSource.Stop();
            timerAudioSource.pitch = normalTickPitch;
        }

        Debug.Log("ROOM 3 TIMER STOPPED.");
    }

    // RESET TIMER
    public void ResetTimer()
    {
        timeRemaining = startingTime;
        timerRunning = false;
        timerFinished = false;

        if (timerAudioSource != null)
        {
            timerAudioSource.Stop();
            timerAudioSource.pitch = normalTickPitch;
        }

        if (timerText != null)
        {
            timerText.color = normalTimerColor;
            timerText.text = FormatTime(timeRemaining);
            timerText.gameObject.SetActive(false);
        }

        Debug.Log("ROOM 3 TIMER RESET.");
    }

    // TIMER SOUND SPEED
    private void UpdateTimerSoundSpeed()
    {
        if (timerAudioSource == null || tickingSound == null)
            return;

        if (timeRemaining <= fastTickThreshold)
        {
            timerAudioSource.pitch = fastTickPitch;
        }
        else
        {
            timerAudioSource.pitch = normalTickPitch;
        }
    }

    // HIDE TIMER DISPLAY
    public void HideTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }
    }

    // GET REMAINING TIME
    public float GetRemainingTime()
    {
        return timeRemaining;
    }

    // CHECK TIMER RUNNING
    public bool IsTimerRunning()
    {
        return timerRunning;
    }

    // TIME UP / GAME OVER
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

        HideTimerDisplay();

        Debug.Log("ROOM 3 TIME UP!");

        if (gameOverController != null)
        {
            gameOverController.ShowGameOver();
        }
        else
        {
            Debug.LogError(
                "Room3ChallengeTimer: Game Over Controller " +
                "is not assigned.",
                this
            );
        }
    }

    // UPDATE TIMER DISPLAY AND COLOR
    private void UpdateTimerDisplay()
    {
        if (timerText == null)
            return;

        timerText.text = FormatTime(timeRemaining);

        // Turn red when 30 seconds or less remain.
        if (timeRemaining <= redTimeThreshold)
        {
            timerText.color = warningTimerColor;
        }
        else
        {
            timerText.color = normalTimerColor;
        }
    }

    // FORMAT TIME
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}