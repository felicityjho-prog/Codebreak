using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CompletionCinematicController : MonoBehaviour
{
    [Header("Cinematic Background")]
    public Image backgroundImage;
    public RectTransform backgroundRect;

    [Header("Completion Overlay")]
    public GameObject completionOverlay;

    [Header("Staggered Reveal — In Display Order")]
    [Tooltip("Drag the title, subtitles, message, checklist and buttons here in order.")]
    public GameObject[] revealElements;

    [Tooltip("Drag the ending button GameObjects here so they become clickable.")]
    public GameObject[] interactiveElements;

    [Tooltip("Usually the CompletionTitle. The AI voice starts when this element appears.")]
    public GameObject voiceStartElement;

    [Header("Completion Music")]
    [Tooltip("AudioSource assigned to the ending music.")]
    public AudioSource completionMusic;

    // The music begins as soon as the completion panel is activated.
    // This gives the cinematic background animation time to act as the music intro.

    [Header("Completion Sound Effect")]
    [Tooltip("Drag the CompletedGAME SOUND audio file here. It plays once when the game-completion panel activates after the correct final answer.")]
    public AudioClip completionSoundEffectClip;

    [Range(0f, 1f)]
    public float completionSoundEffectVolume = 1f;

    [Header("Ending AI Voice")]
    [Tooltip("Ending narration that starts with the selected title element.")]
    public AudioSource endingVoice;

    [Header("Background Animation")]
    [Min(0.1f)]
    public float backgroundFadeDuration = 1.2f;

    [Min(0f)]
    public float cinematicHoldDuration = 0f;

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

    private bool completionSoundEffectPlayed;
    private AudioSource completionSfxSource;

    private void Awake()
    {
        FindReferences();
        CacheOriginalScale();
        PrepareEndingVoice();
        PrepareCompletionMusic();
        PrepareCompletionSoundEffect();
    }

    private void OnEnable()
    {
        FindReferences();
        CacheOriginalScale();
        PrepareEndingVoice();
        PrepareCompletionMusic();
        PrepareCompletionSoundEffect();
        completionSoundEffectPlayed = false;

        // Play the success sound exactly once when CompleteGame activates this panel.
        // Do not play it again during the title/voice reveal.
        PlayCompletionSoundEffect();

        if (cinematicRoutine != null)
        {
            StopCoroutine(cinematicRoutine);
            cinematicRoutine = null;
        }

        cinematicRoutine = StartCoroutine(PlayCinematic());
    }

    private void OnDisable()
    {
        if (cinematicRoutine != null)
        {
            StopCoroutine(cinematicRoutine);
            cinematicRoutine = null;
        }

        if (endingVoice != null && endingVoice.isPlaying)
            endingVoice.Stop();

        if (completionMusic != null && completionMusic.isPlaying)
            completionMusic.Stop();

        if (completionSfxSource != null && completionSfxSource.isPlaying)
            completionSfxSource.Stop();

        completionSoundEffectPlayed = false;

        if (backgroundRect != null && originalScaleCached)
            backgroundRect.localScale = originalBackgroundScale;
    }

    private void PrepareEndingVoice()
    {
        if (endingVoice == null)
            return;

        endingVoice.playOnAwake = false;
        endingVoice.loop = false;

        if (endingVoice.isPlaying)
            endingVoice.Stop();
    }

    private void PrepareCompletionMusic()
    {
        if (completionMusic == null)
            return;

        completionMusic.playOnAwake = false;

        if (completionMusic.isPlaying)
            completionMusic.Stop();

        // The Loop setting remains controlled by the AudioSource Inspector.
    }

    private void PrepareCompletionSoundEffect()
    {
        // Keep the success sound on its own AudioSource so it cannot
        // interrupt the completion music or the AI narration.
        Transform sourceTransform = transform.Find("CompletionSoundEffectSource");

        if (sourceTransform == null)
        {
            GameObject sourceObject = new GameObject("CompletionSoundEffectSource");
            sourceObject.transform.SetParent(transform, false);
            sourceTransform = sourceObject.transform;
        }

        completionSfxSource = sourceTransform.GetComponent<AudioSource>();

        if (completionSfxSource == null)
            completionSfxSource = sourceTransform.gameObject.AddComponent<AudioSource>();

        completionSfxSource.playOnAwake = false;
        completionSfxSource.loop = false;
        completionSfxSource.spatialBlend = 0f; // UI sound: 2D audio.

        if (completionSfxSource.isPlaying)
            completionSfxSource.Stop();

        // If an old AudioSource still has this SFX clip assigned, prevent it
        // from playing on its own. The dedicated one-shot source below is
        // now the only source that plays this clip.
        if (completionSoundEffectClip != null)
        {
            AudioSource[] existingSources =
                GetComponentsInChildren<AudioSource>(true);

            foreach (AudioSource source in existingSources)
            {
                if (source == null ||
                    source == completionSfxSource ||
                    source.clip != completionSoundEffectClip)
                {
                    continue;
                }

                source.playOnAwake = false;

                if (source.isPlaying)
                    source.Stop();
            }
        }
    }

    private void FindReferences()
    {
        if (backgroundImage == null)
        {
            Transform bg = transform.Find("CompletionCinematicBackground");

            if (bg != null)
                backgroundImage = bg.GetComponent<Image>();
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
            Transform overlay = FindChildRecursive(transform, "GameCompletionOverlay");

            if (overlay != null)
                completionOverlay = overlay.gameObject;
        }

        if (completionOverlay != null)
        {
            overlayGroup = GetOrAddCanvasGroup(completionOverlay);

            if (voiceStartElement == null)
            {
                Transform title =
                    FindChildRecursive(completionOverlay.transform, "CompletionTitle");

                if (title != null)
                    voiceStartElement = title.gameObject;
            }
        }

        PrepareRevealElements();
    }

    private Transform FindChildRecursive(Transform parent, string targetName)
    {
        if (parent == null)
            return null;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            if (child.name == targetName)
                return child;

            Transform nested = FindChildRecursive(child, targetName);

            if (nested != null)
                return nested;
        }

        return null;
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

            elementGroups[i] =
                GetOrAddCanvasGroup(revealElements[i]);
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

            group.alpha = Mathf.Lerp(startAlpha, endAlpha, smooth);

            yield return null;
        }

        group.alpha = endAlpha;
    }

    private void StartCompletionMusic()
    {
        if (completionMusic != null && completionMusic == endingVoice)
        {
            Debug.LogError(
                "Completion Cinematic: Completion Music and Ending AI Voice " +
                "must use separate AudioSource components.",
                this
            );
            return;
        }

        if (completionMusic != null && completionMusic.clip != null &&
            endingVoice != null && endingVoice.clip == completionMusic.clip)
        {
            Debug.LogError(
                "Completion Cinematic: Completion Music and Ending AI Voice " +
                "are assigned the same clip. Check their AudioSource clips.",
                this
            );
            return;
        }

        if (completionMusic == null || completionMusic.clip == null)
        {
            Debug.LogWarning(
                "Completion Cinematic: Completion Music AudioSource " +
                "or AudioClip is not assigned.",
                this
            );

            return;
        }

        completionMusic.Stop();
        completionMusic.playOnAwake = false;
        completionMusic.Play();

        Debug.Log(
            "Completion Cinematic: Completion Music started.",
            this
        );
    }

    private void PlayCompletionSoundEffect()
    {
        // This method can be reached through the normal title trigger and
        // the fallback path, so guard against double playback.
        if (completionSoundEffectPlayed)
            return;

        if (completionSoundEffectClip == null)
        {
            Debug.LogWarning(
                "Completion Cinematic: Assign the CompletedGAME SOUND file " +
                "to Completion Sound Effect Clip in the Inspector.",
                this
            );
            return;
        }

        if (completionSfxSource == null)
            PrepareCompletionSoundEffect();

        completionSoundEffectPlayed = true;
        completionSfxSource.Stop();
        completionSfxSource.PlayOneShot(
            completionSoundEffectClip,
            completionSoundEffectVolume
        );

        Debug.Log(
            "Completion Cinematic: CompletedGAME SOUND played once when the completion panel activated.",
            this
        );
    }

    private void StartEndingVoice()
    {
        if (endingVoice != null && endingVoice == completionMusic)
        {
            Debug.LogError(
                "Completion Cinematic: Ending AI Voice and Completion Music " +
                "must use separate AudioSource components.",
                this
            );
            return;
        }

        if (endingVoice == null || endingVoice.clip == null)
        {
            Debug.LogWarning(
                "Completion Cinematic: Ending Voice AudioSource " +
                "or AudioClip is not assigned.",
                this
            );

            return;
        }

        endingVoice.Stop();
        endingVoice.playOnAwake = false;
        endingVoice.loop = false;
        endingVoice.Play();

        Debug.Log(
            "Completion Cinematic: Ending AI Voice started.",
            this
        );
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

        // Start music immediately when the completion panel activates.
        // The background fade/hold provides the musical intro before the title/voice.
        StartCompletionMusic();

        // RESET: background.
        backgroundGroup.alpha = 0f;
        backgroundGroup.interactable = false;
        backgroundGroup.blocksRaycasts = false;
        backgroundRect.localScale = originalBackgroundScale;

        // RESET: overlay.
        overlayGroup.alpha = 0f;
        overlayGroup.interactable = false;
        overlayGroup.blocksRaycasts = true;

        // RESET: text, checklist and buttons.
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

            float progress =
                Mathf.Clamp01(elapsed / backgroundFadeDuration);

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

        // PHASE 2 — Cinematic hold.
        if (cinematicHoldDuration > 0f)
        {
            yield return new WaitForSecondsRealtime(
                cinematicHoldDuration
            );
        }

        // PHASE 3 — Reveal the completion overlay.
        yield return StartCoroutine(
            FadeGroup(
                overlayGroup,
                overlayFadeDuration,
                0f,
                1f
            )
        );

        overlayGroup.interactable = false;
        overlayGroup.blocksRaycasts = true;

        // PHASE 4 — Reveal each element in sequence.
        bool endingVoiceStarted = false;

        int revealCount =
            revealElements != null ? revealElements.Length : 0;

        for (int i = 0; i < revealCount; i++)
        {
            GameObject element = revealElements[i];
            CanvasGroup group = elementGroups[i];

            if (element == null || group == null)
                continue;

            bool shouldStartVoice =
                element == voiceStartElement ||
                (voiceStartElement == null && i == 0);

            if (!endingVoiceStarted && shouldStartVoice)
            {
                // Start the AI voice as the title begins fading in.
                // Completion SFX has already played once on panel activation.
                StartEndingVoice();
                endingVoiceStarted = true;
            }

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

        // Fallback if Voice Start Element is not in Reveal Elements.
        if (!endingVoiceStarted)
        {
            Debug.LogWarning(
                "Completion Cinematic: Voice Start Element was not " +
                "found in Reveal Elements. Starting the voice after " +
                "the reveal as a fallback.",
                this
            );

            StartEndingVoice();
            endingVoiceStarted = true;
        }

        // Wait for the narration to finish before enabling the buttons.
        if (endingVoiceStarted &&
            endingVoice != null &&
            endingVoice.clip != null)
        {
            while (endingVoice.isPlaying)
            {
                yield return null;
            }
        }

        // PHASE 5 — Enable the ending buttons.
        for (int i = 0; i < revealCount; i++)
        {
            if (revealElements[i] == null ||
                elementGroups[i] == null)
            {
                continue;
            }

            bool canInteract =
                IsInteractiveElement(revealElements[i]);

            elementGroups[i].interactable = canInteract;
            elementGroups[i].blocksRaycasts = canInteract;
        }

        overlayGroup.alpha = 1f;
        overlayGroup.interactable = true;
        overlayGroup.blocksRaycasts = true;

        cinematicRoutine = null;

        Debug.Log(
            "Completion cinematic finished. " +
            "Ending buttons are now enabled.",
            this
        );
    }
}