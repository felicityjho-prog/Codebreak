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

    private bool triggered = false;

    private void Start()
    {
        // Show Challenge 1 instruction
        if (hallwayInstruction != null)
        {
            hallwayInstruction.SetActive(true);
        }

        // Hide Room 1 checklist at the start
        if (challengeUI != null)
        {
            challengeUI.SetActive(false);
        }

        // Play voice over
        if (voiceOver != null && voiceOver.clip != null)
        {
            voiceOver.Play();

            // Wait until voice-over actually finishes
            StartCoroutine(WaitForVoiceToFinish());
        }
    }

    private IEnumerator WaitForVoiceToFinish()
    {
        // Wait while the voice is playing
        while (voiceOver != null && voiceOver.isPlaying)
        {
            yield return null;
        }

        // Voice-over finished → now hide instruction
        if (hallwayInstruction != null)
        {
            hallwayInstruction.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            // DO NOT hide hallway instruction here.
            // Voice-over controls when it disappears.

            // Show Room 1 checklist
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