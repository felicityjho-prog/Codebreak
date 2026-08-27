using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    [Header("Timer")]
    public float timeRemaining = 300f;
    private bool timerRunning = true;

    [Header("UI")]
    public TMP_Text timerText;
    public GameObject gameOverPanel;

    [Header("Player")]
    public MonoBehaviour playerMove;
    public MonoBehaviour playerLook;

    [Header("Sound")]
    private AudioSource tickSound;

    void Start()
    {
        gameOverPanel.SetActive(false);

        // Get Audio Source
        tickSound = GetComponent<AudioSource>();

        // Start Clock Ticking
        if (tickSound != null)
        {
            tickSound.Play();
        }
    }

    void Update()
    {
        if (!timerRunning)
            return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 30)
            {
                timerText.color = Color.red;
            }

            // FAST TICKING LAST 10 SECONDS
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

            TimeUp();
        }
    }

    void TimeUp()
    {
        // Stop ticking sound
        if (tickSound != null)
        {
            tickSound.Stop();
        }

        // Show Game Over Panel
        gameOverPanel.SetActive(true);

        // Unlock mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Freeze Player
        if (playerMove != null)
            playerMove.enabled = false;

        if (playerLook != null)
            playerLook.enabled = false;
    }

    void UpdateTimerDisplay(float timeToDisplay)
    {
        timeToDisplay += 1;

        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}