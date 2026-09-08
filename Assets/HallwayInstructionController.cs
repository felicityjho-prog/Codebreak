using UnityEngine;
using System.Collections;

public class HallwayInstructionController : MonoBehaviour
{
    public GameObject instructionPanel;
    public float displayTime = 5f;

    [Header("Heartbeat Animation")]
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.04f;

    private Vector3 originalScale;

    void Start()
    {
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(true);

            originalScale = instructionPanel.transform.localScale;

            StartCoroutine(HeartbeatAnimation());

            Invoke(nameof(HideInstruction), displayTime);
        }
    }

    IEnumerator HeartbeatAnimation()
    {
        while (instructionPanel != null && instructionPanel.activeSelf)
        {
            // Grow
            float time = 0f;

            while (time < 1f)
            {
                time += Time.deltaTime * pulseSpeed;

                float scale = 1f + Mathf.Sin(time * Mathf.PI) * pulseAmount;

                instructionPanel.transform.localScale =
                    originalScale * scale;

                yield return null;
            }

            // Small pause
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