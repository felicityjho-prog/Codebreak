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

        // Only hide the hallway instruction if the player
        // has NOT entered Room 1 yet.
        if (!triggered && hallwayInstruction != null)
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

        // =========================================
        // HIDE HALLWAY INSTRUCTION IMMEDIATELY
        // =========================================
        if (hallwayInstruction != null)
        {
            hallwayInstruction.SetActive(false);
        }

        // =========================================
        // STOP VOICE OVER IMMEDIATELY
        // =========================================
        if (voiceOver != null && voiceOver.isPlaying)
        {
            voiceOver.Stop();
        }

        // =========================================
        // SHOW ROOM 1 CHALLENGE UI
        // =========================================
        if (challengeUI != null)
        {
            challengeUI.SetActive(true);
        }

        // =========================================
        // START TIMER
        // =========================================
        if (gameTimer != null)
        {
            gameTimer.StartTimer();
        }

        // =========================================
        // ENABLE ROOM 1 INTERACTIONS
        // =========================================
        if (room1Interaction != null)
        {
            room1Interaction.EnableRoom1Interactions();
        }
    }
}