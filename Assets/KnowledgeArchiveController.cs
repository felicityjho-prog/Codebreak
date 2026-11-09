using UnityEngine;
using System.Collections;
using TMPro;

public class KnowledgeArchiveController : MonoBehaviour
{
    [Header("Archive UI")]
    public GameObject knowledgeArchivePanel;
    public GameObject knowledgeArchiveButton;
    public GameObject centerDot;

    [Header("Player Controls")]
    public PlayerMovement playerMovement;
    public PlayerController playerController;
    public Transform playerCamera;

    [Header("Room 2 Instructions")]
    public Room2InstructionManager instructionManager;

    [Header("Background Dimming")]
    [Tooltip("Drag ArchiveDimOverlay here.")]
    public GameObject archiveDimOverlay;

    [Range(0f, 1f)]
    public float backgroundDimAlpha = 0.45f;

    [Header("Gold Sweep Effect")]
    [Tooltip("Drag ArchiveGoldSweep here.")]
    public GameObject archiveGoldSweep;

    [Min(0.1f)]
    public float goldSweepDuration = 1.25f;

    [Range(0f, 1f)]
    public float goldSweepPeakAlpha = 0.45f;

    public float goldSweepEndX = 700f;

    [Header("Transition Settings")]
    [Min(0.01f)]
    public float fadeInDuration = 0.3f;

    [Min(0.01f)]
    public float fadeOutDuration = 0.25f;

    [Range(0.8f, 1f)]
    public float startingScaleMultiplier = 0.96f;

    [Header("Cinematic Reveal References")]
    public GameObject archiveTitleObject;
    public Transform topicSelectionPanel;
    public AudioSource unlockSound;

    [Header("Title Reveal Settings")]
    [Min(0.01f)]
    public float titleFadeDuration = 0.3f;

    [Min(0f)]
    public float titleHoldDuration = 0.65f;

    [Range(0.7f, 1f)]
    public float titleStartingScaleMultiplier = 0.9f;

    [Header("Topic Button Reveal Settings")]
    [Min(0.01f)]
    public float buttonRevealDuration = 0.18f;

    [Min(0f)]
    public float buttonStaggerDelay = 0.06f;

    [Range(0.7f, 1f)]
    public float buttonStartingScaleMultiplier = 0.85f;

    private CanvasGroup archiveCanvasGroup;
    private RectTransform archiveRectTransform;

    private CanvasGroup archiveDimCanvasGroup;

    private CanvasGroup archiveGoldSweepCanvasGroup;
    private RectTransform archiveGoldSweepRectTransform;
    private Vector2 goldSweepStartAnchoredPosition;

    private TMP_Text archiveTitleText;
    private CanvasGroup archiveTitleCanvasGroup;
    private RectTransform archiveTitleRectTransform;

    private string originalArchiveTitle = "KNOWLEDGE ARCHIVE";
    private Vector3 originalArchiveTitleScale;

    private CanvasGroup[] topicButtonCanvasGroups;
    private RectTransform[] topicButtonRectTransforms;
    private Vector3[] originalTopicButtonScales;

    private Vector3 originalScale;

    private Coroutine archiveTransitionCoroutine;

    private bool archiveOpen = false;
    private bool isTransitioning = false;
    private bool firstArchiveCloseCompleted = false;

    private bool previousMovementState;
    private bool previousLookState;
    private bool previousCenterDotState;

    private bool timerWasRunning;

    private Quaternion previousCameraRotation;

    // ==========================================
    // AWAKE
    // ==========================================

    private void Awake()
    {
        CacheArchiveComponents();
        CacheBackgroundDimOverlay();
        CacheGoldSweepComponents();
        CacheRevealComponents();
    }

    // ==========================================
    // CACHE ARCHIVE COMPONENTS
    // ==========================================

    private void CacheArchiveComponents()
    {
        if (knowledgeArchivePanel == null)
        {
            Debug.LogError(
                "KnowledgeArchivePanel is not assigned!",
                this
            );

            return;
        }

        archiveCanvasGroup =
            knowledgeArchivePanel.GetComponent<CanvasGroup>();

        if (archiveCanvasGroup == null)
        {
            archiveCanvasGroup =
                knowledgeArchivePanel.AddComponent<CanvasGroup>();

            Debug.LogWarning(
                "CanvasGroup was automatically added to KnowledgeArchivePanel.",
                this
            );
        }

        archiveRectTransform =
            knowledgeArchivePanel.GetComponent<RectTransform>();

        if (archiveRectTransform != null)
        {
            originalScale = archiveRectTransform.localScale;
        }
    }

    // ==========================================
    // CACHE BACKGROUND DIM OVERLAY
    // ==========================================

    private void CacheBackgroundDimOverlay()
    {
        if (archiveDimOverlay == null)
            return;

        archiveDimCanvasGroup =
            archiveDimOverlay.GetComponent<CanvasGroup>();

        if (archiveDimCanvasGroup == null)
        {
            archiveDimCanvasGroup =
                archiveDimOverlay.AddComponent<CanvasGroup>();

            Debug.LogWarning(
                "CanvasGroup was automatically added to ArchiveDimOverlay.",
                this
            );
        }

        archiveDimCanvasGroup.alpha = 0f;
        archiveDimCanvasGroup.interactable = false;
        archiveDimCanvasGroup.blocksRaycasts = false;
    }

    // ==========================================
    // CACHE GOLD SWEEP COMPONENTS
    // ==========================================

    private void CacheGoldSweepComponents()
    {
        if (archiveGoldSweep == null)
            return;

        archiveGoldSweepCanvasGroup =
            archiveGoldSweep.GetComponent<CanvasGroup>();

        if (archiveGoldSweepCanvasGroup == null)
        {
            archiveGoldSweepCanvasGroup =
                archiveGoldSweep.AddComponent<CanvasGroup>();

            Debug.LogWarning(
                "CanvasGroup was automatically added to ArchiveGoldSweep.",
                this
            );
        }

        archiveGoldSweepRectTransform =
            archiveGoldSweep.GetComponent<RectTransform>();

        if (archiveGoldSweepRectTransform != null)
        {
            goldSweepStartAnchoredPosition =
                archiveGoldSweepRectTransform.anchoredPosition;
        }

        archiveGoldSweepCanvasGroup.alpha = 0f;
        archiveGoldSweepCanvasGroup.interactable = false;
        archiveGoldSweepCanvasGroup.blocksRaycasts = false;
    }

    // ==========================================
    // CACHE TITLE AND TOPIC BUTTON COMPONENTS
    // ==========================================

    private void CacheRevealComponents()
    {
        if (archiveTitleObject != null)
        {
            archiveTitleText =
                archiveTitleObject.GetComponent<TMP_Text>();

            archiveTitleCanvasGroup =
                archiveTitleObject.GetComponent<CanvasGroup>();

            archiveTitleRectTransform =
                archiveTitleObject.GetComponent<RectTransform>();

            if (archiveTitleCanvasGroup == null)
            {
                archiveTitleCanvasGroup =
                    archiveTitleObject.AddComponent<CanvasGroup>();
            }

            if (archiveTitleText != null)
            {
                originalArchiveTitle = archiveTitleText.text;
            }
            else
            {
                Debug.LogWarning(
                    "ArchiveTitle has no TextMeshPro component.",
                    this
                );
            }

            if (archiveTitleRectTransform != null)
            {
                originalArchiveTitleScale =
                    archiveTitleRectTransform.localScale;
            }
        }

        if (topicSelectionPanel == null)
            return;

        int buttonCount = topicSelectionPanel.childCount;

        topicButtonCanvasGroups = new CanvasGroup[buttonCount];
        topicButtonRectTransforms = new RectTransform[buttonCount];
        originalTopicButtonScales = new Vector3[buttonCount];

        for (int i = 0; i < buttonCount; i++)
        {
            Transform buttonTransform =
                topicSelectionPanel.GetChild(i);

            GameObject buttonObject =
                buttonTransform.gameObject;

            CanvasGroup buttonGroup =
                buttonObject.GetComponent<CanvasGroup>();

            if (buttonGroup == null)
            {
                buttonGroup =
                    buttonObject.AddComponent<CanvasGroup>();
            }

            RectTransform buttonRect =
                buttonObject.GetComponent<RectTransform>();

            topicButtonCanvasGroups[i] = buttonGroup;
            topicButtonRectTransforms[i] = buttonRect;

            originalTopicButtonScales[i] =
                buttonRect != null
                    ? buttonRect.localScale
                    : Vector3.one;
        }
    }

    // ==========================================
    // START
    // ==========================================

    private void Start()
    {
        archiveOpen = false;
        isTransitioning = false;
        firstArchiveCloseCompleted = false;

        if (knowledgeArchivePanel != null)
        {
            ResetArchiveVisuals();
            knowledgeArchivePanel.SetActive(false);
        }

        if (knowledgeArchiveButton != null)
        {
            knowledgeArchiveButton.SetActive(false);
        }

        if (archiveDimOverlay != null)
        {
            archiveDimOverlay.SetActive(false);
        }

        ResetGoldSweepVisuals();

        Debug.Log("NEW KNOWLEDGE ARCHIVE CONTROLLER READY!");
    }

    // ==========================================
    // RESET ARCHIVE VISUALS
    // ==========================================

    private void ResetArchiveVisuals()
    {
        if (archiveCanvasGroup != null)
        {
            archiveCanvasGroup.alpha = 1f;
            archiveCanvasGroup.interactable = true;
            archiveCanvasGroup.blocksRaycasts = true;
        }

        if (archiveRectTransform != null)
        {
            archiveRectTransform.localScale = originalScale;
        }

        if (archiveDimCanvasGroup != null)
        {
            archiveDimCanvasGroup.alpha = 0f;
            archiveDimCanvasGroup.interactable = false;
            archiveDimCanvasGroup.blocksRaycasts = false;
        }

        if (archiveDimOverlay != null)
        {
            archiveDimOverlay.SetActive(false);
        }

        ResetGoldSweepVisuals();
        ResetRevealVisuals();
    }

    // ==========================================
    // RESET GOLD SWEEP VISUALS
    // ==========================================

    private void ResetGoldSweepVisuals()
    {
        if (archiveGoldSweepCanvasGroup != null)
        {
            archiveGoldSweepCanvasGroup.alpha = 0f;
            archiveGoldSweepCanvasGroup.interactable = false;
            archiveGoldSweepCanvasGroup.blocksRaycasts = false;
        }

        if (archiveGoldSweepRectTransform != null)
        {
            archiveGoldSweepRectTransform.anchoredPosition =
                goldSweepStartAnchoredPosition;
        }
    }

    // ==========================================
    // RESET TITLE AND TOPIC BUTTON VISUALS
    // ==========================================

    private void ResetRevealVisuals()
    {
        if (archiveTitleText != null)
        {
            archiveTitleText.text = originalArchiveTitle;
        }

        if (archiveTitleCanvasGroup != null)
        {
            archiveTitleCanvasGroup.alpha = 1f;
            archiveTitleCanvasGroup.interactable = true;
            archiveTitleCanvasGroup.blocksRaycasts = true;
        }

        if (archiveTitleRectTransform != null)
        {
            archiveTitleRectTransform.localScale =
                originalArchiveTitleScale;
        }

        if (topicButtonCanvasGroups == null)
            return;

        for (int i = 0; i < topicButtonCanvasGroups.Length; i++)
        {
            CanvasGroup group =
                topicButtonCanvasGroups[i];

            if (group != null)
            {
                group.alpha = 1f;
                group.interactable = true;
                group.blocksRaycasts = true;
            }

            if (topicButtonRectTransforms != null &&
                topicButtonRectTransforms[i] != null)
            {
                topicButtonRectTransforms[i].localScale =
                    originalTopicButtonScales[i];
            }
        }
    }

    // ==========================================
    // PREPARE CINEMATIC REVEAL
    // ==========================================

    private void PrepareRevealElements()
    {
        if (archiveTitleCanvasGroup != null)
        {
            archiveTitleCanvasGroup.alpha = 0f;
            archiveTitleCanvasGroup.interactable = false;
            archiveTitleCanvasGroup.blocksRaycasts = false;
        }

        if (archiveTitleRectTransform != null)
        {
            archiveTitleRectTransform.localScale =
                originalArchiveTitleScale *
                titleStartingScaleMultiplier;
        }

        if (topicButtonCanvasGroups == null)
            return;

        for (int i = 0; i < topicButtonCanvasGroups.Length; i++)
        {
            CanvasGroup group =
                topicButtonCanvasGroups[i];

            if (group != null)
            {
                group.alpha = 0f;
                group.interactable = false;
                group.blocksRaycasts = false;
            }

            if (topicButtonRectTransforms != null &&
                topicButtonRectTransforms[i] != null)
            {
                topicButtonRectTransforms[i].localScale =
                    originalTopicButtonScales[i] *
                    buttonStartingScaleMultiplier;
            }
        }
    }

    // ==========================================
    // CINEMATIC REVEAL SEQUENCE
    // ==========================================

    private IEnumerator PlayArchiveRevealSequence()
    {
        // Run one subtle gold light sweep before the title and topic reveal.
        if (archiveGoldSweepCanvasGroup != null &&
            archiveGoldSweepRectTransform != null)
        {
            yield return StartCoroutine(PlayArchiveGoldSweep());
        }

        // Play unlock sound ONLY during the first Archive opening.
        if (!firstArchiveCloseCompleted && unlockSound != null)
        {
            unlockSound.Stop();
            unlockSound.Play();
        }

        if (archiveTitleText != null &&
            archiveTitleCanvasGroup != null)
        {
            if (!firstArchiveCloseCompleted)
            {
                archiveTitleText.text =
                    "NEW KNOWLEDGE UNLOCKED";

                yield return StartCoroutine(
                    AnimateRevealElement(
                        archiveTitleCanvasGroup,
                        archiveTitleRectTransform,
                        0f,
                        1f,
                        archiveTitleRectTransform != null
                            ? archiveTitleRectTransform.localScale
                            : Vector3.one,
                        originalArchiveTitleScale,
                        titleFadeDuration
                    )
                );

                if (titleHoldDuration > 0f)
                {
                    yield return new WaitForSecondsRealtime(
                        titleHoldDuration
                    );
                }

                // Fade out the unlock announcement.
                yield return StartCoroutine(
                    AnimateRevealElement(
                        archiveTitleCanvasGroup,
                        archiveTitleRectTransform,
                        1f,
                        0f,
                        originalArchiveTitleScale,
                        originalArchiveTitleScale *
                        titleStartingScaleMultiplier,
                        titleFadeDuration * 0.65f
                    )
                );
            }

            // Show KNOWLEDGE ARCHIVE every time the panel opens.
            archiveTitleText.text = originalArchiveTitle;

            yield return StartCoroutine(
                AnimateRevealElement(
                    archiveTitleCanvasGroup,
                    archiveTitleRectTransform,
                    0f,
                    1f,
                    originalArchiveTitleScale *
                    titleStartingScaleMultiplier,
                    originalArchiveTitleScale,
                    titleFadeDuration
                )
            );
        }

        // Reveal topic buttons one by one in Hierarchy order.
        if (topicButtonCanvasGroups != null)
        {
            for (int i = 0; i < topicButtonCanvasGroups.Length; i++)
            {
                CanvasGroup group =
                    topicButtonCanvasGroups[i];

                RectTransform buttonRect =
                    topicButtonRectTransforms[i];

                Vector3 targetScale =
                    originalTopicButtonScales[i];

                Vector3 startScale =
                    targetScale * buttonStartingScaleMultiplier;

                if (group != null)
                {
                    group.alpha = 0f;
                    group.interactable = false;
                    group.blocksRaycasts = false;
                }

                if (buttonRect != null)
                {
                    buttonRect.localScale = startScale;
                }

                yield return StartCoroutine(
                    AnimateRevealElement(
                        group,
                        buttonRect,
                        0f,
                        1f,
                        startScale,
                        targetScale,
                        buttonRevealDuration
                    )
                );

                if (group != null)
                {
                    group.interactable = true;
                    group.blocksRaycasts = true;
                }

                if (buttonStaggerDelay > 0f &&
                    i < topicButtonCanvasGroups.Length - 1)
                {
                    yield return new WaitForSecondsRealtime(
                        buttonStaggerDelay
                    );
                }
            }
        }
    }

    // ==========================================
    // FADE AND SCALE A REVEAL ELEMENT
    // ==========================================

    private IEnumerator AnimateRevealElement(
        CanvasGroup canvasGroup,
        RectTransform rectTransform,
        float startAlpha,
        float targetAlpha,
        Vector3 startScale,
        Vector3 targetScale,
        float duration
    )
    {
        duration = Mathf.Max(0.01f, duration);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            float smoothT =
                Mathf.SmoothStep(0f, 1f, t);

            if (canvasGroup != null)
            {
                canvasGroup.alpha =
                    Mathf.Lerp(
                        startAlpha,
                        targetAlpha,
                        smoothT
                    );
            }

            if (rectTransform != null)
            {
                rectTransform.localScale =
                    Vector3.Lerp(
                        startScale,
                        targetScale,
                        smoothT
                    );
            }

            yield return null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = targetAlpha;
        }

        if (rectTransform != null)
        {
            rectTransform.localScale = targetScale;
        }
    }

    // ==========================================
    // GOLD LIGHT SWEEP ANIMATION
    // ==========================================

    private IEnumerator PlayArchiveGoldSweep()
    {
        if (archiveGoldSweepCanvasGroup == null ||
            archiveGoldSweepRectTransform == null)
        {
            yield break;
        }

        if (archiveGoldSweep != null &&
            !archiveGoldSweep.activeSelf)
        {
            archiveGoldSweep.SetActive(true);
        }

        float duration =
            Mathf.Max(0.1f, goldSweepDuration);

        float elapsed = 0f;

        float startX =
            goldSweepStartAnchoredPosition.x;

        float endX = goldSweepEndX;

        float startY =
            goldSweepStartAnchoredPosition.y;

        archiveGoldSweepCanvasGroup.alpha = 0f;
        archiveGoldSweepCanvasGroup.interactable = false;
        archiveGoldSweepCanvasGroup.blocksRaycasts = false;

        archiveGoldSweepRectTransform.anchoredPosition =
            goldSweepStartAnchoredPosition;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            float smoothT =
                Mathf.SmoothStep(0f, 1f, t);

            // Smooth movement from the left side to the right side.
            float currentX =
                Mathf.Lerp(startX, endX, smoothT);

            archiveGoldSweepRectTransform.anchoredPosition =
                new Vector2(currentX, startY);

            // Fade in, hold briefly, then fade out.
            float alpha;

            if (t < 0.25f)
            {
                float fadeInT =
                    Mathf.SmoothStep(0f, 1f, t / 0.25f);

                alpha =
                    Mathf.Lerp(
                        0f,
                        goldSweepPeakAlpha,
                        fadeInT
                    );
            }
            else if (t > 0.75f)
            {
                float fadeOutT =
                    Mathf.SmoothStep(
                        0f,
                        1f,
                        (t - 0.75f) / 0.25f
                    );

                alpha =
                    Mathf.Lerp(
                        goldSweepPeakAlpha,
                        0f,
                        fadeOutT
                    );
            }
            else
            {
                alpha = goldSweepPeakAlpha;
            }

            archiveGoldSweepCanvasGroup.alpha = alpha;

            yield return null;
        }

        ResetGoldSweepVisuals();
    }

    // ==========================================
    // OPEN ARCHIVE
    // ==========================================

    public void OpenArchive()
    {
        if (archiveOpen || isTransitioning)
            return;

        if (knowledgeArchivePanel == null)
        {
            Debug.LogError(
                "KnowledgeArchivePanel is not assigned!",
                this
            );

            return;
        }

        // Save previous gameplay states.
        previousMovementState =
            playerMovement != null &&
            playerMovement.enabled;

        previousLookState =
            playerController != null &&
            playerController.IsLookEnabled;

        previousCenterDotState =
            centerDot != null &&
            centerDot.activeSelf;

        if (playerCamera != null)
        {
            previousCameraRotation =
                playerCamera.localRotation;
        }

        // ======================================
        // PAUSE ROOM 2 TIMER
        // ======================================

        timerWasRunning = false;

        if (Room2ChallengeTimer.Instance != null)
        {
            timerWasRunning =
                Room2ChallengeTimer.Instance.IsTimerRunning();

            if (timerWasRunning)
            {
                Room2ChallengeTimer.Instance.PauseTimer();
            }
        }
        else
        {
            Debug.LogError(
                "Room2ChallengeTimer Instance is missing!",
                this
            );
        }

        // ======================================
        // HIDE ARCHIVE BUTTON AND CENTER DOT
        // ======================================

        if (knowledgeArchiveButton != null)
            knowledgeArchiveButton.SetActive(false);

        if (centerDot != null)
            centerDot.SetActive(false);

        // ======================================
        // FREEZE PLAYER
        // ======================================

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerController != null)
            playerController.DisableLook();

        // ======================================
        // PREPARE ARCHIVE
        // ======================================

        archiveOpen = true;

        // Only the first opening uses cinematic animation.
        bool playFirstTimeCinematic =
            !firstArchiveCloseCompleted;

        isTransitioning = playFirstTimeCinematic;

        // ======================================
        // BACKGROUND DIM OVERLAY
        // ======================================

        if (archiveDimOverlay != null)
        {
            archiveDimOverlay.SetActive(true);

            if (archiveDimCanvasGroup != null)
            {
                archiveDimCanvasGroup.alpha =
                    playFirstTimeCinematic
                        ? 0f
                        : backgroundDimAlpha;

                archiveDimCanvasGroup.interactable = false;
                archiveDimCanvasGroup.blocksRaycasts = false;
            }
        }

        knowledgeArchivePanel.SetActive(true);

        knowledgeArchivePanel.transform.SetAsLastSibling();

        // ======================================
        // FIRST-TIME OPENING
        // ======================================

        if (playFirstTimeCinematic)
        {
            PrepareRevealElements();

            if (archiveCanvasGroup != null)
            {
                archiveCanvasGroup.alpha = 0f;
                archiveCanvasGroup.interactable = false;
                archiveCanvasGroup.blocksRaycasts = true;
            }

            if (archiveRectTransform != null)
            {
                archiveRectTransform.localScale =
                    originalScale * startingScaleMultiplier;
            }
        }

        // ======================================
        // SUBSEQUENT OPENINGS - NO CINEMATIC
        // ======================================

        else
        {
            // Restore the title and buttons immediately.
            ResetRevealVisuals();

            // Hide and reset the gold sweep.
            ResetGoldSweepVisuals();

            // Show Archive immediately without fade or scaling.
            if (archiveCanvasGroup != null)
            {
                archiveCanvasGroup.alpha = 1f;
                archiveCanvasGroup.interactable = true;
                archiveCanvasGroup.blocksRaycasts = true;
            }

            if (archiveRectTransform != null)
            {
                archiveRectTransform.localScale = originalScale;
            }

            isTransitioning = false;

            Debug.Log(
                "KNOWLEDGE ARCHIVE OPENED - NO CINEMATIC (ALREADY UNLOCKED)!"
            );
        }

        // ======================================
        // CURSOR
        // ======================================

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // ======================================
        // START FIRST-TIME ANIMATION ONLY
        // ======================================

        if (playFirstTimeCinematic)
        {
            archiveTransitionCoroutine =
                StartCoroutine(AnimateArchive(true));

            Debug.Log(
                "FIRST-TIME KNOWLEDGE ARCHIVE CINEMATIC STARTED!"
            );
        }
    }

    // ==========================================
    // CLOSE ARCHIVE
    // ==========================================

    public void CloseArchive()
    {
        if (!archiveOpen || isTransitioning)
        {
            Debug.LogWarning(
                "Knowledge Archive is not open or is transitioning!",
                this
            );

            return;
        }

        if (knowledgeArchivePanel == null)
        {
            Debug.LogError(
                "KnowledgeArchivePanel is missing!",
                this
            );

            return;
        }

        isTransitioning = true;

        // Prevent clicking buttons during fade-out.
        if (archiveCanvasGroup != null)
        {
            archiveCanvasGroup.interactable = false;
            archiveCanvasGroup.blocksRaycasts = true;
        }

        // Start smooth fade-out.
        archiveTransitionCoroutine =
            StartCoroutine(AnimateArchive(false));

        Debug.Log(
            "KNOWLEDGE ARCHIVE CLOSING ANIMATION STARTED!"
        );
    }

    // ==========================================
    // ARCHIVE ANIMATION
    // OPEN / CLOSE
    // ==========================================

    private IEnumerator AnimateArchive(bool opening)
    {
        float duration = opening
            ? fadeInDuration
            : fadeOutDuration;

        duration = Mathf.Max(0.01f, duration);

        float startAlpha =
            archiveCanvasGroup != null
                ? archiveCanvasGroup.alpha
                : 1f;

        float targetAlpha =
            opening ? 1f : 0f;

        float startDimAlpha =
            archiveDimCanvasGroup != null
                ? archiveDimCanvasGroup.alpha
                : 0f;

        float targetDimAlpha =
            opening
                ? backgroundDimAlpha
                : 0f;

        Vector3 startScale =
            archiveRectTransform != null
                ? archiveRectTransform.localScale
                : originalScale;

        Vector3 targetScale = opening
            ? originalScale
            : originalScale * startingScaleMultiplier;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(elapsed / duration);

            // Smooth animation curve.
            float smoothT =
                Mathf.SmoothStep(0f, 1f, t);

            // Fade animation.
            if (archiveCanvasGroup != null)
            {
                archiveCanvasGroup.alpha =
                    Mathf.Lerp(
                        startAlpha,
                        targetAlpha,
                        smoothT
                    );
            }

            // Scale animation.
            if (archiveRectTransform != null)
            {
                archiveRectTransform.localScale =
                    Vector3.Lerp(
                        startScale,
                        targetScale,
                        smoothT
                    );
            }

            // Fade the background dim overlay at the same time.
            if (archiveDimCanvasGroup != null)
            {
                archiveDimCanvasGroup.alpha =
                    Mathf.Lerp(
                        startDimAlpha,
                        targetDimAlpha,
                        smoothT
                    );
            }

            yield return null;
        }

        // Apply exact final values.
        if (archiveCanvasGroup != null)
        {
            archiveCanvasGroup.alpha = targetAlpha;
        }

        if (archiveRectTransform != null)
        {
            archiveRectTransform.localScale = targetScale;
        }

        if (archiveDimCanvasGroup != null)
        {
            archiveDimCanvasGroup.alpha = targetDimAlpha;
        }

        // ======================================
        // OPENING FINISHED
        // ======================================

        if (opening)
        {
            // Keep the Archive non-interactable while the cinematic reveal runs.
            if (archiveCanvasGroup != null)
            {
                archiveCanvasGroup.interactable = false;
                archiveCanvasGroup.blocksRaycasts = true;
            }

            yield return StartCoroutine(
                PlayArchiveRevealSequence()
            );

            if (archiveCanvasGroup != null)
            {
                archiveCanvasGroup.interactable = true;
                archiveCanvasGroup.blocksRaycasts = true;
            }

            archiveTransitionCoroutine = null;
            isTransitioning = false;

            Debug.Log(
                "KNOWLEDGE ARCHIVE OPENING COMPLETE!"
            );
        }

        // ======================================
        // CLOSING FINISHED
        // ======================================

        else
        {
            archiveTransitionCoroutine = null;
            isTransitioning = false;

            // Only deactivate after fade-out completes.
            knowledgeArchivePanel.SetActive(false);

            if (archiveDimOverlay != null)
            {
                archiveDimOverlay.SetActive(false);
            }

            archiveOpen = false;

            FinishArchiveClose();

            Debug.Log(
                "KNOWLEDGE ARCHIVE CLOSING COMPLETE!"
            );
        }
    }

    // ==========================================
    // FINISH ARCHIVE CLOSE
    // EXISTING GAMEPLAY LOGIC
    // ==========================================

    private void FinishArchiveClose()
    {
        Debug.Log("KNOWLEDGE ARCHIVE CLOSED!");

        // Resume only if the timer was running before opening.
        if (timerWasRunning)
        {
            if (Room2ChallengeTimer.Instance != null)
            {
                Room2ChallengeTimer.Instance.ResumeTimer();

                Debug.Log(
                    "ROOM 2 TIMER RESUMED AFTER ARCHIVE."
                );
            }
        }

        timerWasRunning = false;

        // ======================================
        // FIRST CLOSE ONLY
        // ======================================

        if (!firstArchiveCloseCompleted)
        {
            firstArchiveCloseCompleted = true;

            if (ChallengeProgressManager.Instance != null)
            {
                ChallengeProgressManager.Instance.CompleteTable(2);
            }
            else
            {
                Debug.LogWarning(
                    "ChallengeProgressManager Instance is missing!"
                );
            }

            // Restore normal gameplay controls.
            RestoreNormalGameplay();

            // Show Table 3 instruction.
            if (instructionManager != null)
            {
                instructionManager.ShowFinalChallengeInstruction();
            }
            else
            {
                Debug.LogWarning(
                    "Room2InstructionManager is not assigned!",
                    this
                );
            }

            Debug.Log(
                "FIRST ARCHIVE READING COMPLETED!"
            );
        }

        // ======================================
        // REUSABLE ARCHIVE CLOSE
        // ======================================

        else
        {
            RestorePreviousGameplayState();

            Debug.Log(
                "ARCHIVE CLOSED - PREVIOUS GAMEPLAY STATE RESTORED!"
            );
        }

        // Show reusable Archive button.
        if (knowledgeArchiveButton != null)
        {
            knowledgeArchiveButton.SetActive(true);

            knowledgeArchiveButton.transform.SetAsLastSibling();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("ARCHIVE CLOSE COMPLETE!");
    }

    // ==========================================
    // RESTORE NORMAL GAMEPLAY
    // FIRST CLOSE
    // ==========================================

    private void RestoreNormalGameplay()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        if (playerController != null)
        {
            playerController.enabled = true;
            playerController.EnableLook();
        }

        if (centerDot != null)
        {
            centerDot.SetActive(true);
        }

        if (playerCamera != null)
        {
            playerCamera.localRotation =
                previousCameraRotation;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("NORMAL ROOM 2 GAMEPLAY RESTORED!");
    }

    // ==========================================
    // RESTORE PREVIOUS GAMEPLAY STATE
    // REUSABLE ARCHIVE
    // ==========================================

    private void RestorePreviousGameplayState()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled =
                previousMovementState;
        }

        if (playerController != null)
        {
            playerController.enabled = true;

            if (previousLookState)
            {
                playerController.EnableLook();
            }
            else
            {
                playerController.DisableLook();
            }
        }

        if (centerDot != null)
        {
            centerDot.SetActive(previousCenterDotState);
        }

        if (playerCamera != null)
        {
            playerCamera.localRotation =
                previousCameraRotation;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ==========================================
    // CHECK ARCHIVE STATE
    // ==========================================

    public bool IsArchiveOpen()
    {
        return archiveOpen;
    }

    public bool HasCompletedFirstArchiveReading()
    {
        return firstArchiveCloseCompleted;
    }

    // ==========================================
    // RESET ARCHIVE FOR ROOM 2 RESTART
    // ==========================================

    public void ResetForRoomRestart()
    {
        Debug.Log("RESETTING KNOWLEDGE ARCHIVE...");

        if (archiveTransitionCoroutine != null)
        {
            StopCoroutine(archiveTransitionCoroutine);
            archiveTransitionCoroutine = null;
        }

        archiveOpen = false;
        isTransitioning = false;
        firstArchiveCloseCompleted = false;
        timerWasRunning = false;

        if (knowledgeArchivePanel != null)
        {
            ResetArchiveVisuals();

            knowledgeArchivePanel.SetActive(false);
        }

        if (knowledgeArchiveButton != null)
        {
            knowledgeArchiveButton.SetActive(false);
        }

        ResetGoldSweepVisuals();

        Debug.Log(
            "KNOWLEDGE ARCHIVE RESET - BUTTON HIDDEN!"
        );
    }
}