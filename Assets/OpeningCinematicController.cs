using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class OpeningCinematicController : MonoBehaviour
{
    [Header("Scene References")]
    public Transform playerRoot;
    public Transform cameraTransform;
    public Transform firstPersonAnchor;

    public PlayerController playerController;
    public PlayerMovement playerMovement;
    public GlobalCameraCollision cameraCollision;
    public RoomSwitcher roomSwitcher;

    [Header("Hallway References")]
    public HallwayInstructionController hallwayInstructionController;
    public RoomEntryTrigger roomEntryTrigger;

    [Header("Loading Screen")]
    [Tooltip("Full-screen simple loading panel. Keep it as a sibling of SystemAlertPanel under the main Canvas.")]
    public GameObject loadingScreenPanel;

    [Tooltip("CanvasGroup used to fade out the loading screen. Added automatically if missing.")]
    public CanvasGroup loadingScreenCanvasGroup;

    [Tooltip("Image set to Type = Filled, Horizontal, Fill Origin = Left.")]
    public Image loadingProgressFill;

    [Tooltip("Optional TMP label for short loading status messages.")]
    public TMP_Text loadingStatusText;

    [Min(0.1f)]
    [Tooltip("Duration of the simple animated loading screen.")]
    public float loadingDuration = 2.5f;

    [Min(0f)]
    [Tooltip("Fade-out time before the System Alert appears.")]
    public float loadingFadeDuration = 0.6f;

    [Header("System Alert UI")]
    public GameObject systemAlertPanel;
    public Button beginMissionButton;

    [Header("Opening Audio Sequence")]
    [Tooltip("First audio: the warning sound when the System Alert panel appears.")]
    public AudioSource alertSound;

    [Tooltip("Second audio: SystemMaleAlertAIVoice. Plays after the warning sound.")]
    public AudioSource systemMaleAlertAIVoice;

    [Header("Starting Camera Shot")]
    [Tooltip("Position relative to the Player.")]
    public Vector3 behindPlayerOffset = new Vector3(0.45f, 1.4f, -1.8f);

    [Tooltip("Look target relative to the Player.")]
    public Vector3 lookTargetOffset = new Vector3(0f, 1.45f, 1.5f);

    [Header("Transition Settings")]
    [Min(0.1f)]
    public float transitionDuration = 2.5f;

    public bool ShouldDelayHallwaySequence { get; private set; }

    private Quaternion originalCameraRotation;

    private bool originalMovementEnabled;
    private bool originalLookEnabled;
    private bool originalCollisionEnabled;

    private bool transitionStarted;
    private bool cinematicActive;
    private bool loadingSequenceStarted;

    private void Awake()
    {
        ResolveReferences();
        PrepareLoadingScreen();

        // Do not run the opening cinematic in Room 2 Test Mode.
        if (roomSwitcher != null && roomSwitcher.testRoom2)
        {
            ShouldDelayHallwaySequence = false;
            StopOpeningAudio();

            if (systemAlertPanel != null)
                systemAlertPanel.SetActive(false);

            HideLoadingScreenImmediately();
            return;
        }

        if (!HasRequiredReferences())
        {
            Debug.LogError(
                "OpeningCinematicController: Missing required references. " +
                "Check the Inspector. Cinematic setup was skipped.",
                this
            );

            ShouldDelayHallwaySequence = false;
            StopOpeningAudio();

            if (systemAlertPanel != null)
                systemAlertPanel.SetActive(false);

            HideLoadingScreenImmediately();
            return;
        }

        // Link hallway scripts before their Start methods execute.
        hallwayInstructionController.openingCinematic = this;
        roomEntryTrigger.openingCinematic = this;

        // Prevent the normal hallway sequence from starting before the opening finishes.
        ShouldDelayHallwaySequence = true;
        cinematicActive = true;

        // Save the normal first-person camera rotation and original control states.
        originalCameraRotation = cameraTransform.localRotation;
        originalMovementEnabled = playerMovement.enabled;
        originalLookEnabled = playerController.IsLookEnabled;
        originalCollisionEnabled = cameraCollision.enabled;

        // Temporarily disable gameplay controls and camera collision during the cinematic.
        playerMovement.enabled = false;
        playerController.DisableLook();
        cameraCollision.enabled = false;

        // Stop the normal hallway voice so it cannot overlap the opening alert.
        if (roomEntryTrigger.voiceOver != null)
            roomEntryTrigger.voiceOver.Stop();

        if (hallwayInstructionController.voiceOver != null)
            hallwayInstructionController.voiceOver.Stop();

        // Start in the third-person camera shot.
        PositionStartingCamera();

        // Keep the System Alert hidden during the loading sequence.
        // It will appear only after the loading bar completes and fades out.
        systemAlertPanel.SetActive(false);
        beginMissionButton.interactable = false;

        beginMissionButton.onClick.RemoveListener(BeginMission);
        beginMissionButton.onClick.AddListener(BeginMission);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(PlayLoadingThenSystemAlert());

        Debug.Log("Opening cinematic started: loading screen, then System Alert audio sequence.");
    }

    private void ResolveReferences()
    {
        if (playerRoot == null && playerController != null)
            playerRoot = playerController.transform;

        if (playerController == null && playerRoot != null)
            playerController = playerRoot.GetComponent<PlayerController>();

        if (playerMovement == null && playerRoot != null)
            playerMovement = playerRoot.GetComponent<PlayerMovement>();

        if (cameraTransform == null && playerController != null)
            cameraTransform = playerController.cameraTransform;

        if (firstPersonAnchor == null && playerRoot != null)
            firstPersonAnchor = playerRoot.Find("FirstPersonAnchor");

        if (cameraCollision == null && cameraTransform != null)
            cameraCollision = cameraTransform.GetComponent<GlobalCameraCollision>();

        if (roomSwitcher == null)
            roomSwitcher = FindFirstObjectByType<RoomSwitcher>();

        if (hallwayInstructionController == null)
            hallwayInstructionController = FindFirstObjectByType<HallwayInstructionController>();

        if (roomEntryTrigger == null)
            roomEntryTrigger = FindFirstObjectByType<RoomEntryTrigger>();

        if (beginMissionButton == null && systemAlertPanel != null)
            beginMissionButton = systemAlertPanel.GetComponentInChildren<Button>(true);
    }

    private bool HasRequiredReferences()
    {
        return playerRoot != null &&
               cameraTransform != null &&
               firstPersonAnchor != null &&
               playerController != null &&
               playerMovement != null &&
               cameraCollision != null &&
               roomSwitcher != null &&
               hallwayInstructionController != null &&
               roomEntryTrigger != null &&
               systemAlertPanel != null &&
               beginMissionButton != null;
    }

    private void PositionStartingCamera()
    {
        cameraTransform.position = playerRoot.TransformPoint(behindPlayerOffset);

        Vector3 lookTarget = playerRoot.TransformPoint(lookTargetOffset);
        Vector3 direction = lookTarget - cameraTransform.position;

        if (direction.sqrMagnitude > 0.001f)
            cameraTransform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    private void LateUpdate()
    {
        // Keep the cursor available while the System Alert is waiting for the player.
        if (cinematicActive && !transitionStarted)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void PrepareLoadingScreen()
    {
        // Prevent the alert from appearing behind the loading screen at scene startup.
        if (systemAlertPanel != null)
            systemAlertPanel.SetActive(false);

        if (loadingScreenPanel == null)
        {
            Debug.LogWarning(
                "OpeningCinematicController: Loading Screen Panel is not assigned. " +
                "The System Alert will appear without the loading screen.",
                this
            );
            return;
        }

        if (loadingScreenCanvasGroup == null)
            loadingScreenCanvasGroup = loadingScreenPanel.GetComponent<CanvasGroup>();

        if (loadingScreenCanvasGroup == null)
            loadingScreenCanvasGroup = loadingScreenPanel.AddComponent<CanvasGroup>();

        loadingScreenPanel.SetActive(true);
        loadingScreenPanel.transform.SetAsLastSibling();

        loadingScreenCanvasGroup.alpha = 1f;
        loadingScreenCanvasGroup.interactable = false;
        loadingScreenCanvasGroup.blocksRaycasts = true;

        if (loadingProgressFill != null)
            loadingProgressFill.fillAmount = 0f;

        if (loadingStatusText != null)
            loadingStatusText.text = "Preparing your game...";
    }

    private void HideLoadingScreenImmediately()
    {
        if (loadingScreenPanel != null)
            loadingScreenPanel.SetActive(false);
    }

    private IEnumerator PlayLoadingThenSystemAlert()
    {
        if (loadingSequenceStarted)
            yield break;

        loadingSequenceStarted = true;

        if (loadingScreenPanel != null)
        {
            loadingScreenPanel.SetActive(true);
            loadingScreenPanel.transform.SetAsLastSibling();

            if (loadingScreenCanvasGroup != null)
            {
                loadingScreenCanvasGroup.alpha = 1f;
                loadingScreenCanvasGroup.interactable = false;
                loadingScreenCanvasGroup.blocksRaycasts = true;
            }

            if (loadingProgressFill != null)
                loadingProgressFill.fillAmount = 0f;

            float duration = Mathf.Max(0.1f, loadingDuration);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                if (!cinematicActive || transitionStarted)
                    yield break;

                elapsed += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(elapsed / duration);
                float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

                if (loadingProgressFill != null)
                    loadingProgressFill.fillAmount = smoothProgress;

                if (loadingStatusText != null)
                {
                    if (progress < 0.35f)
                        loadingStatusText.text = "Preparing your game...";
                    else if (progress < 0.75f)
                        loadingStatusText.text = "Loading environment...";
                    else
                        loadingStatusText.text = "Almost ready...";
                }

                yield return null;
            }

            if (loadingProgressFill != null)
                loadingProgressFill.fillAmount = 1f;

            // Briefly let the player see the completed progress bar before transitioning.
            yield return new WaitForSecondsRealtime(0.2f);

            if (!cinematicActive || transitionStarted)
                yield break;

            // IMPORTANT: Activate the System Alert UNDER the still-opaque loading screen.
            // This prevents the gameplay hallway from flashing through during the fade.
            // Do not start alert audio yet; it begins only after the loading screen is gone.
            if (systemAlertPanel != null)
            {
                systemAlertPanel.SetActive(true);
                systemAlertPanel.transform.SetAsLastSibling();
                beginMissionButton.interactable = false;
            }

            // Put the loading screen back on top so it covers the alert until its fade starts.
            loadingScreenPanel.transform.SetAsLastSibling();

            if (loadingScreenCanvasGroup != null)
            {
                yield return StartCoroutine(
                    FadeLoadingScreen(1f, 0f, loadingFadeDuration)
                );
            }

            loadingScreenPanel.SetActive(false);
        }

        if (!cinematicActive || transitionStarted)
            yield break;

        // Ensure the alert is the top UI now that loading is finished.
        if (systemAlertPanel != null)
        {
            systemAlertPanel.SetActive(true);
            systemAlertPanel.transform.SetAsLastSibling();
        }
        beginMissionButton.interactable = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(PlaySystemAlertSequence());
    }

    private IEnumerator FadeLoadingScreen(
        float startAlpha,
        float endAlpha,
        float duration)
    {
        if (loadingScreenCanvasGroup == null)
            yield break;

        if (duration <= 0f)
        {
            loadingScreenCanvasGroup.alpha = endAlpha;
            yield break;
        }

        float elapsed = 0f;
        loadingScreenCanvasGroup.alpha = startAlpha;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);
            loadingScreenCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, smoothProgress);
            yield return null;
        }

        loadingScreenCanvasGroup.alpha = endAlpha;
    }

    private IEnumerator PlaySystemAlertSequence()
    {
        // 1. Play the warning sound first.
        if (alertSound != null && alertSound.clip != null)
        {
            PrepareOneShotAudio(alertSound);
            alertSound.Play();

            yield return WaitForAudioToFinish(alertSound);
        }
        else
        {
            Debug.LogWarning(
                "OpeningCinematicController: Warning sound AudioSource/clip is not assigned.",
                this
            );
        }

        if (!cinematicActive || transitionStarted)
            yield break;

        // 2. Start the System Alert AI voice.
        if (systemMaleAlertAIVoice != null &&
            systemMaleAlertAIVoice.clip != null)
        {
            PrepareOneShotAudio(systemMaleAlertAIVoice);
            systemMaleAlertAIVoice.Play();

            // IMPORTANT:
            // Enable BEGIN MISSION as soon as the AI voice starts.
            // The player no longer needs to wait for the voice to finish.
            if (cinematicActive &&
                !transitionStarted &&
                beginMissionButton != null)
            {
                beginMissionButton.interactable = true;

                Debug.Log(
                    "BEGIN MISSION is available while the System Alert AI voice is playing."
                );
            }

            // The AI voice continues playing unless the player clicks
            // BEGIN MISSION, which stops the audio and starts the transition.
            yield return WaitForAudioToFinish(systemMaleAlertAIVoice);
        }
        else
        {
            Debug.LogWarning(
                "OpeningCinematicController: SystemMaleAlertAIVoice AudioSource/clip is not assigned.",
                this
            );

            // Still allow the player to proceed if no AI voice is assigned.
            if (cinematicActive &&
                !transitionStarted &&
                beginMissionButton != null)
            {
                beginMissionButton.interactable = true;
            }
        }
    }

    private void PrepareOneShotAudio(AudioSource source)
    {
        if (source == null)
            return;

        source.Stop();
        source.playOnAwake = false;
        source.loop = false;
    }

    private IEnumerator WaitForAudioToFinish(AudioSource source)
    {
        while (source != null && source.isPlaying)
            yield return null;
    }

    private void BeginMission()
    {
        if (transitionStarted || !ShouldDelayHallwaySequence)
            return;

        if (cameraTransform == null || cameraTransform.parent == null || firstPersonAnchor == null)
        {
            Debug.LogError(
                "Opening cinematic cannot transition: Camera parent or FirstPersonAnchor is missing.",
                this
            );
            return;
        }

        transitionStarted = true;
        beginMissionButton.interactable = false;

        // Stop any remaining alert audio, then hide the panel and transition.
        StopOpeningAudio();
        systemAlertPanel.SetActive(false);

        StartCoroutine(TransitionToFirstPersonThenWelcome());
    }

    private void StopOpeningAudio()
    {
        if (alertSound != null && alertSound.isPlaying)
            alertSound.Stop();

        if (systemMaleAlertAIVoice != null && systemMaleAlertAIVoice.isPlaying)
            systemMaleAlertAIVoice.Stop();
    }

    private IEnumerator TransitionToFirstPersonThenWelcome()
    {
        Transform cameraParent = cameraTransform.parent;

        Vector3 startingPosition = cameraTransform.localPosition;
        Quaternion startingRotation = cameraTransform.localRotation;
        Vector3 targetPosition = cameraParent.InverseTransformPoint(firstPersonAnchor.position);
        Quaternion targetRotation = originalCameraRotation;

        float duration = Mathf.Max(0.1f, transitionDuration);
        float elapsed = 0f;

        // 4. Smoothly move from the third-person shot to the first-person anchor.
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            cameraTransform.localPosition = Vector3.Lerp(startingPosition, targetPosition, smoothProgress);
            cameraTransform.localRotation = Quaternion.Slerp(startingRotation, targetRotation, smoothProgress);

            yield return null;
        }

        cameraTransform.localPosition = targetPosition;
        cameraTransform.localRotation = targetRotation;

        // Keep the first-person camera stable and synchronize mouse-look rotation.
        playerController.SetCameraRotation(targetRotation);
        cameraCollision.SetNormalLocalPosition(targetPosition);
        cameraCollision.enabled = originalCollisionEnabled;

        // Let the existing hallway scripts start, but keep movement and look disabled for the welcome voice.
        ShouldDelayHallwaySequence = false;
        hallwayInstructionController.BeginInstructionVisualsOnly();
        roomEntryTrigger.BeginHallwaySequence();

        // 5. RoomEntryTrigger's voiceOver is the existing WELCOME TO CODEBREAK AI Voice.
        AudioSource welcomeVoice = roomEntryTrigger != null ? roomEntryTrigger.voiceOver : null;
        if (welcomeVoice != null && welcomeVoice.clip != null)
        {
            // Give BeginHallwaySequence one frame to start its assigned AudioSource.
            yield return null;

            while (welcomeVoice != null && welcomeVoice.isPlaying)
                yield return null;
        }
        else
        {
            Debug.LogWarning(
                "OpeningCinematicController: RoomEntryTrigger Voice Over is not assigned. " +
                "The game will continue without waiting for WELCOME TO CODEBREAK.",
                this
            );
        }

        // 6. Normal gameplay begins only after the welcome voice finishes.
        RestoreGameplayControls();
        Debug.Log("Opening cinematic complete. Welcome voice finished; normal gameplay is active.");
    }

    private void RestoreGameplayControls()
    {
        cinematicActive = false;

        playerMovement.enabled = originalMovementEnabled;

        if (originalLookEnabled)
        {
            playerController.EnableLook();
        }
        else
        {
            playerController.DisableLook();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void OnDestroy()
    {
        StopOpeningAudio();

        if (roomEntryTrigger != null && roomEntryTrigger.voiceOver != null)
            roomEntryTrigger.voiceOver.Stop();

        if (beginMissionButton != null)
            beginMissionButton.onClick.RemoveListener(BeginMission);
    }
}
