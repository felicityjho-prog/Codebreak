using System.Collections;
using UnityEngine;

public class CompletionCinematicController : MonoBehaviour
{
    [Header("Cinematic Background")]
    public UnityEngine.UI.Image backgroundImage;
    public RectTransform backgroundRect;

    [Header("Completion Overlay")]
    public GameObject completionOverlay;

    [Header("Staggered Reveal — In Display Order")]
    [Tooltip("Drag the title, subtitles, message, checklist and buttons here in order.")]
    public GameObject[] revealElements;

    [Tooltip("Drag the four button GameObjects here so they become clickable.")]
    public GameObject[] interactiveElements;

    [Header("Background Animation")]
    [Min(0.1f)]
    public float backgroundFadeDuration = 1.2f;

    [Min(0f)]
    public float cinematicHoldDuration = 1.3f;

    [Range(1f, 1.15f)]
    public float zoomAmount = 1.06f;

    [Header("Overlay Animation")]
    [Min(0.1f)]
    public float overlayFadeDuration = 0.45f;

    [Min(0f)]
    public float elementDelay = 0.18f;

    private CanvasGroup backgroundGroup;
    private CanvasGroup overlayGroup;

    private CanvasGroup[] elementGroups;
    private Vector3 originalBackgroundScale;

    private bool originalScaleCached;
    private Coroutine cinematicRoutine;

    private void Awake()
    {
        FindReferences();
        CacheOriginalScale();
    }

    private void OnEnable()
    {
        FindReferences();
        CacheOriginalScale();

        if (cinematicRoutine != null)
            StopCoroutine(cinematicRoutine);

        cinematicRoutine = StartCoroutine(PlayCinematic());
    }

    private void OnDisable()
    {
        if (cinematicRoutine != null)
        {
            StopCoroutine(cinematicRoutine);
            cinematicRoutine = null;
        }

        if (backgroundRect != null && originalScaleCached)
            backgroundRect.localScale = originalBackgroundScale;
    }

    private void FindReferences()
    {
        if (backgroundImage == null)
        {
            Transform bg = transform.Find(
                "CompletionCinematicBackground"
            );

            if (bg != null)
                backgroundImage = bg.GetComponent<UnityEngine.UI.Image>();
        }

        if (backgroundImage != null)
        {
            if (backgroundRect == null)
                backgroundRect = backgroundImage.rectTransform;

            backgroundGroup = GetOrAddCanvasGroup(
                backgroundImage.gameObject
            );
        }

        if (completionOverlay == null)
        {
            Transform overlay = transform.Find(
                "GameCompletionOverlay"
            );

            if (overlay != null)
                completionOverlay = overlay.gameObject;
        }

        if (completionOverlay != null)
        {
            overlayGroup = GetOrAddCanvasGroup(
                completionOverlay
            );
        }

        PrepareRevealElements();
    }

    private CanvasGroup GetOrAddCanvasGroup(GameObject target)
    {
        if (target == null)
            return null;

        CanvasGroup group = target.GetComponent<CanvasGroup>();

        if (group == null)
            group = target.AddComponent<CanvasGroup>();

        return group;
    }

    private void CacheOriginalScale()
    {
        if (backgroundRect == null || originalScaleCached)
            return;

        originalBackgroundScale = backgroundRect.localScale;
        originalScaleCached = true;
    }

    private void PrepareRevealElements()
    {
        if (revealElements == null)
        {
            elementGroups = new CanvasGroup[0];
            return;
        }

        elementGroups = new CanvasGroup[revealElements.Length];

        for (int i = 0; i < revealElements.Length; i++)
        {
            if (revealElements[i] == null)
                continue;

            elementGroups[i] = GetOrAddCanvasGroup(
                revealElements[i]
            );
        }
    }

    private bool IsInteractiveElement(GameObject target)
    {
        if (target == null || interactiveElements == null)
            return false;

        foreach (GameObject item in interactiveElements)
        {
            if (item == target)
                return true;
        }

        return false;
    }

    private IEnumerator FadeGroup(
        CanvasGroup group,
        float duration,
        float startAlpha,
        float endAlpha)
    {
        if (group == null)
            yield break;

        group.alpha = startAlpha;

        if (duration <= 0f)
        {
            group.alpha = endAlpha;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);
            float smooth = Mathf.SmoothStep(0f, 1f, progress);

            group.alpha = Mathf.Lerp(
                startAlpha,
                endAlpha,
                smooth
            );

            yield return null;
        }

        group.alpha = endAlpha;
    }

    private IEnumerator PlayCinematic()
    {
        if (backgroundImage == null ||
            backgroundRect == null ||
            backgroundGroup == null ||
            completionOverlay == null ||
            overlayGroup == null)
        {
            Debug.LogError(
                "Completion Cinematic: Assign Background Image, " +
                "Background Rect, and Completion Overlay.",
                this
            );

            cinematicRoutine = null;
            yield break;
        }

        // Reset the background.
        backgroundGroup.alpha = 0f;
        backgroundGroup.interactable = false;
        backgroundGroup.blocksRaycasts = false;

        backgroundRect.localScale = originalBackgroundScale;

        // Hide the overlay before revealing its individual elements.
        overlayGroup.alpha = 0f;
        overlayGroup.interactable = false;
        overlayGroup.blocksRaycasts = false;

        // Hide each text, checklist and button before the animation.
        PrepareRevealElements();

        foreach (CanvasGroup group in elementGroups)
        {
            if (group == null)
                continue;

            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;
        }

        // PHASE 1 — Background fade and slow zoom.
        float elapsed = 0f;

        while (elapsed < backgroundFadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(
                elapsed / backgroundFadeDuration
            );

            float smooth = Mathf.SmoothStep(0f, 1f, progress);

            backgroundGroup.alpha = smooth;

            backgroundRect.localScale =
                originalBackgroundScale *
                Mathf.Lerp(1f, zoomAmount, smooth);

            yield return null;
        }

        backgroundGroup.alpha = 1f;

        backgroundRect.localScale =
            originalBackgroundScale * zoomAmount;

        // PHASE 2 — Pause for cinematic effect.
        if (cinematicHoldDuration > 0f)
        {
            yield return new WaitForSecondsRealtime(
                cinematicHoldDuration
            );
        }

        // PHASE 3 — Reveal the overlay background.
        yield return StartCoroutine(
            FadeGroup(
                overlayGroup,
                overlayFadeDuration,
                0f,
                1f
            )
        );

        // Keep all interactive elements disabled during the reveal.
        overlayGroup.interactable = false;
        overlayGroup.blocksRaycasts = false;

        // PHASE 4 — Reveal each assigned UI element in sequence.
        foreach (CanvasGroup group in elementGroups)
        {
            if (group == null)
                continue;

            yield return StartCoroutine(
                FadeGroup(
                    group,
                    overlayFadeDuration,
                    0f,
                    1f
                )
            );

            if (elementDelay > 0f)
            {
                yield return new WaitForSecondsRealtime(
                    elementDelay
                );
            }
        }

        // Enable the four buttons after the complete reveal.
        for (int i = 0; i < revealElements.Length; i++)
        {
            if (revealElements[i] == null ||
                elementGroups[i] == null)
            {
                continue;
            }

            bool canInteract = IsInteractiveElement(
                revealElements[i]
            );

            elementGroups[i].interactable = canInteract;
            elementGroups[i].blocksRaycasts = canInteract;
        }

        overlayGroup.alpha = 1f;
        overlayGroup.interactable = true;
        overlayGroup.blocksRaycasts = true;

        cinematicRoutine = null;
    }
}