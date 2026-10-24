using TMPro;
using UnityEngine;

public class Room3ChallengeTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float startingTime = 180f;

    [Header("Timer UI")]
    public TMP_Text timerText;

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
    }

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

        UpdateTimerDisplay();

        Debug.Log("ROOM 3 TIMER STARTED.");
    }

    public void StopTimer()
    {
        timerRunning = false;
        Debug.Log("ROOM 3 TIMER STOPPED.");
    }

    public void ResetTimer()
    {
        timeRemaining = startingTime;
        timerRunning = false;
        timerFinished = false;

        if (timerText != null)
        {
            timerText.color = normalTimerColor;
            timerText.text = FormatTime(timeRemaining);
            timerText.gameObject.SetActive(false);
        }

        Debug.Log("ROOM 3 TIMER RESET.");
    }

    public void HideTimerDisplay()
    {
        if (timerText != null)
            timerText.gameObject.SetActive(false);
    }

    public float GetRemainingTime()
    {
        return timeRemaining;
    }

    public bool IsTimerRunning()
    {
        return timerRunning;
    }

    private void TimeUp()
    {
        if (timerFinished)
            return;

        timerFinished = true;
        timerRunning = false;

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

    private void UpdateTimerDisplay()
    {
        if (timerText == null)
            return;

        timerText.text = FormatTime(timeRemaining);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}