
using UnityEngine;
using System.Collections;
using TMPro;

public class Room2InstructionManager : MonoBehaviour
{
    [Header("Instruction Panel")]
    public CanvasGroup instructionCanvasGroup;
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

    // Text belonging to the persistent top instruction.
    private TMP_Text persistentInstructionText;

    private const string InitialInstruction =
        "CHALLENGE 1 AWAITS — PROCEED TO TABLE 1";

    private void Awake()
    {
        CachePersistentInstructionText();
    }

    private void Start()
    {
        CachePersistentInstructionText();

        if (persistentInstructionText != null)
        {
            persistentInstructionText.text = InitialInstruction;
        }

        HideLargeInstructionImmediately();
        HidePersistentInstructionImmediately();
    }

    private void CachePersistentInstructionText()
    {
        if (persistentInstructionCanvasGroup == null)
        {
            persistentInstructionText = null;
            return;
        }

        persistentInstructionText =
            persistentInstructionCanvasGroup.GetComponentInChildren<TMP_Text>(true);
    }

    // =========================================================
    // SHOW INITIAL LARGE INSTRUCTION PANEL
    // Used by the initial instruction and Try Again / Restart.
    // =========================================================

    public void ShowInstruction()
    {
        if (instructionPanelRoot != null &&
            !instructionPanelRoot.activeSelf)
        {
            instructionPanelRoot.SetActive(true);
        }

        if (instructionCanvasGroup != null &&
            !instructionCanvasGroup.gameObject.activeSelf)
        {
            instructionCanvasGroup.gameObject.SetActive(true);
        }

        if (instructionText != null)
        {
            instructionText.text = InitialInstruction;
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

        HideInstruction();
        ShowPersistentInstruction();

        if (controlsPanel != null)
        {
            controlsPanel.SetActive(true);
            Debug.Log("ROOM 2 CONTROLS SHOWN.");
        }

        if (centerDot != null)
        {
            centerDot.SetActive(true);
            Debug.Log("ROOM 2 CENTER DOT SHOWN.");
        }

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

        if (!persistentInstructionCanvasGroup.gameObject.activeSelf)
        {
            persistentInstructionCanvasGroup.gameObject.SetActive(true);
        }

        if (persistentFadeCoroutine != null)
        {
            StopCoroutine(persistentFadeCoroutine);
            persistentFadeCoroutine = null;
        }

        persistentInstructionCanvasGroup.interactable = false;
        persistentInstructionCanvasGroup.blocksRaycasts = false;

        persistentFadeCoroutine =
            StartCoroutine(FadePersistentInstruction(1f));
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
            StartCoroutine(FadePersistentInstruction(0f));
    }

    // =========================================================
    // RESET ROOM 2 INSTRUCTION FOR TRY AGAIN / RESTART
    // =========================================================

    public void ResetForTryAgain()
    {
        Debug.Log("RESETTING ROOM 2 INSTRUCTIONS...");

        StopInstructionFades();
        CachePersistentInstructionText();

        if (instructionText != null)
        {
            instructionText.text = InitialInstruction;
        }

        if (persistentInstructionText != null)
        {
            persistentInstructionText.text = InitialInstruction;
        }

        HidePersistentInstructionImmediately();

        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }

        if (centerDot != null)
        {
            centerDot.SetActive(false);
        }

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

            instructionCanvasGroup.alpha = 0f;
            instructionCanvasGroup.interactable = true;
            instructionCanvasGroup.blocksRaycasts = true;
        }

        ShowInstruction();

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
    // UPDATE TOP INSTRUCTION WITHOUT SHOWING LARGE PANEL
    // =========================================================

    private void ShowPersistentStatusOnly(string message)
    {
        StopInstructionFades();
        CachePersistentInstructionText();

        if (persistentInstructionText == null)
        {
            Debug.LogWarning(
                "Persistent instruction TMP_Text was not found. " +
                "Check the Persistent Instruction CanvasGroup assignment.",
                this
            );
            return;
        }

        // Update the TMP text under the persistent top instruction.
        persistentInstructionText.text = message;

        // Keep the large challenge panel hidden.
        if (instructionPanelRoot != null)
        {
            instructionPanelRoot.SetActive(false);
        }

        if (instructionCanvasGroup != null)
        {
            instructionCanvasGroup.alpha = 0f;
            instructionCanvasGroup.interactable = false;
            instructionCanvasGroup.blocksRaycasts = false;
        }

        // Fade the top instruction back in.
        if (persistentInstructionCanvasGroup != null)
        {
            if (!persistentInstructionCanvasGroup.gameObject.activeSelf)
            {
                persistentInstructionCanvasGroup.gameObject.SetActive(true);
            }

            persistentInstructionCanvasGroup.alpha = 0f;
            persistentInstructionCanvasGroup.interactable = false;
            persistentInstructionCanvasGroup.blocksRaycasts = false;

            ShowPersistentInstruction();
        }
        else
        {
            Debug.LogWarning(
                "Persistent Instruction CanvasGroup is not assigned!",
                this
            );
        }
    }

    // =========================================================
    // AFTER TABLE 1 — UPDATE TOP TEXT ONLY
    // =========================================================

    public void ShowChallengeCleared()
    {
        ShowPersistentStatusOnly(
            "CHALLENGE CLEARED — PROCEED TO TABLE 2"
        );
    }

    // =========================================================
    // NEXT TABLE — UPDATE TOP TEXT ONLY
    // =========================================================

    public void ShowNextTableInstruction()
    {
        ShowPersistentStatusOnly(
            "NEXT CHALLENGE AWAITS — FIND THE TABLE"
        );
    }

    // =========================================================
    // TABLE 3 — UPDATE TOP TEXT ONLY
    // =========================================================

    public void ShowFinalChallengeInstruction()
    {
        ShowPersistentStatusOnly(
            "FINAL CHALLENGE AWAITS — PROCEED TO THE LAST TABLE"
        );
    }

    // =========================================================
    // GENERAL LARGE-PANEL FADE
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

        fadeCoroutine = StartCoroutine(FadeCoroutine(targetAlpha));
    }

    private IEnumerator FadeCoroutine(float targetAlpha)
    {
        if (instructionCanvasGroup == null)
            yield break;

        float startAlpha = instructionCanvasGroup.alpha;
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

    // =========================================================
    // PERSISTENT TOP-TEXT FADE
    // =========================================================

    private IEnumerator FadePersistentInstruction(float targetAlpha)
    {
        if (persistentInstructionCanvasGroup == null)
            yield break;

        float startAlpha = persistentInstructionCanvasGroup.alpha;
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

        instructionCanvasGroup.interactable = false;
        instructionCanvasGroup.blocksRaycasts = false;

        FadeTo(0f);
    }
}
