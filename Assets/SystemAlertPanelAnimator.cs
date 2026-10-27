using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class SystemAlertPanelAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panelTransform;

    [Header("Room 3 Test Mode Protection")]
    [SerializeField] private bool hideDuringRoom3Test = true;

    [Header("Initial Alert Effect")]
    [SerializeField] private bool useAlertFlicker = true;

    [SerializeField, Min(0.01f)]
    private float flickerInterval = 0.045f;

    [Header("Heartbeat Animation")]
    [Tooltip("Scale increase for each heartbeat. 0.018 = 1.8%.")]
    [SerializeField, Range(0.001f, 0.05f)]
    private float heartbeatScale = 0.018f;

    [SerializeField, Min(0.03f)]
    private float beatDuration = 0.12f;

    [SerializeField, Min(0f)]
    private float doubleBeatGap = 0.10f;

    [SerializeField, Min(0f)]
    private float pauseBetweenHeartbeats = 0.70f;

    private Vector3 originalScale;
    private bool originalScaleCaptured;
    private Coroutine animationRoutine;

    private void Awake()
    {
        ResolveReferences();
        CaptureOriginalScale();
    }

    private void ResolveReferences()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (panelTransform == null)
            panelTransform = GetComponent<RectTransform>();
    }

    private void CaptureOriginalScale()
    {
        if (panelTransform == null)
            return;

        originalScale = panelTransform.localScale;
        originalScaleCaptured = true;
    }

    // =====================================================
    // ROOM 3 TEST MODE DETECTION
    // =====================================================

    private bool IsRoom3TestModeEnabled()
    {
        if (!hideDuringRoom3Test)
            return false;

        RoomSwitcher[] roomSwitchers =
            FindObjectsByType<RoomSwitcher>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (RoomSwitcher roomSwitcher in roomSwitchers)
        {
            if (roomSwitcher != null && roomSwitcher.testRoom3)
            {
                return true;
            }
        }

        return false;
    }

    // =====================================================
    // ON ENABLE
    // =====================================================

    private void OnEnable()
    {
        ResolveReferences();

        // Prevent the System Alert from appearing during
        // direct Room 3 testing, even if another script
        // attempts to activate this GameObject again.
        if (IsRoom3TestModeEnabled())
        {
            if (animationRoutine != null)
            {
                StopCoroutine(animationRoutine);
                animationRoutine = null;
            }

            Debug.Log(
                "System Alert suppressed: Room 3 Test Mode is enabled.",
                this
            );

            gameObject.SetActive(false);
            return;
        }

        if (panelTransform == null || canvasGroup == null)
        {
            Debug.LogError(
                "SystemAlertPanelAnimator: Missing RectTransform or CanvasGroup.",
                this
            );

            return;
        }

        if (!originalScaleCaptured)
            CaptureOriginalScale();

        // Reset to the original layout before every activation.
        panelTransform.localScale = originalScale;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = true;

        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        animationRoutine = StartCoroutine(
            PlayHeartbeatAnimation()
        );
    }

    // =====================================================
    // ALERT FLICKER AND HEARTBEAT
    // =====================================================

    private IEnumerator PlayHeartbeatAnimation()
    {
        // Brief alert flicker when the panel first appears.
        if (useAlertFlicker)
        {
            float[] flickerValues = { 0.20f, 0.85f, 0.40f, 1f };

            foreach (float alpha in flickerValues)
            {
                canvasGroup.alpha = alpha;

                yield return new WaitForSecondsRealtime(
                    flickerInterval
                );
            }
        }

        canvasGroup.alpha = 1f;

        // Enable the button after the opening effect.
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // Keep the entire alert composition pulsing together.
        while (isActiveAndEnabled)
        {
            // First heartbeat.
            yield return AnimateScale(
                1f + heartbeatScale,
                beatDuration
            );

            yield return AnimateScale(
                1f,
                beatDuration
            );

            // Short pause between the two beats.
            if (doubleBeatGap > 0f)
            {
                yield return new WaitForSecondsRealtime(
                    doubleBeatGap
                );
            }

            // Second, slightly stronger heartbeat.
            yield return AnimateScale(
                1f + heartbeatScale * 1.3f,
                beatDuration * 0.85f
            );

            yield return AnimateScale(
                1f,
                beatDuration
            );

            // Pause before the next heartbeat cycle.
            if (pauseBetweenHeartbeats > 0f)
            {
                yield return new WaitForSecondsRealtime(
                    pauseBetweenHeartbeats
                );
            }
        }

        animationRoutine = null;
    }

    // =====================================================
    // SCALE ANIMATION
    // =====================================================

    private IEnumerator AnimateScale(
        float scaleMultiplier,
        float duration
    )
    {
        Vector3 startingScale = panelTransform.localScale;
        Vector3 targetScale = originalScale * scaleMultiplier;

        float elapsed = 0f;
        duration = Mathf.Max(0.03f, duration);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);

            float smoothProgress = Mathf.SmoothStep(
                0f,
                1f,
                progress
            );

            panelTransform.localScale = Vector3.Lerp(
                startingScale,
                targetScale,
                smoothProgress
            );

            yield return null;
        }

        panelTransform.localScale = targetScale;
    }

    // =====================================================
    // ON DISABLE
    // =====================================================

    private void OnDisable()
    {
        if (animationRoutine != null)
        {
            StopCoroutine(animationRoutine);
            animationRoutine = null;
        }

        // Restore the original panel layout.
        if (panelTransform != null && originalScaleCaptured)
        {
            panelTransform.localScale = originalScale;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
}