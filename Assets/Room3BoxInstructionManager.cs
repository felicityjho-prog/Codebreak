using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Room3BoxInstructionManager : MonoBehaviour
{
    [Header("Instruction Panel")]
    public GameObject instructionPanel;
    public TMP_Text headerTitle;

    [Header("Lesson Sections")]
    public GameObject[] lessonSections = new GameObject[5];

    [Header("Navigation Buttons")]
    public Button[] navigationButtons = new Button[5];

    [Header("Start Button")]
    public Button startButton;
    public TMP_Text startButtonText;

    [Header("View Instructions Buttons")]
    public Button[] viewInstructionsButtons = new Button[3];

    [Header("Challenge Manager")]
    public Room3CodingChallengeManager codingChallengeManager;

    [Header("Player Controls")]
    public PlayerMovement playerMovement;
    public PlayerController playerController;

    [Header("Gameplay UI")]
    public GameObject centerDot;
    public GameObject controlsPanel;
    public GameObject pauseButton;
    public GameObject pauseOverlay;

    [Header("Lesson Availability")]
    public bool box2LessonReady = false;
    public bool box3LessonReady = false;

    [Header("UI Transitions")]
    [Min(0.05f)] public float panelTransitionDuration = 0.24f;
    [Min(0.05f)] public float sectionTransitionDuration = 0.16f;
    [Range(0.90f, 1f)] public float panelStartScale = 0.96f;
    [Min(0f)] public float panelSlideDistance = 14f;

    private int currentBoxNumber;
    private int currentSectionIndex;
    private bool challengeHasStarted;
    private bool transitionLocked;

    private CanvasGroup instructionCanvasGroup;
    private RectTransform instructionRect;
    private Vector3 instructionOriginalScale;
    private Vector2 instructionOriginalPosition;

    private CanvasGroup[] sectionCanvasGroups;
    private RectTransform[] sectionRectTransforms;
    private Vector2[] sectionOriginalPositions;

    private Coroutine instructionPanelTransition;
    private Coroutine sectionTransition;
    private Coroutine flowTransition;

    private void Awake()
    {
        if (instructionPanel != null)
        {
            instructionCanvasGroup = GetOrAddCanvasGroup(instructionPanel);
            instructionRect = instructionPanel.GetComponent<RectTransform>();

            if (instructionRect != null)
            {
                instructionOriginalScale = instructionRect.localScale;
                instructionOriginalPosition = instructionRect.anchoredPosition;
            }

            instructionCanvasGroup.alpha = 0f;
            instructionCanvasGroup.interactable = false;
            instructionCanvasGroup.blocksRaycasts = false;
            instructionPanel.SetActive(false);
        }

        CacheLessonSections();
    }

    private void OnEnable()
    {
        RegisterButton(GetButton(navigationButtons, 0), ShowOverview);
        RegisterButton(GetButton(navigationButtons, 1), ShowExampleCode);
        RegisterButton(GetButton(navigationButtons, 2), ShowCodeBreakdown);
        RegisterButton(GetButton(navigationButtons, 3), ShowExpectedOutput);
        RegisterButton(GetButton(navigationButtons, 4), ShowKeyTakeaway);

        RegisterButton(startButton, StartOrResumeChallenge);

        RegisterButton(GetButton(viewInstructionsButtons, 0), ReviewBox1Instructions);
        RegisterButton(GetButton(viewInstructionsButtons, 1), ReviewBox2Instructions);
        RegisterButton(GetButton(viewInstructionsButtons, 2), ReviewBox3Instructions);
    }

    private void OnDisable()
    {
        UnregisterButton(GetButton(navigationButtons, 0), ShowOverview);
        UnregisterButton(GetButton(navigationButtons, 1), ShowExampleCode);
        UnregisterButton(GetButton(navigationButtons, 2), ShowCodeBreakdown);
        UnregisterButton(GetButton(navigationButtons, 3), ShowExpectedOutput);
        UnregisterButton(GetButton(navigationButtons, 4), ShowKeyTakeaway);

        UnregisterButton(startButton, StartOrResumeChallenge);

        UnregisterButton(GetButton(viewInstructionsButtons, 0), ReviewBox1Instructions);
        UnregisterButton(GetButton(viewInstructionsButtons, 1), ReviewBox2Instructions);
        UnregisterButton(GetButton(viewInstructionsButtons, 2), ReviewBox3Instructions);
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

    private void CacheLessonSections()
    {
        if (lessonSections == null)
            return;

        sectionCanvasGroups = new CanvasGroup[lessonSections.Length];
        sectionRectTransforms = new RectTransform[lessonSections.Length];
        sectionOriginalPositions = new Vector2[lessonSections.Length];

        for (int i = 0; i < lessonSections.Length; i++)
        {
            GameObject section = lessonSections[i];

            if (section == null)
                continue;

            sectionCanvasGroups[i] = GetOrAddCanvasGroup(section);
            sectionRectTransforms[i] = section.GetComponent<RectTransform>();

            if (sectionRectTransforms[i] != null)
                sectionOriginalPositions[i] = sectionRectTransforms[i].anchoredPosition;

            sectionCanvasGroups[i].alpha = 0f;
            sectionCanvasGroups[i].interactable = false;
            sectionCanvasGroups[i].blocksRaycasts = false;
            section.SetActive(false);
        }
    }

    private Button GetButton(Button[] buttons, int index)
    {
        if (buttons == null || index < 0 || index >= buttons.Length)
            return null;

        return buttons[index];
    }

    private void RegisterButton(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button == null)
            return;

        button.onClick.RemoveListener(action);
        button.onClick.AddListener(action);
    }

    private void UnregisterButton(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null)
            button.onClick.RemoveListener(action);
    }

    public bool CanOpenBox(int boxNumber)
    {
        if (instructionPanel == null)
        {
            Debug.LogError("Box Instruction Panel is not assigned.", this);
            return false;
        }

        if (boxNumber == 2 && !box2LessonReady)
        {
            Debug.LogWarning("Box 2 lesson content is not ready yet.", this);
            return false;
        }

        if (boxNumber == 3 && !box3LessonReady)
        {
            Debug.LogWarning("Box 3 lesson content is not ready yet.", this);
            return false;
        }

        return boxNumber >= 1 && boxNumber <= 3;
    }

    public void OpenForBox(int boxNumber)
    {
        if (transitionLocked || !CanOpenBox(boxNumber))
            return;

        if (currentBoxNumber != boxNumber)
        {
            currentBoxNumber = boxNumber;
            currentSectionIndex = 0;
            challengeHasStarted = false;
        }

        UpdateHeaderTitle();
        UpdateStartButtonLabel();
        SetLessonSectionImmediate(currentSectionIndex);
        FreezePlayerForInstructions();

        if (instructionPanelTransition != null)
            StopCoroutine(instructionPanelTransition);

        instructionPanelTransition = StartCoroutine(AnimateInstructionPanel(true));

        Debug.Log("BOX " + currentBoxNumber + " INSTRUCTIONS OPENED.", this);
    }

    private void UpdateHeaderTitle()
    {
        if (headerTitle == null)
            return;

        switch (currentBoxNumber)
        {
            case 1:
                headerTitle.text = "BOX 1 — IF-ELSE STATEMENT";
                break;
            case 2:
                headerTitle.text = "BOX 2 — WHILE LOOP";
                break;
            case 3:
                headerTitle.text = "BOX 3 — FOR LOOP";
                break;
        }
    }

    private void ShowOverview() => SetLessonSection(0);
    private void ShowExampleCode() => SetLessonSection(1);
    private void ShowCodeBreakdown() => SetLessonSection(2);
    private void ShowExpectedOutput() => SetLessonSection(3);
    private void ShowKeyTakeaway() => SetLessonSection(4);

    private void SetLessonSection(int sectionIndex)
    {
        if (transitionLocked || lessonSections == null ||
            sectionIndex < 0 || sectionIndex >= lessonSections.Length ||
            lessonSections[sectionIndex] == null)
        {
            return;
        }

        if (sectionIndex == currentSectionIndex &&
            lessonSections[sectionIndex].activeSelf)
        {
            return;
        }

        if (sectionTransition != null)
            StopCoroutine(sectionTransition);

        sectionTransition = StartCoroutine(SwitchLessonSection(sectionIndex));
    }

    private void SetLessonSectionImmediate(int sectionIndex)
    {
        if (lessonSections == null || sectionIndex < 0 ||
            sectionIndex >= lessonSections.Length)
        {
            return;
        }

        currentSectionIndex = sectionIndex;

        for (int i = 0; i < lessonSections.Length; i++)
        {
            if (lessonSections[i] == null || sectionCanvasGroups == null ||
                i >= sectionCanvasGroups.Length)
            {
                continue;
            }

            bool selected = i == sectionIndex;
            lessonSections[i].SetActive(selected);

            CanvasGroup group = sectionCanvasGroups[i];
            if (group != null)
            {
                group.alpha = selected ? 1f : 0f;
                group.interactable = selected;
                group.blocksRaycasts = selected;
            }

            if (sectionRectTransforms[i] != null)
                sectionRectTransforms[i].anchoredPosition = sectionOriginalPositions[i];
        }
    }

    private IEnumerator SwitchLessonSection(int nextIndex)
    {
        transitionLocked = true;
        SetNavigationInteractable(false);

        int previousIndex = currentSectionIndex;

        if (previousIndex >= 0 && previousIndex < lessonSections.Length &&
            previousIndex != nextIndex && lessonSections[previousIndex] != null &&
            lessonSections[previousIndex].activeSelf)
        {
            yield return AnimateLessonSection(previousIndex, false);
        }

        currentSectionIndex = nextIndex;
        yield return AnimateLessonSection(nextIndex, true);

        transitionLocked = false;
        SetNavigationInteractable(true);
        sectionTransition = null;
    }

    private IEnumerator AnimateLessonSection(int index, bool show)
    {
        if (index < 0 || index >= lessonSections.Length ||
            lessonSections[index] == null || sectionCanvasGroups[index] == null)
        {
            yield break;
        }

        GameObject section = lessonSections[index];
        CanvasGroup group = sectionCanvasGroups[index];
        RectTransform rect = sectionRectTransforms[index];
        Vector2 home = sectionOriginalPositions[index];
        float duration = Mathf.Max(0.05f, sectionTransitionDuration);

        if (show)
        {
            section.SetActive(true);
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;

            if (rect != null)
                rect.anchoredPosition = home + new Vector2(panelSlideDistance, 0f);
        }
        else
        {
            group.interactable = false;
            group.blocksRaycasts = false;
        }

        float startAlpha = group.alpha;
        float endAlpha = show ? 1f : 0f;
        Vector2 startPosition = rect != null ? rect.anchoredPosition : home;
        Vector2 endPosition = show
            ? home
            : home + new Vector2(-panelSlideDistance, 0f);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = Mathf.SmoothStep(0f, 1f, t);

            group.alpha = Mathf.Lerp(startAlpha, endAlpha, eased);

            if (rect != null)
                rect.anchoredPosition = Vector2.Lerp(startPosition, endPosition, eased);

            yield return null;
        }

        group.alpha = endAlpha;

        if (rect != null)
            rect.anchoredPosition = home;

        if (show)
        {
            group.interactable = true;
            group.blocksRaycasts = true;
        }
        else
        {
            section.SetActive(false);
        }
    }

    private void SetNavigationInteractable(bool interactable)
    {
        if (navigationButtons != null)
        {
            foreach (Button button in navigationButtons)
            {
                if (button != null)
                    button.interactable = interactable;
            }
        }

        if (startButton != null)
            startButton.interactable = interactable;
    }

    private void UpdateStartButtonLabel()
    {
        if (startButtonText == null)
            return;

        startButtonText.text = challengeHasStarted
            ? "BACK TO CHALLENGE"
            : "START";
    }

    private void FreezePlayerForInstructions()
    {
        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerController != null)
        {
            playerController.DisableLook();
            playerController.enabled = false;
        }

        if (centerDot != null)
            centerDot.SetActive(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (pauseButton != null)
            pauseButton.SetActive(false);

        if (pauseOverlay != null)
            pauseOverlay.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void StartOrResumeChallenge()
    {
        if (transitionLocked || codingChallengeManager == null)
            return;

        if (flowTransition != null)
            StopCoroutine(flowTransition);

        flowTransition = StartCoroutine(StartChallengeTransition());
    }

    private IEnumerator StartChallengeTransition()
    {
        transitionLocked = true;
        SetNavigationInteractable(false);

        yield return AnimateInstructionPanel(false);

        bool started = codingChallengeManager != null &&
                       codingChallengeManager.BeginSelectedChallenge();

        if (!started)
        {
            yield return AnimateInstructionPanel(true);
            SetNavigationInteractable(true);
            transitionLocked = false;
            flowTransition = null;
            yield break;
        }

        challengeHasStarted = true;
        UpdateStartButtonLabel();

        SetNavigationInteractable(true);
        transitionLocked = false;
        flowTransition = null;

        Debug.Log("BOX " + currentBoxNumber + " CHALLENGE START/RESUME REQUESTED.", this);
    }

    private void OpenInstructionsForReview(int boxNumber)
    {
        if (transitionLocked || currentBoxNumber != boxNumber || !challengeHasStarted)
            return;

        if (codingChallengeManager == null)
        {
            Debug.LogError("Room3CodingChallengeManager is not assigned.", this);
            return;
        }

        if (flowTransition != null)
            StopCoroutine(flowTransition);

        flowTransition = StartCoroutine(OpenInstructionsForReviewRoutine(boxNumber));
    }

    private IEnumerator OpenInstructionsForReviewRoutine(int boxNumber)
    {
        transitionLocked = true;
        SetNavigationInteractable(false);
        FreezePlayerForInstructions();

        yield return codingChallengeManager.PauseSelectedChallengeForInstructions();

        UpdateHeaderTitle();
        UpdateStartButtonLabel();
        yield return AnimateInstructionPanel(true);

        SetNavigationInteractable(true);
        transitionLocked = false;
        flowTransition = null;

        Debug.Log("BOX " + boxNumber + " INSTRUCTIONS REOPENED.", this);
    }

    private void ReviewBox1Instructions() => OpenInstructionsForReview(1);
    private void ReviewBox2Instructions() => OpenInstructionsForReview(2);
    private void ReviewBox3Instructions() => OpenInstructionsForReview(3);

    private IEnumerator AnimateInstructionPanel(bool show)
    {
        if (instructionPanel == null || instructionCanvasGroup == null)
            yield break;

        float duration = Mathf.Max(0.05f, panelTransitionDuration);
        Vector3 homeScale = instructionOriginalScale;
        Vector2 homePosition = instructionOriginalPosition;

        if (show)
        {
            instructionPanel.SetActive(true);
            instructionPanel.transform.SetAsLastSibling();
            instructionCanvasGroup.alpha = 0f;
            instructionCanvasGroup.interactable = false;
            instructionCanvasGroup.blocksRaycasts = false;

            if (instructionRect != null)
            {
                instructionRect.localScale = homeScale * panelStartScale;
                instructionRect.anchoredPosition = homePosition + new Vector2(0f, -panelSlideDistance);
            }
        }
        else
        {
            instructionCanvasGroup.interactable = false;
            instructionCanvasGroup.blocksRaycasts = false;
        }

        float startAlpha = instructionCanvasGroup.alpha;
        float endAlpha = show ? 1f : 0f;
        Vector3 startScale = instructionRect != null ? instructionRect.localScale : homeScale;
        Vector3 endScale = show ? homeScale : homeScale * panelStartScale;
        Vector2 startPosition = instructionRect != null
            ? instructionRect.anchoredPosition
            : homePosition;
        Vector2 endPosition = show
            ? homePosition
            : homePosition + new Vector2(0f, -panelSlideDistance);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = Mathf.SmoothStep(0f, 1f, t);

            instructionCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, eased);

            if (instructionRect != null)
            {
                instructionRect.localScale = Vector3.Lerp(startScale, endScale, eased);
                instructionRect.anchoredPosition = Vector2.Lerp(startPosition, endPosition, eased);
            }

            yield return null;
        }

        instructionCanvasGroup.alpha = endAlpha;

        if (instructionRect != null)
        {
            instructionRect.localScale = homeScale;
            instructionRect.anchoredPosition = homePosition;
        }

        if (show)
        {
            instructionCanvasGroup.interactable = true;
            instructionCanvasGroup.blocksRaycasts = true;
        }
        else
        {
            instructionPanel.SetActive(false);
        }
    }

    public void ResetForRoomRestart()
    {
        if (instructionPanelTransition != null)
            StopCoroutine(instructionPanelTransition);

        if (sectionTransition != null)
            StopCoroutine(sectionTransition);

        if (flowTransition != null)
            StopCoroutine(flowTransition);

        currentBoxNumber = 0;
        currentSectionIndex = 0;
        challengeHasStarted = false;
        transitionLocked = false;

        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
            instructionCanvasGroup.alpha = 0f;
            instructionCanvasGroup.interactable = false;
            instructionCanvasGroup.blocksRaycasts = false;

            if (instructionRect != null)
            {
                instructionRect.localScale = instructionOriginalScale;
                instructionRect.anchoredPosition = instructionOriginalPosition;
            }
        }

        SetLessonSectionImmediate(0);
        UpdateStartButtonLabel();
        SetNavigationInteractable(true);
    }
}
