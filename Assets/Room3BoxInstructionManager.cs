using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Room3BoxInstructionManager : MonoBehaviour
{
    [Header("Instruction Panel")]
    public GameObject instructionPanel;
    public TMP_Text headerTitle;

    [Header("Box Content Parents")]
    public GameObject[] boxContentParents = new GameObject[3];

    [Header("Box 1 Lesson Sections")]
    public GameObject[] lessonSections = new GameObject[5];

    [Header("Box 2 Lesson Sections")]
    public GameObject[] box2LessonSections = new GameObject[5];

    [Header("Box 3 Lesson Sections")]
    public GameObject[] box3LessonSections = new GameObject[5];

    [Header("Navigation Buttons")]
    public Button[] navigationButtons = new Button[5];

    [Header("Navigation Button Highlight")]
    public Color selectedNavigationColor =
        new Color32(8, 126, 170, 255);

    public Color selectedNavigationHighlightedColor =
        new Color32(0, 176, 210, 255);

    public Color selectedNavigationPressedColor =
        new Color32(4, 96, 130, 255);

    public Color selectedNavigationOutlineColor =
        new Color32(0, 223, 255, 255);

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
    [Min(0.05f)]
    public float panelTransitionDuration = 0.24f;

    [Min(0.05f)]
    public float sectionTransitionDuration = 0.16f;

    [Range(0.90f, 1f)]
    public float panelStartScale = 0.96f;

    [Min(0f)]
    public float panelSlideDistance = 14f;

    private int currentBoxNumber;
    private int currentSectionIndex;

    private bool challengeHasStarted;
    private bool transitionLocked;

    private CanvasGroup instructionCanvasGroup;
    private RectTransform instructionRect;

    private Vector3 instructionOriginalScale;
    private Vector2 instructionOriginalPosition;

    private CanvasGroup[][] sectionCanvasGroupsByBox;
    private RectTransform[][] sectionRectTransformsByBox;
    private Vector2[][] sectionOriginalPositionsByBox;

    private ColorBlock[] originalNavigationColorBlocks;
    private Outline[] navigationOutlines;
    private Color[] originalNavigationOutlineColors;

    private Coroutine instructionPanelTransition;
    private Coroutine sectionTransition;
    private Coroutine flowTransition;

    private void Awake()
    {
        if (instructionPanel != null)
        {
            instructionCanvasGroup =
                GetOrAddCanvasGroup(instructionPanel);

            instructionRect =
                instructionPanel.GetComponent<RectTransform>();

            if (instructionRect != null)
            {
                instructionOriginalScale =
                    instructionRect.localScale;

                instructionOriginalPosition =
                    instructionRect.anchoredPosition;
            }

            instructionCanvasGroup.alpha = 0f;
            instructionCanvasGroup.interactable = false;
            instructionCanvasGroup.blocksRaycasts = false;

            instructionPanel.SetActive(false);
        }

        CacheLessonSections();
        CacheNavigationButtonStyles();

        UpdateNavigationButtonHighlight(0);
        SetContentParent(1);
    }

    private void OnEnable()
    {
        RegisterButton(GetButton(navigationButtons, 0), ShowOverview);
        RegisterButton(GetButton(navigationButtons, 1), ShowExampleCode);
        RegisterButton(GetButton(navigationButtons, 2), ShowCodeBreakdown);
        RegisterButton(GetButton(navigationButtons, 3), ShowExpectedOutput);
        RegisterButton(GetButton(navigationButtons, 4), ShowKeyTakeaway);

        RegisterButton(startButton, StartOrResumeChallenge);

        RegisterButton(
            GetButton(viewInstructionsButtons, 0),
            ReviewBox1Instructions
        );

        RegisterButton(
            GetButton(viewInstructionsButtons, 1),
            ReviewBox2Instructions
        );

        RegisterButton(
            GetButton(viewInstructionsButtons, 2),
            ReviewBox3Instructions
        );
    }

    private void OnDisable()
    {
        UnregisterButton(GetButton(navigationButtons, 0), ShowOverview);
        UnregisterButton(GetButton(navigationButtons, 1), ShowExampleCode);
        UnregisterButton(GetButton(navigationButtons, 2), ShowCodeBreakdown);
        UnregisterButton(GetButton(navigationButtons, 3), ShowExpectedOutput);
        UnregisterButton(GetButton(navigationButtons, 4), ShowKeyTakeaway);

        UnregisterButton(startButton, StartOrResumeChallenge);

        UnregisterButton(
            GetButton(viewInstructionsButtons, 0),
            ReviewBox1Instructions
        );

        UnregisterButton(
            GetButton(viewInstructionsButtons, 1),
            ReviewBox2Instructions
        );

        UnregisterButton(
            GetButton(viewInstructionsButtons, 2),
            ReviewBox3Instructions
        );
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

    // =========================================================
    // LESSON SECTION REFERENCES
    // =========================================================

    private GameObject[] GetSectionsForBox(int boxNumber)
    {
        switch (boxNumber)
        {
            case 1:
                return lessonSections;

            case 2:
                return box2LessonSections;

            case 3:
                return box3LessonSections;

            default:
                return lessonSections;
        }
    }

    private int GetBoxIndex(int boxNumber)
    {
        return Mathf.Clamp(boxNumber - 1, 0, 2);
    }

    private CanvasGroup[] GetSectionCanvasGroups(int boxNumber)
    {
        if (sectionCanvasGroupsByBox == null)
            return null;

        int index = GetBoxIndex(boxNumber);

        return index < sectionCanvasGroupsByBox.Length
            ? sectionCanvasGroupsByBox[index]
            : null;
    }

    private RectTransform[] GetSectionRectTransforms(int boxNumber)
    {
        if (sectionRectTransformsByBox == null)
            return null;

        int index = GetBoxIndex(boxNumber);

        return index < sectionRectTransformsByBox.Length
            ? sectionRectTransformsByBox[index]
            : null;
    }

    private Vector2[] GetSectionOriginalPositions(int boxNumber)
    {
        if (sectionOriginalPositionsByBox == null)
            return null;

        int index = GetBoxIndex(boxNumber);

        return index < sectionOriginalPositionsByBox.Length
            ? sectionOriginalPositionsByBox[index]
            : null;
    }

    private void CacheLessonSections()
    {
        sectionCanvasGroupsByBox = new CanvasGroup[3][];
        sectionRectTransformsByBox = new RectTransform[3][];
        sectionOriginalPositionsByBox = new Vector2[3][];

        CacheLessonSectionsForBox(1);
        CacheLessonSectionsForBox(2);
        CacheLessonSectionsForBox(3);
    }

    private void CacheLessonSectionsForBox(int boxNumber)
    {
        GameObject[] sections = GetSectionsForBox(boxNumber);

        if (sections == null)
            return;

        int boxIndex = GetBoxIndex(boxNumber);

        sectionCanvasGroupsByBox[boxIndex] =
            new CanvasGroup[sections.Length];

        sectionRectTransformsByBox[boxIndex] =
            new RectTransform[sections.Length];

        sectionOriginalPositionsByBox[boxIndex] =
            new Vector2[sections.Length];

        for (int i = 0; i < sections.Length; i++)
        {
            GameObject section = sections[i];

            if (section == null)
                continue;

            sectionCanvasGroupsByBox[boxIndex][i] =
                GetOrAddCanvasGroup(section);

            sectionRectTransformsByBox[boxIndex][i] =
                section.GetComponent<RectTransform>();

            if (sectionRectTransformsByBox[boxIndex][i] != null)
            {
                sectionOriginalPositionsByBox[boxIndex][i] =
                    sectionRectTransformsByBox[boxIndex][i]
                    .anchoredPosition;
            }

            sectionCanvasGroupsByBox[boxIndex][i].alpha = 0f;
            sectionCanvasGroupsByBox[boxIndex][i].interactable = false;
            sectionCanvasGroupsByBox[boxIndex][i].blocksRaycasts = false;

            section.SetActive(false);
        }
    }

    private void SetContentParent(int boxNumber)
    {
        if (boxContentParents == null)
            return;

        for (int i = 0; i < boxContentParents.Length; i++)
        {
            if (boxContentParents[i] != null)
            {
                boxContentParents[i].SetActive(
                    i == boxNumber - 1
                );
            }
        }
    }

    // =========================================================
    // NAVIGATION BUTTON HIGHLIGHT
    // =========================================================

    private void CacheNavigationButtonStyles()
    {
        if (navigationButtons == null)
        {
            originalNavigationColorBlocks = new ColorBlock[0];
            navigationOutlines = new Outline[0];
            originalNavigationOutlineColors = new Color[0];
            return;
        }

        int count = navigationButtons.Length;

        originalNavigationColorBlocks = new ColorBlock[count];
        navigationOutlines = new Outline[count];
        originalNavigationOutlineColors = new Color[count];

        for (int i = 0; i < count; i++)
        {
            Button button = navigationButtons[i];

            if (button == null)
                continue;

            originalNavigationColorBlocks[i] = button.colors;

            navigationOutlines[i] =
                button.GetComponent<Outline>();

            if (navigationOutlines[i] != null)
            {
                originalNavigationOutlineColors[i] =
                    navigationOutlines[i].effectColor;
            }

            // The script controls the selected color directly.
            // This prevents Unity's temporary hover tint from replacing it.
            button.transition = Selectable.Transition.None;
        }
    }

    private void UpdateNavigationButtonHighlight(int selectedIndex)
    {
        if (navigationButtons == null)
            return;

        if (originalNavigationColorBlocks == null ||
            originalNavigationColorBlocks.Length != navigationButtons.Length)
        {
            CacheNavigationButtonStyles();
        }

        for (int i = 0; i < navigationButtons.Length; i++)
        {
            Button button = navigationButtons[i];

            if (button == null)
                continue;

            bool isSelected = i == selectedIndex;

            if (button.targetGraphic != null)
            {
                if (isSelected)
                {
                    button.targetGraphic.color =
                        selectedNavigationColor;
                }
                else
                {
                    // Restore the original normal button color manually.
                    button.targetGraphic.color =
                        originalNavigationColorBlocks[i].normalColor;
                }
            }

            if (navigationOutlines != null &&
                i < navigationOutlines.Length &&
                navigationOutlines[i] != null)
            {
                navigationOutlines[i].effectColor =
                    isSelected
                        ? selectedNavigationOutlineColor
                        : originalNavigationOutlineColors[i];
            }
        }
    }

    // =========================================================
    // BUTTON HELPERS
    // =========================================================

    private Button GetButton(Button[] buttons, int index)
    {
        if (buttons == null ||
            index < 0 ||
            index >= buttons.Length)
        {
            return null;
        }

        return buttons[index];
    }

    private void RegisterButton(
        Button button,
        UnityEngine.Events.UnityAction action)
    {
        if (button == null)
            return;

        button.onClick.RemoveListener(action);
        button.onClick.AddListener(action);
    }

    private void UnregisterButton(
        Button button,
        UnityEngine.Events.UnityAction action)
    {
        if (button != null)
            button.onClick.RemoveListener(action);
    }

    // =========================================================
    // CHECK LESSON AVAILABILITY
    // =========================================================

    public bool CanOpenBox(int boxNumber)
    {
        if (instructionPanel == null)
        {
            Debug.LogError(
                "Box Instruction Panel is not assigned.",
                this
            );

            return false;
        }

        if (boxNumber < 1 || boxNumber > 3)
            return false;

        if (boxNumber == 2 && !box2LessonReady)
        {
            Debug.LogWarning(
                "Box 2 lesson content is not ready yet.",
                this
            );

            return false;
        }

        if (boxNumber == 3 && !box3LessonReady)
        {
            Debug.LogWarning(
                "Box 3 lesson content is not ready yet.",
                this
            );

            return false;
        }

        GameObject[] sections =
            GetSectionsForBox(boxNumber);

        if (sections == null || sections.Length < 5)
        {
            Debug.LogError(
                "The lesson section array for Box " +
                boxNumber + " must contain five pages.",
                this
            );

            return false;
        }

        for (int i = 0; i < 5; i++)
        {
            if (sections[i] == null)
            {
                Debug.LogError(
                    "A lesson page is missing in Box " +
                    boxNumber + " at index " + i + ".",
                    this
                );

                return false;
            }
        }

        return true;
    }

    // =========================================================
    // OPEN LESSON FOR SELECTED BOX
    // =========================================================

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

        SetContentParent(currentBoxNumber);

        UpdateHeaderTitle();
        UpdateStartButtonLabel();

        SetLessonSectionImmediate(currentSectionIndex);

        FreezePlayerForInstructions();

        if (instructionPanelTransition != null)
            StopCoroutine(instructionPanelTransition);

        instructionPanelTransition =
            StartCoroutine(AnimateInstructionPanel(true));

        Debug.Log(
            "BOX " + currentBoxNumber + " INSTRUCTIONS OPENED.",
            this
        );
    }

    private void UpdateHeaderTitle()
    {
        if (headerTitle == null)
            return;

        switch (currentBoxNumber)
        {
            case 1:
                headerTitle.text =
                    "BOX 1 — IF-ELSE STATEMENT";
                break;

            case 2:
                headerTitle.text =
                    "BOX 2 — WHILE LOOP";
                break;

            case 3:
                headerTitle.text =
                    "BOX 3 — FOR LOOP";
                break;
        }
    }

    // =========================================================
    // NAVIGATION BUTTON ACTIONS
    // =========================================================

    private void ShowOverview()
    {
        SetLessonSection(0);
    }

    private void ShowExampleCode()
    {
        SetLessonSection(1);
    }

    private void ShowCodeBreakdown()
    {
        SetLessonSection(2);
    }

    private void ShowExpectedOutput()
    {
        SetLessonSection(3);
    }

    private void ShowKeyTakeaway()
    {
        SetLessonSection(4);
    }

    private void SetLessonSection(int sectionIndex)
    {
        GameObject[] sections =
            GetSectionsForBox(currentBoxNumber);

        if (transitionLocked ||
            sections == null ||
            sectionIndex < 0 ||
            sectionIndex >= sections.Length ||
            sections[sectionIndex] == null)
        {
            return;
        }

        if (sectionIndex == currentSectionIndex &&
            sections[sectionIndex].activeSelf)
        {
            return;
        }

        if (sectionTransition != null)
            StopCoroutine(sectionTransition);

        sectionTransition =
            StartCoroutine(
                SwitchLessonSection(currentBoxNumber, sectionIndex)
            );
    }

    private void SetLessonSectionImmediate(int sectionIndex)
    {
        GameObject[] sections =
            GetSectionsForBox(currentBoxNumber);

        CanvasGroup[] groups =
            GetSectionCanvasGroups(currentBoxNumber);

        RectTransform[] rects =
            GetSectionRectTransforms(currentBoxNumber);

        Vector2[] originalPositions =
            GetSectionOriginalPositions(currentBoxNumber);

        if (sections == null ||
            sectionIndex < 0 ||
            sectionIndex >= sections.Length)
        {
            return;
        }

        currentSectionIndex = sectionIndex;

        SetContentParent(currentBoxNumber);

        for (int i = 0; i < sections.Length; i++)
        {
            if (sections[i] == null)
                continue;

            bool selected = i == sectionIndex;

            sections[i].SetActive(selected);

            if (groups != null &&
                i < groups.Length &&
                groups[i] != null)
            {
                groups[i].alpha = selected ? 1f : 0f;
                groups[i].interactable = selected;
                groups[i].blocksRaycasts = selected;
            }

            if (rects != null &&
                originalPositions != null &&
                i < rects.Length &&
                i < originalPositions.Length &&
                rects[i] != null)
            {
                rects[i].anchoredPosition =
                    originalPositions[i];
            }
        }

        UpdateNavigationButtonHighlight(sectionIndex);
    }

    // =========================================================
    // SWITCH LESSON SECTION
    // =========================================================

    private IEnumerator SwitchLessonSection(
        int boxNumber,
        int nextIndex)
    {
        transitionLocked = true;

        SetNavigationInteractable(false);

        GameObject[] sections =
            GetSectionsForBox(boxNumber);

        int previousIndex = currentSectionIndex;

        if (sections != null &&
            previousIndex >= 0 &&
            previousIndex < sections.Length &&
            previousIndex != nextIndex &&
            sections[previousIndex] != null &&
            sections[previousIndex].activeSelf)
        {
            yield return AnimateLessonSection(
                boxNumber,
                previousIndex,
                false
            );
        }

        currentSectionIndex = nextIndex;

        yield return AnimateLessonSection(
            boxNumber,
            nextIndex,
            true
        );

        UpdateNavigationButtonHighlight(nextIndex);

        transitionLocked = false;

        SetNavigationInteractable(true);

        sectionTransition = null;
    }

    private IEnumerator AnimateLessonSection(
        int boxNumber,
        int index,
        bool show)
    {
        GameObject[] sections =
            GetSectionsForBox(boxNumber);

        CanvasGroup[] groups =
            GetSectionCanvasGroups(boxNumber);

        RectTransform[] rects =
            GetSectionRectTransforms(boxNumber);

        Vector2[] originalPositions =
            GetSectionOriginalPositions(boxNumber);

        if (sections == null ||
            groups == null ||
            rects == null ||
            originalPositions == null ||
            index < 0 ||
            index >= sections.Length ||
            index >= groups.Length ||
            index >= rects.Length ||
            index >= originalPositions.Length ||
            sections[index] == null ||
            groups[index] == null)
        {
            yield break;
        }

        GameObject section = sections[index];

        CanvasGroup group = groups[index];

        RectTransform rect = rects[index];

        Vector2 home = originalPositions[index];

        float duration =
            Mathf.Max(0.05f, sectionTransitionDuration);

        if (show)
        {
            section.SetActive(true);

            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;

            if (rect != null)
            {
                rect.anchoredPosition =
                    home + new Vector2(panelSlideDistance, 0f);
            }
        }
        else
        {
            group.interactable = false;
            group.blocksRaycasts = false;
        }

        float startAlpha = group.alpha;
        float endAlpha = show ? 1f : 0f;

        Vector2 startPosition =
            rect != null ? rect.anchoredPosition : home;

        Vector2 endPosition = show
            ? home
            : home + new Vector2(-panelSlideDistance, 0f);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            float eased = Mathf.SmoothStep(0f, 1f, t);

            group.alpha =
                Mathf.Lerp(startAlpha, endAlpha, eased);

            if (rect != null)
            {
                rect.anchoredPosition =
                    Vector2.Lerp(
                        startPosition,
                        endPosition,
                        eased
                    );
            }

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

    // =========================================================
    // NAVIGATION INTERACTION
    // =========================================================

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

        if (interactable)
        {
            UpdateNavigationButtonHighlight(
                currentSectionIndex
            );
        }
    }

    // =========================================================
    // START BUTTON LABEL
    // =========================================================

    private void UpdateStartButtonLabel()
    {
        if (startButtonText == null)
            return;

        startButtonText.text = challengeHasStarted
            ? "BACK TO CHALLENGE"
            : "START";
    }

    // =========================================================
    // FREEZE PLAYER
    // =========================================================

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

    // =========================================================
    // START OR RESUME CHALLENGE
    // =========================================================

    private void StartOrResumeChallenge()
    {
        if (transitionLocked ||
            codingChallengeManager == null)
        {
            return;
        }

        if (flowTransition != null)
            StopCoroutine(flowTransition);

        flowTransition =
            StartCoroutine(StartChallengeTransition());
    }

    private IEnumerator StartChallengeTransition()
    {
        transitionLocked = true;

        SetNavigationInteractable(false);

        yield return AnimateInstructionPanel(false);

        bool started =
            codingChallengeManager != null &&
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

        Debug.Log(
            "BOX " + currentBoxNumber +
            " CHALLENGE START/RESUME REQUESTED.",
            this
        );
    }

    // =========================================================
    // VIEW INSTRUCTIONS DURING CHALLENGE
    // =========================================================

    private void OpenInstructionsForReview(int boxNumber)
    {
        if (transitionLocked ||
            currentBoxNumber != boxNumber ||
            !challengeHasStarted)
        {
            return;
        }

        if (codingChallengeManager == null)
        {
            Debug.LogError(
                "Room3CodingChallengeManager is not assigned.",
                this
            );

            return;
        }

        if (flowTransition != null)
            StopCoroutine(flowTransition);

        flowTransition =
            StartCoroutine(
                OpenInstructionsForReviewRoutine(boxNumber)
            );
    }

    private IEnumerator OpenInstructionsForReviewRoutine(
        int boxNumber)
    {
        transitionLocked = true;

        SetNavigationInteractable(false);

        FreezePlayerForInstructions();

        yield return codingChallengeManager
            .PauseSelectedChallengeForInstructions();

        UpdateHeaderTitle();
        UpdateStartButtonLabel();

        UpdateNavigationButtonHighlight(currentSectionIndex);

        yield return AnimateInstructionPanel(true);

        SetNavigationInteractable(true);

        transitionLocked = false;

        flowTransition = null;

        Debug.Log(
            "BOX " + boxNumber + " INSTRUCTIONS REOPENED.",
            this
        );
    }

    private void ReviewBox1Instructions()
    {
        OpenInstructionsForReview(1);
    }

    private void ReviewBox2Instructions()
    {
        OpenInstructionsForReview(2);
    }

    private void ReviewBox3Instructions()
    {
        OpenInstructionsForReview(3);
    }

    // =========================================================
    // INSTRUCTION PANEL TRANSITION
    // =========================================================

    private IEnumerator AnimateInstructionPanel(bool show)
    {
        if (instructionPanel == null ||
            instructionCanvasGroup == null)
        {
            yield break;
        }

        float duration =
            Mathf.Max(0.05f, panelTransitionDuration);

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
                instructionRect.localScale =
                    homeScale * panelStartScale;

                instructionRect.anchoredPosition =
                    homePosition +
                    new Vector2(0f, -panelSlideDistance);
            }
        }
        else
        {
            instructionCanvasGroup.interactable = false;
            instructionCanvasGroup.blocksRaycasts = false;
        }

        float startAlpha = instructionCanvasGroup.alpha;

        float endAlpha = show ? 1f : 0f;

        Vector3 startScale =
            instructionRect != null
                ? instructionRect.localScale
                : homeScale;

        Vector3 endScale = show
            ? homeScale
            : homeScale * panelStartScale;

        Vector2 startPosition =
            instructionRect != null
                ? instructionRect.anchoredPosition
                : homePosition;

        Vector2 endPosition = show
            ? homePosition
            : homePosition +
              new Vector2(0f, -panelSlideDistance);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            float eased = Mathf.SmoothStep(0f, 1f, t);

            instructionCanvasGroup.alpha =
                Mathf.Lerp(startAlpha, endAlpha, eased);

            if (instructionRect != null)
            {
                instructionRect.localScale =
                    Vector3.Lerp(startScale, endScale, eased);

                instructionRect.anchoredPosition =
                    Vector2.Lerp(
                        startPosition,
                        endPosition,
                        eased
                    );
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

    // =========================================================
    // RESET ROOM 3
    // =========================================================

    public void ResetForRoomRestart()
    {
        if (instructionPanelTransition != null)
            StopCoroutine(instructionPanelTransition);

        if (sectionTransition != null)
            StopCoroutine(sectionTransition);

        if (flowTransition != null)
            StopCoroutine(flowTransition);

        currentBoxNumber = 1;
        currentSectionIndex = 0;

        challengeHasStarted = false;
        transitionLocked = false;

        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);

            if (instructionCanvasGroup != null)
            {
                instructionCanvasGroup.alpha = 0f;
                instructionCanvasGroup.interactable = false;
                instructionCanvasGroup.blocksRaycasts = false;
            }

            if (instructionRect != null)
            {
                instructionRect.localScale =
                    instructionOriginalScale;

                instructionRect.anchoredPosition =
                    instructionOriginalPosition;
            }
        }

        SetContentParent(1);

        SetLessonSectionImmediate(0);

        UpdateHeaderTitle();
        UpdateStartButtonLabel();

        SetNavigationInteractable(true);
    }
}