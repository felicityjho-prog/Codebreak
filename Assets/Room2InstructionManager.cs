using UnityEngine;
using System.Collections;
using TMPro;

public class Room2InstructionManager : MonoBehaviour
{
    [Header("Instruction Panel")]
    public CanvasGroup instructionCanvasGroup;

    [Header("Instruction Text")]
    public TMP_Text instructionText;

    [Header("Persistent Challenge Instruction")]
    public CanvasGroup persistentInstructionCanvasGroup;

    [Header("Global Controls UI")]
    public GameObject controlsPanel;

    [Header("Global Center Dot")]
    public GameObject centerDot;

    [Header("Fade Settings")]
    public float fadeDuration = 0.3f;

    private Coroutine fadeCoroutine;
    private Coroutine persistentFadeCoroutine;

    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        // Hide the large instruction panel initially
        if (instructionCanvasGroup != null)
        {
            instructionCanvasGroup.alpha = 0f;
            instructionCanvasGroup.interactable = false;
            instructionCanvasGroup.blocksRaycasts = false;
        }

        // Hide top instruction initially.
        // It will appear after GOT IT.
        if (persistentInstructionCanvasGroup != null)
        {
            persistentInstructionCanvasGroup.alpha = 0f;
            persistentInstructionCanvasGroup.interactable = false;
            persistentInstructionCanvasGroup.blocksRaycasts = false;
        }
    }

    // =========================================================
    // SHOW INITIAL INSTRUCTION
    // =========================================================

    public void ShowInstruction()
    {
        if (instructionText != null)
        {
            instructionText.text =
                "CHALLENGE 1 AWAITS — PROCEED TO TABLE 1";
        }

        FadeTo(1f);
    }

    // =========================================================
    // GOT IT
    // =========================================================

    public void OnGotItClicked()
    {
        Debug.Log("ROOM 2 INSTRUCTION CONFIRMED.");

        // Hide large instruction panel
        HideInstruction();

        // Show top persistent instruction
        ShowPersistentInstruction();

        // =========================================
        // SHOW CONTROLS AGAIN
        // =========================================

        if (controlsPanel != null)
        {
            controlsPanel.SetActive(true);

            Debug.Log(
                "CONTROLS PANEL SHOWN AGAIN IN ROOM 2."
            );
        }

        // =========================================
        // SHOW CENTER DOT AGAIN
        // =========================================

        if (centerDot != null)
        {
            centerDot.SetActive(true);

            Debug.Log(
                "CENTER DOT SHOWN AGAIN IN ROOM 2."
            );
        }

        // =========================================
        // START ROOM 2 TIMER
        // =========================================

        if (Room2ChallengeTimer.Instance != null)
        {
            Room2ChallengeTimer.Instance.StartTimer();

            Debug.Log(
                "ROOM 2 GLOBAL TIMER STARTED AFTER GOT IT."
            );
        }
        else
        {
            Debug.LogWarning(
                "Room2ChallengeTimer Instance NOT FOUND!"
            );
        }
    }

    // =========================================================
    // SHOW PERSISTENT TOP INSTRUCTION
    // =========================================================

    public void ShowPersistentInstruction()
    {
        if (persistentInstructionCanvasGroup == null)
            return;

        if (persistentFadeCoroutine != null)
        {
            StopCoroutine(persistentFadeCoroutine);
        }

        persistentFadeCoroutine =
            StartCoroutine(
                FadePersistentInstruction(1f)
            );
    }

    // =========================================================
    // HIDE PERSISTENT TOP INSTRUCTION
    // =========================================================

    public void HidePersistentInstruction()
    {
        if (persistentInstructionCanvasGroup == null)
            return;

        if (persistentFadeCoroutine != null)
        {
            StopCoroutine(persistentFadeCoroutine);
        }

        persistentFadeCoroutine =
            StartCoroutine(
                FadePersistentInstruction(0f)
            );
    }

    // =========================================================
    // RESET ROOM 2 INSTRUCTION
    // =========================================================

    public void ResetForTryAgain()
    {
        Debug.Log("RESETTING ROOM 2 INSTRUCTIONS...");

        // Stop current fades
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        if (persistentFadeCoroutine != null)
        {
            StopCoroutine(persistentFadeCoroutine);
            persistentFadeCoroutine = null;
        }

        // Reset large instruction
        if (instructionText != null)
        {
            instructionText.text =
                "CHALLENGE 1 AWAITS — PROCEED TO TABLE 1";
        }

        if (instructionCanvasGroup != null)
        {
            instructionCanvasGroup.alpha = 0f;
            instructionCanvasGroup.interactable = false;
            instructionCanvasGroup.blocksRaycasts = false;
        }

        // Reset TOP instruction
        if (persistentInstructionCanvasGroup != null)
        {
            persistentInstructionCanvasGroup.alpha = 1f;
            persistentInstructionCanvasGroup.interactable = false;
            persistentInstructionCanvasGroup.blocksRaycasts = false;
        }

        Debug.Log(
            "ROOM 2 INSTRUCTION RESET COMPLETE."
        );
    }

    // =========================================================
    // PERSISTENT TEXT FADE
    // =========================================================

    IEnumerator FadePersistentInstruction(float targetAlpha)
    {
        if (persistentInstructionCanvasGroup == null)
            yield break;

        float startAlpha =
            persistentInstructionCanvasGroup.alpha;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float t =
                elapsed / fadeDuration;

            persistentInstructionCanvasGroup.alpha =
                Mathf.Lerp(
                    startAlpha,
                    targetAlpha,
                    t
                );

            yield return null;
        }

        persistentInstructionCanvasGroup.alpha =
            targetAlpha;
    }

    // =========================================================
    // HIDE LARGE INSTRUCTION PANEL
    // =========================================================

    public void HideInstruction()
    {
        FadeTo(0f);
    }

    // =========================================================
    // AFTER TABLE 1
    // =========================================================

    public void ShowChallengeCleared()
    {
        if (instructionText != null)
        {
            instructionText.text =
                "CHALLENGE CLEARED — PROCEED TO TABLE 2";
        }

        FadeTo(1f);
    }

    // =========================================================
    // AFTER TABLE 2
    // =========================================================

    public void ShowNextTableInstruction()
    {
        if (instructionText != null)
        {
            instructionText.text =
                "NEXT CHALLENGE AWAITS — FIND THE TABLE";
        }

        FadeTo(1f);
    }

    // =========================================================
    // TABLE 3
    // =========================================================

    public void ShowFinalChallengeInstruction()
    {
        if (instructionText != null)
        {
            instructionText.text =
                "FINAL CHALLENGE AWAITS — PROCEED TO THE LAST TABLE";
        }

        FadeTo(1f);
    }

    // =========================================================
    // GENERAL FADE
    // =========================================================

    void FadeTo(float targetAlpha)
    {
        if (instructionCanvasGroup == null)
            return;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine =
            StartCoroutine(
                FadeCoroutine(targetAlpha)
            );
    }

    // =========================================================
    // GENERAL FADE COROUTINE
    // =========================================================

    IEnumerator FadeCoroutine(float targetAlpha)
    {
        if (instructionCanvasGroup == null)
            yield break;

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