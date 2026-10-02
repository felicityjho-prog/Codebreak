using UnityEngine;
using System.Collections;
using TMPro;

public class Room2InstructionManager : MonoBehaviour
{
    [Header("Instruction UI")]
    public CanvasGroup instructionCanvasGroup;

    [Header("Instruction Text")]
    public TMP_Text instructionText;

    [Header("Fade Settings")]
    public float fadeDuration = 0.3f;

    private Coroutine fadeCoroutine;

    void Start()
    {
        if (instructionCanvasGroup != null)
        {
            instructionCanvasGroup.alpha = 0f;
        }
    }

    // ==========================================
    // AFTER CLICKING GOT IT
    // ==========================================

    public void ShowInstruction()
    {
        if (instructionText != null)
        {
            instructionText.text =
                "CHALLENGE 1 AWAITS — PROCEED TO TABLE 1";
        }

        FadeTo(1f);
    }

    // ==========================================
    // HIDE INSTRUCTION
    // ==========================================

    public void HideInstruction()
    {
        FadeTo(0f);
    }

    // ==========================================
    // AFTER TABLE 1
    // ==========================================

    public void ShowChallengeCleared()
    {
        if (instructionText != null)
        {
            instructionText.text =
                "CHALLENGE CLEARED — PROCEED TO TABLE 2";
        }

        FadeTo(1f);
    }

    // ==========================================
    // TABLE 2
    // ==========================================

    public void ShowNextTableInstruction()
    {
        if (instructionText != null)
        {
            instructionText.text =
                "NEXT CHALLENGE AWAITS — FIND THE TABLE";
        }

        FadeTo(1f);
    }

    // ==========================================
    // TABLE 3
    // ==========================================

    public void ShowFinalChallengeInstruction()
    {
        if (instructionText != null)
        {
            instructionText.text =
                "FINAL CHALLENGE AWAITS — PROCEED TO THE LAST TABLE";
        }

        FadeTo(1f);
    }

    // ==========================================
    // FADE
    // ==========================================

    void FadeTo(float targetAlpha)
    {
        if (instructionCanvasGroup == null)
            return;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(
            FadeCoroutine(targetAlpha)
        );
    }

    IEnumerator FadeCoroutine(float targetAlpha)
    {
        float startAlpha =
            instructionCanvasGroup.alpha;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float t =
                elapsed / fadeDuration;

            instructionCanvasGroup.alpha =
                Mathf.Lerp(
                    startAlpha,
                    targetAlpha,
                    t
                );

            yield return null;
        }

        instructionCanvasGroup.alpha =
            targetAlpha;
    }
}