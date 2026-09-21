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

            // TIME UP
            TimeUp();
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

        // Display timer immediately
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
    // TIME UP / GAME OVER
    // ==========================================

    private void TimeUp()
    {
        // Stop timer and ticking sound
        StopTimer();

        // Show Game Over Panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Stop player movement
        if (playerMove != null)
        {
            playerMove.enabled = false;
        }

        // Stop player camera look
        if (playerLook != null)
        {
            playerLook.enabled = false;
        }

        // Unlock cursor so player can click the panel
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ==========================================
    // TIMER DISPLAY
    // ==========================================

    private void UpdateTimerDisplay(float timeToDisplay)
    {
        if (timerText == null)
            return;

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