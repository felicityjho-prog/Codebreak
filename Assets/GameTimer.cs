using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    [Header("Timer")]
    public float timeRemaining = 300f;
    private bool timerRunning = false;

    [Header("UI")]
    public TMP_Text timerText;
    public GameObject gameOverPanel;

    [Header("Player")]
    public MonoBehaviour playerMove;
    public MonoBehaviour playerLook;

    [Header("Sound")]
    private AudioSource tickSound;

    private void Start()
    {
        // Hide Game Over at the beginning
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Get Audio Source
        tickSound = GetComponent<AudioSource>();

        // Timer is NOT running yet
        timerRunning = false;

        // Hide timer at the beginning
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!timerRunning)
            return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            // Red when 30 seconds or less
            if (timeRemaining <= 30 && timerText != null)
            {
                timerText.color = Color.red;
            }

            // Fast ticking during last 10 seconds
            if (timeRemaining <= 10 && tickSound != null)
            {
                tickSound.pitch = 1.5f;
            }

            UpdateTimerDisplay(timeRemaining);
        }
        else
        {
            timeRemaining = 0;
            timerRunning = false;

            UpdateTimerDisplay(timeRemaining);

            StopTimer();
        }
    }

    // ==========================================
    // START TIMER
    // ==========================================

    public void StartTimer()
    {
        if (timerRunning)
            return;

        timerRunning = true;

        // Show timer
        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
            timerText.color = Color.white;
        }

        // Start ticking sound
        if (tickSound != null)
        {
            tickSound.pitch = 1f;
            tickSound.Play();
        }

        // Display 05:00 immediately
        UpdateTimerDisplay(timeRemaining);
    }

    // ==========================================
    // STOP TIMER
    // ==========================================

    public void StopTimer()
    {
        timerRunning = false;

        // Stop ticking sound
        if (tickSound != null)
        {
            tickSound.Stop();
        }
    }

    // ==========================================
    // TIMER DISPLAY
    // ==========================================

    private void UpdateTimerDisplay(float timeToDisplay)
    {
        if (timerText == null)
            return;

        timeToDisplay += 1;

        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // ==========================================
    // RESTART
    // ==========================================

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}