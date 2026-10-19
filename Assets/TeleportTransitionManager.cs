
using System;
using System.Collections;
using UnityEngine;

public class TeleportTransitionManager : MonoBehaviour
{
    [Header("Fade Overlay")]
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField, Min(0f)] private float fadeDuration = 0.6f;

    [Header("Player Controls")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerController playerController;

    private bool isTransitioning;
    private bool movementWasEnabled;

    public bool IsTransitioning => isTransitioning;

    // =====================================================
    // RUN TRANSITION
    // =====================================================

    public void RunTransition(
        Action teleportAction,
        Action afterFadeIn = null)
    {
        if (isTransitioning)
        {
            Debug.LogWarning(
                "Teleport transition is already running."
            );
            return;
        }

        if (fadeGroup == null)
        {
            Debug.LogError(
                "TeleportTransitionManager: Fade Group is missing."
            );
            return;
        }

        StartCoroutine(
            TransitionRoutine(teleportAction, afterFadeIn)
        );
    }

    // =====================================================
    // TRANSITION ROUTINE
    // =====================================================

    private IEnumerator TransitionRoutine(
        Action teleportAction,
        Action afterFadeIn)
    {
        isTransitioning = true;

        // Remember the original movement state.
        movementWasEnabled =
            playerMovement != null &&
            playerMovement.enabled;

        // Lock player controls during the transition.
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        fadeGroup.blocksRaycasts = true;
        fadeGroup.interactable = false;

        // Fade out to black.
        yield return Fade(0f, 1f);

        // Switch rooms and teleport while the screen is black.
        if (teleportAction != null)
        {
            teleportAction.Invoke();
        }

        // Allow the destination room to update.
        yield return null;

        // Fade in to reveal the destination.
        yield return Fade(1f, 0f);

        fadeGroup.blocksRaycasts = false;

        // Restore the original movement state.
        if (playerMovement != null)
        {
            playerMovement.enabled = movementWasEnabled;
        }

        // Always re-enable the controller component.
        // Camera look can still be disabled separately
        // through PlayerController.DisableLook().
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        isTransitioning = false;

        // Run actions after the fade-in.
        // Room 2 Voice AI starts here.
        if (afterFadeIn != null)
        {
            afterFadeIn.Invoke();
        }

        Debug.Log(
            "Teleport transition completed successfully."
        );
    }

    // =====================================================
    // FADE ANIMATION
    // =====================================================

    private IEnumerator Fade(float from, float to)
    {
        if (fadeDuration <= 0f)
        {
            fadeGroup.alpha = to;
            yield break;
        }

        float elapsed = 0f;

        fadeGroup.alpha = from;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(
                elapsed / fadeDuration
            );

            fadeGroup.alpha = Mathf.Lerp(
                from,
                to,
                progress
            );

            yield return null;
        }

        fadeGroup.alpha = to;
    }

    // =====================================================
    // INITIALIZE
    // =====================================================

    private void Awake()
    {
        if (fadeGroup != null)
        {
            fadeGroup.alpha = 0f;
            fadeGroup.blocksRaycasts = false;
            fadeGroup.interactable = false;
        }
        else
        {
            Debug.LogError(
                "TeleportTransitionManager: " +
                "Please assign the Fade Group in the Inspector."
            );
        }
    }
}
