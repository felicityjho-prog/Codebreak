
using UnityEngine;
using System.Collections;
using TMPro;

public class Room2InstructionManager : MonoBehaviour
{
    [Header("Instruction Panel")]
    public CanvasGroup instructionCanvasGroup;

    [Tooltip("Root GameObject of the large Room 2 challenge panel.")]
    public GameObject instructionPanelRoot;

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

    private void Start()
    {
        HideLargeInstructionImmediately();
        HidePersistentInstructionImmediately();
    }

    // =========================================================
    // SHOW INITIAL INSTRUCTION
    // =========================================================

    public void ShowInstruction()
    {
        // Make sure the panel root is active.
        if (instructionPanelRoot != null &&
            !instructionPanelRoot.activeSelf)
        {
            instructionPanelRoot.SetActive(true);
        }

        // Also ensure the CanvasGroup object is active.
        if (instructionCanvasGroup != null &&
            !instructionCanvasGroup.gameObject.activeSelf)
        {
            instructionCanvasGroup.gameObject.SetActive(true);
        }

        if (instructionText != null)
        {
            instructionText.text =
                "CHALLENGE 1 AWAITS — PROCEED TO TABLE 1";
        }

        if (instructionCanvasGroup != null)
        {
            instructionCanvasGroup.interactable = true;
            instructionCanvasGroup.blocksRaycasts = true;
        }

        FadeTo(1f);
    }

    // =========================================================
    // GOT IT
    // =========================================================

    public void OnGotItClicked()
    {
        Debug.Log("ROOM 2 INSTRUCTION CONFIRMED.");

        // Hide the large instruction panel.
        HideInstruction();

        // Show the persistent top instruction.
        ShowPersistentInstruction();

        // Restore controls.
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(true);
            Debug.Log("ROOM 2 CONTROLS SHOWN.");
        }

        // Restore the center dot.
        if (centerDot != null)
        {
            centerDot.SetActive(true);
            Debug.Log("ROOM 2 CENTER DOT SHOWN.");
        }

        // Start the Room 2 timer only after GOT IT.
        if (Room2ChallengeTimer.Instance != null)
        {
            Room2ChallengeTimer.Instance.StartTimer();
            Debug.Log("ROOM 2 TIMER STARTED AFTER GOT IT.");
        }
        else
        {
            Debug.LogWarning(
                "Room2ChallengeTimer Instance NOT FOUND!"
            );
        }

        // Lock the cursor for gameplay.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
            persistentFadeCoroutine = null;
        }

        persistentInstructionCanvasGroup.interactable = false;
        persistentInstructionCanvasGroup.blocksRaycasts = false;

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
            persistentFadeCoroutine = null;
        }

        persistentFadeCoroutine =
            StartCoroutine(
                FadePersistentInstruction(0f)
            );
    }

    // =========================================================
    // RESET ROOM 2 INSTRUCTION FOR TRY AGAIN / RESTART
    // =========================================================

    public void ResetForTryAgain()
    {
        Debug.Log("RESETTING ROOM 2 INSTRUCTIONS...");

        StopInstructionFades();

        // Restore the initial challenge instruction text.
        if (instructionText != null)
        {
            instructionText.text =
                "CHALLENGE 1 AWAITS — PROCEED TO TABLE 1";
        }

        // Hide the persistent top instruction until GOT IT.
        HidePersistentInstructionImmediately();

        // Hide controls and center dot until GOT IT is clicked.
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }

        if (centerDot != null)
        {
            centerDot.SetActive(false);
        }

        // Restore and show the large challenge panel.
        if (instructionPanelRoot != null)
        {
            instructionPanelRoot.SetActive(true);
        }

        if (instructionCanvasGroup != null)
        {
            if (!instructionCanvasGroup.gameObject.activeSelf)
            {
                instructionCanvasGroup.gameObject.SetActive(true);
            }

            // Start the fade from transparent.
            instructionCanvasGroup.alpha = 0f;
            instructionCanvasGroup.interactable = true;
            instructionCanvasGroup.blocksRaycasts = true;
        }

        ShowInstruction();

        // Allow the user to click GOT IT.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("ROOM 2 INSTRUCTION PANEL RESTORED.");
    }

    // =========================================================
    // STOP ACTIVE FADE COROUTINES
    // =========================================================

    private void StopInstructionFades()
    {
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
    }

    // =========================================================
    // HIDE LARGE INSTRUCTION IMMEDIATELY
    // =========================================================

    private void HideLargeInstructionImmediately()
    {
        if (instructionCanvasGroup == null)
            return;

        instructionCanvasGroup.alpha = 0f;
        instructionCanvasGroup.interactable = false;
        instructionCanvasGroup.blocksRaycasts = false;
    }

    // =========================================================
    // HIDE PERSISTENT INSTRUCTION IMMEDIATELY
    // =========================================================

    private void HidePersistentInstructionImmediately()
    {
        if (persistentInstructionCanvasGroup == null)
            return;

        persistentInstructionCanvasGroup.alpha = 0f;
        persistentInstructionCanvasGroup.interactable = false;
        persistentInstructionCanvasGroup.blocksRaycasts = false;
    }

    // =========================================================
    // PERSISTENT TEXT FADE
    // =========================================================

    private IEnumerator FadePersistentInstruction(float targetAlpha)
    {
        if (persistentInstructionCanvasGroup == null)
            yield break;

        float startAlpha =
            persistentInstructionCanvasGroup.alpha;

        float duration = Mathf.Max(0.01f, fadeDuration);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            persistentInstructionCanvasGroup.alpha =
                Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null;
        }

        persistentInstructionCanvasGroup.alpha = targetAlpha;
        persistentFadeCoroutine = null;
    }

    // =========================================================
    // HIDE LARGE INSTRUCTION PANEL
    // =========================================================

    public void HideInstruction()
    {
        if (instructionCanvasGroup == null)
            return;

        // Prevent clicks through the fading panel.
        instructionCanvasGroup.interactable = false;
        instructionCanvasGroup.blocksRaycasts = false;

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

        ShowInstruction();
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

        ShowInstruction();
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

        ShowInstruction();
    }

    // =========================================================
    // GENERAL FADE
    // =========================================================

    private void FadeTo(float targetAlpha)
    {
        if (instructionCanvasGroup == null)
            return;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        fadeCoroutine =
            StartCoroutine(
                FadeCoroutine(targetAlpha)
            );
    }

    // =========================================================
    // GENERAL FADE COROUTINE
    // =========================================================

    private IEnumerator FadeCoroutine(float targetAlpha)
    {
        if (instructionCanvasGroup == null)
            yield break;

        float startAlpha =
            instructionCanvasGroup.alpha;

        float duration = Mathf.Max(0.01f, fadeDuration);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            instructionCanvasGroup.alpha =
                Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null;
        }

        instructionCanvasGroup.alpha = targetAlpha;
        fadeCoroutine = null;
    }
}
