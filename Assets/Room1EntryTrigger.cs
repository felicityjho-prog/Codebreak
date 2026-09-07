using UnityEngine;

public class RoomEntryTrigger : MonoBehaviour
{
    [Header("Hallway")]
    public GameObject hallwayInstruction;

    [Header("Room 1 Challenge")]
    public GameObject challengeUI;

    [Header("Timer")]
    public GameTimer gameTimer;

    private bool triggered = false;

    private void Start()
    {
        // Hallway instruction is visible at the start
        if (hallwayInstruction != null)
        {
            hallwayInstruction.SetActive(true);
        }

        // Room 1 challenge is hidden at the start
        if (challengeUI != null)
        {
            challengeUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            // Hide hallway instruction
            if (hallwayInstruction != null)
            {
                hallwayInstruction.SetActive(false);
            }

            // Show Room 1 challenge
            if (challengeUI != null)
            {
                challengeUI.SetActive(true);
            }

            // Start timer
            if (gameTimer != null)
            {
                gameTimer.StartTimer();
            }
        }
    }
}