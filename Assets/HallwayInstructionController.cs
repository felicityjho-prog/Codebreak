using UnityEngine;
using System.Collections;

public class HallwayInstructionController : MonoBehaviour
{
    public GameObject instructionPanel;

    [Header("Voice Over")]
    public AudioSource voiceOver;

    [Header("Test Mode")]
    public RoomSwitcher roomSwitcher;

    [Header("Heartbeat Animation")]
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.04f;

    private Vector3 originalScale;

    void Start()
    {
        // =========================================
        // TEST ROOM 2 CHECK
        // =========================================
        if (roomSwitcher != null && roomSwitcher.testRoom2)
        {
            if (voiceOver != null)
            {
                voiceOver.Stop();
            }

            if (instructionPanel != null)
            {
                instructionPanel.SetActive(false);
            }

            return;
        }

        // =========================================
        // SHOW INSTRUCTION PANEL
        // =========================================
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(true);

            originalScale = instructionPanel.transform.localScale;

            StartCoroutine(HeartbeatAnimation());
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

    IEnumerator WaitForVoiceToFinish()
    {
        while (voiceOver != null && voiceOver.isPlaying)
        {
            yield return null;
        }

        HideInstruction();
    }

    IEnumerator HeartbeatAnimation()
    {
        while (instructionPanel != null && instructionPanel.activeSelf)
        {
            float time = 0f;

            while (time < 1f)
            {
                time += Time.deltaTime * pulseSpeed;

                float scale = 1f + Mathf.Sin(time * Mathf.PI) * pulseAmount;

                instructionPanel.transform.localScale =
                    originalScale * scale;

                yield return null;
            }

            yield return new WaitForSeconds(0.15f);
        }
    }

    void HideInstruction()
    {
        if (instructionPanel != null)
        {
            instructionPanel.transform.localScale = originalScale;
            instructionPanel.SetActive(false);
        }
    }
}