using UnityEngine;
using System.Collections;

public class RoomEntryTrigger : MonoBehaviour
{
    [Header("Hallway")]
    public GameObject hallwayInstruction;

    [Header("Room 1 Challenge")]
    public GameObject challengeUI;

    [Header("Room 1 Instruction")]
    public GameObject instructionPanel;

    [Header("Timer")]
    public GameTimer gameTimer;

    [Header("Voice Over")]
    public AudioSource voiceOver;

    [Header("Room 1 Interaction")]
    public Room1InteractionController room1Interaction;

    [Header("Test Mode")]
    public RoomSwitcher roomSwitcher;

    private bool triggered = false;

    private void Start()
    {
        // =========================================
        // TEST ROOM 2 CHECK
        // =========================================
        if (roomSwitcher != null && roomSwitcher.testRoom2)
        {
            // Make sure Room 1 voice/UI does not start
            if (voiceOver != null)
            {
                voiceOver.Stop();
            }

            if (hallwayInstruction != null)
            {
                hallwayInstruction.SetActive(false);
            }

            if (challengeUI != null)
            {
                challengeUI.SetActive(false);
            }

            if (instructionPanel != null)
            {
                instructionPanel.SetActive(false);
            }

            return;
        }

        // =========================================
        // SHOW HALLWAY INSTRUCTION
        // =========================================
        if (hallwayInstruction != null)
        {
            hallwayInstruction.SetActive(true);
        }

        // =========================================
        // HIDE ROOM 1 CHALLENGE UI
        // =========================================
        if (challengeUI != null)
        {
            challengeUI.SetActive(false);
        }

        // =========================================
        // HIDE ROOM 1 INSTRUCTION UI
        // =========================================
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
        }

        // =========================================
        // PLAY VOICE OVER
        // =========================================
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

        // Only hide hallway instruction if the player
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

        // =========================================
        // DO NOT ALLOW ROOM 1 IN TEST ROOM 2
        // =========================================
        if (roomSwitcher != null && roomSwitcher.testRoom2)
        {
            return;
        }

        triggered = true;

        Debug.Log("PLAYER ENTERED ROOM 1!");

        // =========================================
        // HIDE HALLWAY INSTRUCTION
        // =========================================
        if (hallwayInstruction != null)
        {
            hallwayInstruction.SetActive(false);
        }

        // =========================================
        // STOP VOICE OVER
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
        // SHOW ROOM 1 INSTRUCTION
        // =========================================
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(true);
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