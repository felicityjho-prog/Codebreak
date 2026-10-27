
using UnityEngine;
using System.Collections;

public class HallwayInstructionController : MonoBehaviour
{
    [Header("Instruction UI")]
    public GameObject instructionPanel;

    [Header("Voice Over")]
    // Kept for compatibility with existing Inspector assignments.
    // RoomEntryTrigger is responsible for playing the voice.
    public AudioSource voiceOver;

    [Header("Test Mode")]
    public RoomSwitcher roomSwitcher;

    [Header("Opening Cinematic")]
    public OpeningCinematicController openingCinematic;

    [Header("Heartbeat Animation")]
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.04f;

    private Vector3 originalScale;
    private bool originalScaleCaptured;
    private bool visualsRequested;
    private Coroutine heartbeatRoutine;

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        // Do not show Room 1 instructions during Room 2 Test Mode.
        if (roomSwitcher != null && roomSwitcher.testRoom2)
        {
            StopHallwayVoice();

            if (instructionPanel != null)
                instructionPanel.SetActive(false);

            visualsRequested = false;
            return;
        }

        // Wait for the opening cinematic to finish.
        if (openingCinematic != null &&
            openingCinematic.ShouldDelayHallwaySequence)
        {
            StopHallwayVoice();

            if (instructionPanel != null)
                instructionPanel.SetActive(false);

            visualsRequested = false;
            return;
        }

        // Only show the instruction visuals.
        // RoomEntryTrigger owns the hallway voice playback.
        BeginInstructionVisualsOnly();
    }

    // =====================================================
    // SHOW INSTRUCTION VISUALS
    // Does not start the AI voice.
    // =====================================================

    public void BeginInstructionVisualsOnly()
    {
        if (roomSwitcher != null && roomSwitcher.testRoom2)
            return;

        if (instructionPanel == null)
            return;

        visualsRequested = true;

        if (!originalScaleCaptured)
        {
            originalScale =
                instructionPanel.transform.localScale;

            originalScaleCaptured = true;
        }

        instructionPanel.SetActive(true);

        instructionPanel.transform.localScale =
            originalScale;

        if (heartbeatRoutine == null && isActiveAndEnabled)
        {
            heartbeatRoutine =
                StartCoroutine(HeartbeatAnimation());
        }
    }

    // =====================================================
    // STOP HALLWAY VOICE
    // =====================================================

    private void StopHallwayVoice()
    {
        if (voiceOver != null && voiceOver.isPlaying)
            voiceOver.Stop();
    }

    // =====================================================
    // ENABLE
    // =====================================================

    private void OnEnable()
    {
        if (visualsRequested &&
            instructionPanel != null &&
            instructionPanel.activeSelf &&
            heartbeatRoutine == null)
        {
            heartbeatRoutine =
                StartCoroutine(HeartbeatAnimation());
        }
    }

    // =====================================================
    // DISABLE
    // =====================================================

    private void OnDisable()
    {
        if (heartbeatRoutine != null)
        {
            StopCoroutine(heartbeatRoutine);
            heartbeatRoutine = null;
        }

        if (instructionPanel != null &&
            originalScaleCaptured)
        {
            instructionPanel.transform.localScale =
                originalScale;
        }
    }

    // =====================================================
    // HEARTBEAT ANIMATION
    // =====================================================

    private IEnumerator HeartbeatAnimation()
    {
        while (instructionPanel != null &&
               instructionPanel.activeSelf)
        {
            float time = 0f;

            while (time < 1f)
            {
                if (instructionPanel == null ||
                    !instructionPanel.activeSelf)
                {
                    heartbeatRoutine = null;
                    yield break;
                }

                time += Time.deltaTime * pulseSpeed;

                float scale =
                    1f +
                    Mathf.Sin(time * Mathf.PI) * pulseAmount;

                instructionPanel.transform.localScale =
                    originalScale * scale;

                yield return null;
            }

            yield return new WaitForSeconds(0.15f);
        }

        heartbeatRoutine = null;
    }
}
