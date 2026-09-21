using UnityEngine;
using System.Collections;

public class RoomEntryTrigger : MonoBehaviour
{
    [Header("Hallway")]
    public GameObject hallwayInstruction;

    [Header("Room 1 Challenge")]
    public GameObject challengeUI;

    [Header("Timer")]
    public GameTimer gameTimer;

    [Header("Voice Over")]
    public AudioSource voiceOver;

    [Header("Room 1 Interaction")]
    public Room1InteractionController room1Interaction;

    private bool triggered = false;

    private void Start()
    {
        // Show hallway instruction
        if (hallwayInstruction != null)
        {
            hallwayInstruction.SetActive(true);
        }

        // Hide Room 1 challenge UI
        if (challengeUI != null)
        {
            challengeUI.SetActive(false);
        }

        // Play voice over
        if (voiceOver != null && voiceOver.clip != null)
        {
            voiceOver.Play();
            StartCoroutine(WaitForVoiceToFinish());
        }
    }

    private IEnumerator WaitForVoiceToFinish()
    {
        while (voiceOver != null && voiceOver.isPlaying)
        {
            yield return null;
        }

        if (hallwayInstruction != null)
        {
            hallwayInstruction.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        triggered = true;

        Debug.Log("PLAYER ENTERED ROOM 1!");

        // Hide hallway instruction
        if (hallwayInstruction != null)
        {
            hallwayInstruction.SetActive(false);
        }

        // Show Room 1 challenge UI
        if (challengeUI != null)
        {
            challengeUI.SetActive(true);
        }

        // Start timer
        if (gameTimer != null)
        {
            gameTimer.StartTimer();
        }

        // Enable Room 1 non-clickable interactions
        if (room1Interaction != null)
        {
            room1Interaction.EnableRoom1Interactions();
        }
    }
}