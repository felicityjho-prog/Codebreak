using UnityEngine;
using TMPro;
using System.Collections;

public class NeonGlitch : MonoBehaviour
{
    public TMP_Text text;

    [Header("Flicker Settings")]
    public float minInterval = 0.5f;
    public float maxInterval = 1f;
    public float flickerDuration = 0.05f;

    [Header("Glitch Settings")]
    [Range(0f, 1f)]
    public float glitchChance = 0.15f;

    public float glitchDistance = 0.02f;

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.localPosition;

        StartCoroutine(GlitchLoop());
    }

    IEnumerator GlitchLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            if (Random.value <= glitchChance)
            {
                yield return StartCoroutine(DoGlitch());
            }
            else
            {
                yield return StartCoroutine(DoFlicker());
            }
        }
    }

    IEnumerator DoFlicker()
    {
        float originalAlpha = text.color.a;

        text.alpha = 0.15f;
        yield return new WaitForSeconds(flickerDuration);

        text.alpha = 0.8f;
        yield return new WaitForSeconds(0.04f);

        text.alpha = originalAlpha;
    }

    IEnumerator DoGlitch()
    {
        float originalAlpha = text.color.a;

        for (int i = 0; i < 3; i++)
        {
            transform.localPosition = originalPosition +
                new Vector3(
                    Random.Range(-glitchDistance, glitchDistance),
                    0,
                    0
                );

            text.alpha = Random.Range(0.3f, 1f);

            yield return new WaitForSeconds(0.03f);
        }

        transform.localPosition = originalPosition;
        text.alpha = originalAlpha;
    }
}