using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Room3CodingChallengeManager : MonoBehaviour
{
    private const int MaxHints = 3;

    private enum ChallengeType { None, Box1, Box2, Box3 }

    private ChallengeType activeChallenge = ChallengeType.None;
    private ChallengeType selectedChallenge = ChallengeType.None;

    [Header("Box 1 — If-Else Panel")]
    public GameObject codingPanel;

    [Header("Box 2 — While Loop Panel")]
    public GameObject codingPanelBox2;

    [Header("Box 3 — For Loop Panel")]
    public GameObject codingPanelBox3;

    [Header("Room 3 Instruction Manager")]
    public Room3InstructionManager instructionManager;

    [Header("Room 3 and Challenge Boxes")]
    public GameObject room3Root;
    public GameObject box1Object;
    public GameObject box2Object;
    public GameObject box3Object;
    public Camera playerCamera;

    [Min(0.5f)]
    public float interactionDistance = 5f;

    [Header("Player Controls")]
    public PlayerMovement playerMovement;
    public PlayerController playerController;

    [Header("Room 3 Timer")]
    public Room3ChallengeTimer room3Timer;

    [Header("Background Music")]
    [Tooltip("Assign the AudioSource that plays the background music across the rooms (for example, Room1Music).")]
    public AudioSource roomBackgroundMusic;

    [Header("Global Gameplay UI")]
    public GameObject centerDot;
    public GameObject controlsPanel;
    public GameObject pauseButton;
    public GameObject pauseMenu;

    [Header("Game Completion")]
    public GameObject gameCompletionPanel;
    public Button playAgainButton;
    public Button mainMenuButton;
    public Button giveFeedbackButton;
    public Button exitGameButton;

    public string mainMenuSceneName = "MainMenu";
    public string feedbackFormURL = "";

    [Header("Box 1 — If-Else Text")]
    public TMP_Text challengeTitle;
    public TMP_Text questionText;
    public TMP_Text codeText;
    public TMP_Text expectedOutputText;
    public TMP_InputField answerInput;
    public TMP_Text feedbackText;
    public TMP_Text hintMessageText;
    public TMP_Text hintsRemainingText;

    [Header("Box 1 — If-Else Buttons")]
    public Button submitButton;
    public Button hintButton;
    public Button closeButton;

    [Header("Box 2 — While Loop Text")]
    public TMP_Text box2ChallengeTitle;
    public TMP_Text box2QuestionText;
    public TMP_Text box2CodeText;
    public TMP_Text box2ExpectedOutputText;

    [Header("Box 2 — Answer Inputs")]
    public TMP_InputField box2AnswerInput1;
    public TMP_InputField box2AnswerInput2;

    [Header("Box 2 — Feedback and Hints")]
    public TMP_Text box2FeedbackText;
    public TMP_Text box2HintMessageText;
    public TMP_Text box2HintsRemainingText;

    [Header("Box 2 — Buttons")]
    public Button box2SubmitButton;
    public Button box2HintButton;
    public Button box2CloseButton;

    [Header("Box 2 — Correct Answer Field Color (Editable)")]
    public Color box2CorrectInputColor =
        new Color32(0, 168, 59, 255);

    [Header("Box 3 — For Loop Text")]
    public TMP_Text box3ChallengeTitle;
    public TMP_Text box3QuestionText;
    public TMP_Text box3CodeText;
    public TMP_Text box3ExpectedOutputText;

    [Header("Box 3 — Answer Inputs")]
    public TMP_InputField box3AnswerInput1;
    public TMP_InputField box3AnswerInput2;

    [Header("Box 3 — Feedback and Hints")]
    public TMP_Text box3FeedbackText;
    public TMP_Text box3HintMessageText;
    public TMP_Text box3HintsRemainingText;

    [Header("Box 3 — Buttons")]
    public Button box3SubmitButton;
    public Button box3HintButton;
    public Button box3CloseButton;

    [Header("Box 3 — Editable Hint Text")]
    [TextArea(1, 2)]
    public string box3InitialHintText = "Hints will appear here.";

    [TextArea(1, 3)]
    public string box3Hint1Text =
        "HINT 1: The loop variable is i, and it starts at 1.";

    [TextArea(1, 3)]
    public string box3Hint2Text =
        "HINT 2: The loop must include every value from 1 through 5.";

    [TextArea(1, 3)]
    public string box3Hint3Text =
        "HINT 3: After each pass, i should advance to the next integer.";

    [Header("Box 3 — Correct Answer Field Color")]
    public Color box3CorrectFieldColor =
        new Color(0.65f, 1f, 0.70f, 1f);

    [Header("Hint History UI")]
    public GameObject hintHistoryPanel;
    public TMP_Text hintHistoryListText;
    public Button closeHistoryButton;

    [Header("View Hint History Buttons")]
    public Button box1ViewHintsButton;
    public Button box2ViewHintsButton;
    public Button box3ViewHintsButton;

    private readonly List<string> box1HintHistory =
        new List<string>();

    private readonly List<string> box2HintHistory =
        new List<string>();

    private readonly List<string> box3HintHistory =
        new List<string>();

    private bool box1Completed;
    private bool box2Completed;
    private bool box3Completed;
    private bool gameCompleted;

    private bool box2ConditionCorrect;
    private bool box2IncrementCorrect;

    private bool box3ConditionCorrect;
    private bool box3IncrementCorrect;

    private int box1HintsUsed;
    private int box2HintsUsed;
    private int box3HintsUsed;

    private string box1CurrentHint = "Hints will appear here.";
    private string box2CurrentHint = "Hints will appear here.";
    private string box3CurrentHint = "Hints will appear here.";

    [Header("Hint Message Color")]
    public Color hintMessageColor = new Color(1f, 0.78f, 0.35f, 1f);

    private Color box2Input1OriginalColor = Color.white;
    private Color box2Input2OriginalColor = Color.white;

    private ColorBlock box2Input1OriginalColors;
    private ColorBlock box2Input2OriginalColors;

    private bool box2Input1ColorsCached;
    private bool box2Input2ColorsCached;

    private Color box3Input1OriginalColor = Color.white;
    private Color box3Input2OriginalColor = Color.white;

    private ColorBlock box3Input1OriginalColors;
    private ColorBlock box3Input2OriginalColors;

    private bool box3Input1ColorsCached;
    private bool box3Input2ColorsCached;

    // =====================================================
    // INITIALIZATION
    // =====================================================

    private void Awake()
    {
        CacheBox2InputColors();
        CacheBox3InputColors();

        SetPanelActive(codingPanel, false);
        SetPanelActive(codingPanelBox2, false);
        SetPanelActive(codingPanelBox3, false);
        SetPanelActive(gameCompletionPanel, false);
        SetPanelActive(hintHistoryPanel, false);

        ApplySelectedBoxVisibility();
    }

    private void OnEnable()
    {
        RegisterButton(submitButton, SubmitAnswer);
        RegisterButton(hintButton, UseHint);
        RegisterButton(closeButton, ClosePanel);

        RegisterButton(box2SubmitButton, SubmitAnswer);
        RegisterButton(box2HintButton, UseHint);
        RegisterButton(box2CloseButton, ClosePanel);

        RegisterButton(box3SubmitButton, SubmitAnswer);
        RegisterButton(box3HintButton, UseHint);
        RegisterButton(box3CloseButton, ClosePanel);

        RegisterButton(box1ViewHintsButton, ShowBox1HintHistory);
        RegisterButton(box2ViewHintsButton, ShowBox2HintHistory);
        RegisterButton(box3ViewHintsButton, ShowBox3HintHistory);

        RegisterButton(closeHistoryButton, CloseHintHistory);

        RegisterButton(playAgainButton, PlayAgain);
        RegisterButton(mainMenuButton, GoToMainMenu);
        RegisterButton(giveFeedbackButton, OpenFeedbackForm);
        RegisterButton(exitGameButton, ExitGame);

        AddAnswerSelectListener(answerInput);
        AddAnswerSelectListener(box2AnswerInput1);
        AddAnswerSelectListener(box2AnswerInput2);
        AddAnswerSelectListener(box3AnswerInput1);
        AddAnswerSelectListener(box3AnswerInput2);
    }

    private void OnDisable()
    {
        UnregisterButton(submitButton, SubmitAnswer);
        UnregisterButton(hintButton, UseHint);
        UnregisterButton(closeButton, ClosePanel);

        UnregisterButton(box2SubmitButton, SubmitAnswer);
        UnregisterButton(box2HintButton, UseHint);
        UnregisterButton(box2CloseButton, ClosePanel);

        UnregisterButton(box3SubmitButton, SubmitAnswer);
        UnregisterButton(box3HintButton, UseHint);
        UnregisterButton(box3CloseButton, ClosePanel);

        UnregisterButton(box1ViewHintsButton, ShowBox1HintHistory);
        UnregisterButton(box2ViewHintsButton, ShowBox2HintHistory);
        UnregisterButton(box3ViewHintsButton, ShowBox3HintHistory);

        UnregisterButton(closeHistoryButton, CloseHintHistory);

        UnregisterButton(playAgainButton, PlayAgain);
        UnregisterButton(mainMenuButton, GoToMainMenu);
        UnregisterButton(giveFeedbackButton, OpenFeedbackForm);
        UnregisterButton(exitGameButton, ExitGame);

        RemoveAnswerSelectListener(answerInput);
        RemoveAnswerSelectListener(box2AnswerInput1);
        RemoveAnswerSelectListener(box2AnswerInput2);
        RemoveAnswerSelectListener(box3AnswerInput1);
        RemoveAnswerSelectListener(box3AnswerInput2);
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

    private void AddAnswerSelectListener(TMP_InputField input)
    {
        if (input == null)
            return;

        input.onSelect.RemoveListener(OnAnswerSelected);
        input.onSelect.AddListener(OnAnswerSelected);
    }

    private void RemoveAnswerSelectListener(TMP_InputField input)
    {
        if (input != null)
            input.onSelect.RemoveListener(OnAnswerSelected);
    }

    private void Start()
    {
        // Get the editable initial hint from the Inspector.
        box3CurrentHint = box3InitialHintText;

        SetBox1Content();
        SetBox2Content();
        SetBox3Content();

        UpdateHintsRemainingUI();
        ValidateReferences();
    }

    // =====================================================
    // BOX INTERACTION
    // =====================================================

    private void Update()
    {
        if (gameCompleted ||
            room3Root == null ||
            !room3Root.activeInHierarchy)
        {
            return;
        }

        if (IsAnyCodingPanelOpen())
            return;

        if (gameCompletionPanel != null &&
            gameCompletionPanel.activeInHierarchy)
        {
            return;
        }

        if (instructionManager != null &&
            instructionManager.instructionPanel != null &&
            instructionManager.instructionPanel.activeInHierarchy)
        {
            return;
        }

        if (playerMovement != null &&
            !playerMovement.enabled)
        {
            return;
        }

        if (playerController != null &&
            !playerController.enabled)
        {
            return;
        }

        if (Cursor.visible ||
            Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        if (!Input.GetMouseButtonDown(0))
            return;

        Camera cam = playerCamera != null
            ? playerCamera
            : Camera.main;

        if (cam == null)
        {
            Debug.LogWarning(
                "Room3CodingChallengeManager: Player camera is missing.",
                this
            );

            return;
        }

        Ray ray = cam.ScreenPointToRay(
            new Vector3(
                Screen.width * 0.5f,
                Screen.height * 0.5f,
                0f
            )
        );

        if (!Physics.Raycast(
            ray,
            out RaycastHit hit,
            interactionDistance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Collide))
        {
            return;
        }

        Transform hitTransform = hit.collider.transform;

        if (IsHitObject(hitTransform, box1Object))
        {
            OpenChallenge(ChallengeType.Box1);
        }
        else if (IsHitObject(hitTransform, box2Object))
        {
            OpenChallenge(ChallengeType.Box2);
        }
        else if (IsHitObject(hitTransform, box3Object))
        {
            OpenChallenge(ChallengeType.Box3);
        }
    }

    private bool IsAnyCodingPanelOpen()
    {
        return IsPanelActive(codingPanel) ||
               IsPanelActive(codingPanelBox2) ||
               IsPanelActive(codingPanelBox3);
    }

    private bool IsPanelActive(GameObject panel)
    {
        return panel != null && panel.activeInHierarchy;
    }

    private void SetPanelActive(GameObject panel, bool active)
    {
        if (panel != null)
            panel.SetActive(active);
    }

    private bool IsHitObject(
        Transform hitTransform,
        GameObject target)
    {
        if (hitTransform == null ||
            target == null ||
            !target.activeInHierarchy)
        {
            return false;
        }

        return hitTransform == target.transform ||
               hitTransform.IsChildOf(target.transform);
    }

    private void ApplySelectedBoxVisibility()
    {
        switch (selectedChallenge)
        {
            case ChallengeType.Box1:
                SetPanelActive(box1Object, true);
                SetPanelActive(box2Object, false);
                SetPanelActive(box3Object, false);
                break;

            case ChallengeType.Box2:
                SetPanelActive(box1Object, false);
                SetPanelActive(box2Object, true);
                SetPanelActive(box3Object, false);
                break;

            case ChallengeType.Box3:
                SetPanelActive(box1Object, false);
                SetPanelActive(box2Object, false);
                SetPanelActive(box3Object, true);
                break;

            default:
                SetPanelActive(box1Object, true);
                SetPanelActive(box2Object, true);
                SetPanelActive(box3Object, true);
                break;
        }
    }

    // =====================================================
    // OPEN CHALLENGE — ONE BOX ONLY
    // =====================================================

    private void OpenChallenge(ChallengeType challenge)
    {
        if (gameCompleted)
            return;

        if (selectedChallenge != ChallengeType.None &&
            selectedChallenge != challenge)
        {
            Debug.LogWarning(
                "BOX " + (int)selectedChallenge +
                " IS ALREADY SELECTED. The other two boxes are locked. " +
                "Choose TRY AGAIN to select a different box.",
                this
            );

            return;
        }

        if (challenge == ChallengeType.Box1 && box1Completed)
            return;

        if (challenge == ChallengeType.Box2 && box2Completed)
            return;

        if (challenge == ChallengeType.Box3 && box3Completed)
            return;

        GameObject selectedPanel = GetPanel(challenge);

        if (selectedPanel == null)
        {
            Debug.LogError(
                "The coding panel for " + challenge +
                " is not assigned in the Inspector.",
                this
            );

            return;
        }

        if (selectedChallenge == ChallengeType.None)
        {
            selectedChallenge = challenge;

            Debug.Log(
                "BOX " + (int)selectedChallenge +
                " SELECTED. The other two boxes are now locked.",
                this
            );
        }

        ApplySelectedBoxVisibility();

        activeChallenge = challenge;

        SetPanelActive(hintHistoryPanel, false);

        SetPanelActive(
            codingPanel,
            challenge == ChallengeType.Box1
        );

        SetPanelActive(
            codingPanelBox2,
            challenge == ChallengeType.Box2
        );

        SetPanelActive(
            codingPanelBox3,
            challenge == ChallengeType.Box3
        );

        switch (challenge)
        {
            case ChallengeType.Box1:
                SetBox1Content();

                if (answerInput != null)
                    answerInput.interactable = !box1Completed;

                ShowFeedback(
                    "Enter your answer above. Try your best!",
                    Color.white
                );
                break;

            case ChallengeType.Box2:
                SetBox2Content();
                ApplyBox2InputStates();

                ShowFeedback(
                    "Complete both blanks, then click SUBMIT.",
                    Color.white
                );
                break;

            case ChallengeType.Box3:
                SetBox3Content();
                ApplyBox3InputStates();

                ShowFeedback(
                    "Complete both blanks, then click SUBMIT.",
                    Color.white
                );
                break;
        }

        UpdateHintsRemainingUI();

        SetPanelActive(centerDot, false);

        SetPlayerControls(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log(
            "BOX " + (int)challenge + " CODING PANEL OPENED.",
            this
        );
    }

    private GameObject GetPanel(ChallengeType challenge)
    {
        switch (challenge)
        {
            case ChallengeType.Box1:
                return codingPanel;

            case ChallengeType.Box2:
                return codingPanelBox2;

            case ChallengeType.Box3:
                return codingPanelBox3;

            default:
                return null;
        }
    }

    // =====================================================
    // BOX 1 CONTENT — IF-ELSE
    // =====================================================

    private void SetBox1Content()
    {
        // Title, code, and expected output remain editable in the Inspector.
        // Do not overwrite their rich-text formatting.

        if (questionText != null)
        {
            questionText.text =
                "A score of 75 or higher is passing. " +
                "Complete the missing condition so the program " +
                "displays \"Passed\".";
        }

        if (hintMessageText != null)
        {
            hintMessageText.text = box1CurrentHint;
            hintMessageText.color = hintMessageColor;
        }
    }

    // =====================================================
    // BOX 2 CONTENT — WHILE LOOP
    // =====================================================

    private void SetBox2Content()
    {
        // Title, code, and expected output remain editable in the Inspector.
        // Do not overwrite their rich-text formatting.

        if (box2QuestionText != null)
        {
            box2QuestionText.text =
                "Complete both missing parts so the loop prints " +
                "all even numbers from 2 to 6.";
        }

        if (box2HintMessageText != null)
        {
            box2HintMessageText.text = box2CurrentHint;
            box2HintMessageText.color = hintMessageColor;
        }

        SetPlaceholder(
            box2AnswerInput1,
            "Enter the loop condition..."
        );

        SetPlaceholder(
            box2AnswerInput2,
            "Enter the update statement..."
        );
    }

    // =====================================================
    // BOX 3 CONTENT — FOR LOOP
    // =====================================================

    private void SetBox3Content()
    {
        // IMPORTANT:
        // Do not assign text to box3ChallengeTitle, box3QuestionText,
        // box3CodeText, or box3ExpectedOutputText here.
        //
        // Those TMP objects are configured in the Unity Inspector.
        // This preserves your rich-text syntax colors when the game plays.

        if (box3HintMessageText != null)
        {
            box3HintMessageText.text = box3CurrentHint;
            box3HintMessageText.color = hintMessageColor;
        }

        SetPlaceholder(
            box3AnswerInput1,
            "Enter the loop condition..."
        );

        SetPlaceholder(
            box3AnswerInput2,
            "Enter the increment statement..."
        );
    }

    private void SetPlaceholder(
        TMP_InputField input,
        string placeholder)
    {
        if (input == null || input.placeholder == null)
            return;

        TMP_Text placeholderText = input.placeholder as TMP_Text;

        if (placeholderText != null)
            placeholderText.text = placeholder;
    }

    // =====================================================
    // SUBMIT ANSWERS
    // =====================================================

    public void SubmitAnswer()
    {
        if (gameCompleted)
            return;

        switch (activeChallenge)
        {
            case ChallengeType.Box1:
                SubmitBox1Answer();
                break;

            case ChallengeType.Box2:
                SubmitBox2Answer();
                break;

            case ChallengeType.Box3:
                SubmitBox3Answer();
                break;

            default:
                Debug.LogWarning(
                    "SubmitAnswer called without an active challenge.",
                    this
                );
                break;
        }
    }

    // =====================================================
    // BOX 1 ANSWER VALIDATION
    // =====================================================

    private void SubmitBox1Answer()
    {
        if (box1Completed)
            return;

        if (answerInput == null)
        {
            Debug.LogError(
                "Box 1 AnswerInput is not assigned.",
                this
            );
            return;
        }

        string answer = NormalizeAnswer(answerInput.text);

        bool correct =
            answer == "score>=75" ||
            answer == "75<=score" ||
            answer == "score>75" ||
            answer == "75<score";

        if (!correct)
        {
            bool isFailedAnswer =
                answer == "score<75" ||
                answer == "75>score" ||
                answer == "score<=75" ||
                answer == "75>=score";

            if (isFailedAnswer)
            {
                ShowFeedback(
                    "Incorrect! That condition would display \"Failed\".",
                    Color.red
                );
            }
            else
            {
                ShowFeedback(
                    "Incorrect. Try again, or use a hint.",
                    Color.red
                );
            }

            return;
        }

        box1Completed = true;
        box1CurrentHint = string.Empty;

        if (hintMessageText != null)
            hintMessageText.text = string.Empty;

        ShowFeedback("Correct! Box 1 completed.", Color.green);

        answerInput.interactable = false;

        if (submitButton != null)
            submitButton.interactable = false;

        if (hintButton != null)
            hintButton.interactable = false;

        UpdateHintsRemainingUI();
        CompleteGame();
    }

    // =====================================================
    // BOX 2 ANSWER VALIDATION
    // =====================================================

    private void SubmitBox2Answer()
    {
        if (box2Completed)
            return;

        if (box2AnswerInput1 == null ||
            box2AnswerInput2 == null)
        {
            Debug.LogError(
                "Both Box 2 answer input fields must be assigned.",
                this
            );
            return;
        }

        if (!box2ConditionCorrect)
        {
            string condition = NormalizeAnswer(box2AnswerInput1.text);

            box2ConditionCorrect =
                condition == "number<=6" ||
                condition == "number<7" ||
                condition == "6>=number" ||
                condition == "7>number";
        }

        if (!box2IncrementCorrect)
        {
            string increment = RemoveTrailingSemicolon(
                NormalizeAnswer(box2AnswerInput2.text)
            );

            box2IncrementCorrect =
                increment == "number+=2" ||
                increment == "number=number+2" ||
                increment == "number=2+number";
        }

        ApplyBox2InputStates();
        UpdateHintsRemainingUI();

        if (box2ConditionCorrect && box2IncrementCorrect)
        {
            box2Completed = true;
            box2CurrentHint = string.Empty;

            if (box2HintMessageText != null)
                box2HintMessageText.text = string.Empty;

            ShowFeedback("Correct! Box 2 completed.", Color.green);

            if (box2SubmitButton != null)
                box2SubmitButton.interactable = false;

            if (box2HintButton != null)
                box2HintButton.interactable = false;

            UpdateHintsRemainingUI();
            CompleteGame();
            return;
        }

        if (!box2ConditionCorrect && !box2IncrementCorrect)
        {
            ShowFeedback(
                "Both blanks are incorrect. Check the condition and update.",
                Color.red
            );
        }
        else if (!box2ConditionCorrect)
        {
            ShowFeedback(
                "Blank 1 is incorrect. Check when the loop should stop.",
                Color.red
            );
        }
        else
        {
            ShowFeedback(
                "Blank 2 is incorrect. Check how number changes each loop.",
                Color.red
            );
        }
    }

    // =====================================================
    // BOX 3 ANSWER VALIDATION
    // =====================================================

    private void SubmitBox3Answer()
    {
        if (box3Completed)
            return;

        if (box3AnswerInput1 == null ||
            box3AnswerInput2 == null)
        {
            Debug.LogError(
                "Both Box 3 answer input fields must be assigned.",
                this
            );
            return;
        }

        if (!box3ConditionCorrect)
        {
            string condition = NormalizeAnswer(box3AnswerInput1.text);

            box3ConditionCorrect =
                condition == "i<=5" ||
                condition == "i<6" ||
                condition == "5>=i" ||
                condition == "6>i";
        }

        if (!box3IncrementCorrect)
        {
            string increment = RemoveTrailingSemicolon(
                NormalizeAnswer(box3AnswerInput2.text)
            );

            box3IncrementCorrect =
                increment == "i++" ||
                increment == "++i" ||
                increment == "i+=1" ||
                increment == "i=i+1" ||
                increment == "i=1+i";
        }

        ApplyBox3InputStates();
        UpdateHintsRemainingUI();

        if (box3ConditionCorrect && box3IncrementCorrect)
        {
            box3Completed = true;
            box3CurrentHint = string.Empty;

            if (box3HintMessageText != null)
                box3HintMessageText.text = string.Empty;

            ShowFeedback("Correct! Box 3 completed.", Color.green);

            if (box3SubmitButton != null)
                box3SubmitButton.interactable = false;

            if (box3HintButton != null)
                box3HintButton.interactable = false;

            UpdateHintsRemainingUI();
            CompleteGame();
            return;
        }

        if (!box3ConditionCorrect && !box3IncrementCorrect)
        {
            ShowFeedback(
                "Both blanks are incorrect. Check the loop condition and increment.",
                Color.red
            );
        }
        else if (!box3ConditionCorrect)
        {
            ShowFeedback(
                "Blank 1 is incorrect. The loop must include 5.",
                Color.red
            );
        }
        else
        {
            ShowFeedback(
                "Blank 2 is incorrect. Move i to the next integer each iteration.",
                Color.red
            );
        }
    }

    // =====================================================
    // INPUT COLOR AND LOCKING
    // =====================================================

    private void CacheBox2InputColors()
    {
        CacheOneInput(
            box2AnswerInput1,
            ref box2Input1OriginalColor,
            ref box2Input1OriginalColors,
            ref box2Input1ColorsCached
        );

        CacheOneInput(
            box2AnswerInput2,
            ref box2Input2OriginalColor,
            ref box2Input2OriginalColors,
            ref box2Input2ColorsCached
        );
    }

    private void CacheBox3InputColors()
    {
        CacheOneInput(
            box3AnswerInput1,
            ref box3Input1OriginalColor,
            ref box3Input1OriginalColors,
            ref box3Input1ColorsCached
        );

        CacheOneInput(
            box3AnswerInput2,
            ref box3Input2OriginalColor,
            ref box3Input2OriginalColors,
            ref box3Input2ColorsCached
        );
    }

    private void CacheOneInput(
        TMP_InputField input,
        ref Color originalColor,
        ref ColorBlock originalColors,
        ref bool cached)
    {
        if (cached || input == null)
            return;

        originalColors = input.colors;

        originalColor = input.targetGraphic != null
            ? input.targetGraphic.color
            : Color.white;

        cached = true;
    }

    private void ApplyBox2InputStates()
    {
        CacheBox2InputColors();

        ApplyAnswerInputState(
            box2AnswerInput1,
            box2ConditionCorrect,
            box2Input1OriginalColor,
            box2Input1OriginalColors,
            box2Input1ColorsCached,
            box2CorrectInputColor
        );

        ApplyAnswerInputState(
            box2AnswerInput2,
            box2IncrementCorrect,
            box2Input2OriginalColor,
            box2Input2OriginalColors,
            box2Input2ColorsCached,
            box2CorrectInputColor
        );
    }

    private void ApplyBox3InputStates()
    {
        CacheBox3InputColors();

        ApplyAnswerInputState(
            box3AnswerInput1,
            box3ConditionCorrect,
            box3Input1OriginalColor,
            box3Input1OriginalColors,
            box3Input1ColorsCached,
            box3CorrectFieldColor
        );

        ApplyAnswerInputState(
            box3AnswerInput2,
            box3IncrementCorrect,
            box3Input2OriginalColor,
            box3Input2OriginalColors,
            box3Input2ColorsCached,
            box3CorrectFieldColor
        );
    }

    private void ApplyAnswerInputState(
        TMP_InputField input,
        bool correct,
        Color originalColor,
        ColorBlock originalColors,
        bool colorsCached,
        Color correctColor)
    {
        if (input == null)
            return;

        if (correct)
        {
            ColorBlock colors = input.colors;
            colors.normalColor = correctColor;
            colors.highlightedColor = correctColor;
            colors.pressedColor = correctColor;
            colors.selectedColor = correctColor;
            colors.disabledColor = correctColor;

            input.colors = colors;
            input.interactable = false;

            if (input.targetGraphic != null)
                input.targetGraphic.color = correctColor;
        }
        else
        {
            if (colorsCached)
                input.colors = originalColors;

            input.interactable = true;

            if (input.targetGraphic != null)
                input.targetGraphic.color = originalColor;
        }
    }

    // =====================================================
    // THREE-STEP HINT SYSTEM
    // =====================================================

    public void UseHint()
    {
        if (gameCompleted)
            return;

        switch (activeChallenge)
        {
            case ChallengeType.Box1:
                UseBox1Hint();
                break;

            case ChallengeType.Box2:
                UseBox2Hint();
                break;

            case ChallengeType.Box3:
                UseBox3Hint();
                break;
        }
    }

    private void UseBox1Hint()
    {
        ClearFeedback();

        if (box1Completed)
            return;

        if (box1HintsUsed >= MaxHints)
        {
            ShowFeedback(
                "No hints remaining. Think about the clues!",
                Color.red
            );
            return;
        }

        box1HintsUsed++;

        switch (box1HintsUsed)
        {
            case 1:
                box1CurrentHint =
                    "HINT 1: Look at the variable that stores " +
                    "the student's score. What is its name.";
                break;

            case 2:
                box1CurrentHint =
                    "HINT 2: The passing mark is 75. " +
                    "Your condition must compare the score with 75.";
                break;

            case 3:
                box1CurrentHint =
                    "HINT 3: A score of exactly 75 must pass too. " +
                    "Think of the comparison operator that includes " +
                    "values equal to the passing mark.";
                break;
        }

        box1HintHistory.Add(box1CurrentHint);

        if (hintMessageText != null)
        {
            hintMessageText.text = box1CurrentHint;
            hintMessageText.color = hintMessageColor;
        }

        UpdateHintsRemainingUI();
    }

    private void UseBox2Hint()
    {
        ClearFeedback();

        if (box2Completed)
            return;

        if (box2HintsUsed >= MaxHints)
        {
            ShowFeedback(
                "No hints remaining. Think about the clues!",
                Color.red
            );
            return;
        }

        box2HintsUsed++;

        switch (box2HintsUsed)
        {
            case 1:
                box2CurrentHint =
                    "HINT 1: The variable starts at 2 and changes " +
                    "after every loop iteration.";
                break;

            case 2:
                box2CurrentHint =
                    "HINT 2: The loop must include 2, 4 and 6, " +
                    "then stop before printing another number.";
                break;

            case 3:
                box2CurrentHint =
                    "HINT 3: The update must increase number by 2. " +
                    "The condition must still allow the value 6.";
                break;
        }

        box2HintHistory.Add(box2CurrentHint);

        if (box2HintMessageText != null)
        {
            box2HintMessageText.text = box2CurrentHint;
            box2HintMessageText.color = hintMessageColor;
        }

        UpdateHintsRemainingUI();
    }

    private void UseBox3Hint()
    {
        ClearFeedback();

        if (box3Completed)
            return;

        if (box3HintsUsed >= MaxHints)
        {
            ShowFeedback(
                "No hints remaining. Think about the clues!",
                Color.red
            );
            return;
        }

        box3HintsUsed++;

        // These strings can now be edited in the Unity Inspector.
        switch (box3HintsUsed)
        {
            case 1:
                box3CurrentHint = box3Hint1Text;
                break;

            case 2:
                box3CurrentHint = box3Hint2Text;
                break;

            case 3:
                box3CurrentHint = box3Hint3Text;
                break;
        }

        box3HintHistory.Add(box3CurrentHint);

        if (box3HintMessageText != null)
        {
            box3HintMessageText.text = box3CurrentHint;
            box3HintMessageText.color = hintMessageColor;
        }

        UpdateHintsRemainingUI();
    }

    private void UpdateHintsRemainingUI()
    {
        if (hintsRemainingText != null)
        {
            hintsRemainingText.text =
                "Hints Remaining: " +
                Mathf.Max(0, MaxHints - box1HintsUsed);
        }

        if (box2HintsRemainingText != null)
        {
            box2HintsRemainingText.text =
                "Hints Remaining: " +
                Mathf.Max(0, MaxHints - box2HintsUsed);
        }

        if (box3HintsRemainingText != null)
        {
            box3HintsRemainingText.text =
                "Hints Remaining: " +
                Mathf.Max(0, MaxHints - box3HintsUsed);
        }

        if (hintButton != null)
        {
            hintButton.interactable =
                !box1Completed &&
                box1HintsUsed < MaxHints;
        }

        if (box2HintButton != null)
        {
            box2HintButton.interactable =
                !box2Completed &&
                box2HintsUsed < MaxHints;
        }

        if (box3HintButton != null)
        {
            box3HintButton.interactable =
                !box3Completed &&
                box3HintsUsed < MaxHints;
        }
    }

    // =====================================================
    // HINT HISTORY
    // =====================================================

    private void ShowBox1HintHistory()
    {
        ShowHintHistory(ChallengeType.Box1);
    }

    private void ShowBox2HintHistory()
    {
        ShowHintHistory(ChallengeType.Box2);
    }

    private void ShowBox3HintHistory()
    {
        ShowHintHistory(ChallengeType.Box3);
    }

    private void ShowHintHistory(ChallengeType challenge)
    {
        if (hintHistoryPanel == null)
        {
            Debug.LogError(
                "HintHistoryPanel is not assigned.",
                this
            );
            return;
        }

        if (hintHistoryListText == null)
        {
            Debug.LogError(
                "HintHistoryListText is not assigned.",
                this
            );
            return;
        }

        List<string> history;
        string boxTitle;

        switch (challenge)
        {
            case ChallengeType.Box1:
                history = box1HintHistory;
                boxTitle = "BOX 1 — IF-ELSE";
                break;

            case ChallengeType.Box2:
                history = box2HintHistory;
                boxTitle = "BOX 2 — WHILE LOOP";
                break;

            case ChallengeType.Box3:
                history = box3HintHistory;
                boxTitle = "BOX 3 — FOR LOOP";
                break;

            default:
                return;
        }

        if (history.Count == 0)
        {
            hintHistoryListText.text =
                boxTitle +
                "\n\nNo hints used yet.\n\n" +
                "Use the HINT button during this challenge " +
                "to save clues here.";
        }
        else
        {
            hintHistoryListText.text =
                boxTitle + "\n\n" +
                string.Join("\n\n", history);
        }

        hintHistoryPanel.SetActive(true);
        hintHistoryPanel.transform.SetAsLastSibling();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseHintHistory()
    {
        if (hintHistoryPanel != null)
            hintHistoryPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // =====================================================
    // ANSWER NORMALIZATION
    // =====================================================

    private string NormalizeAnswer(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        return input.Replace(" ", string.Empty)
                    .Replace("\t", string.Empty)
                    .Replace("\r", string.Empty)
                    .Replace("\n", string.Empty);
    }

    private string RemoveTrailingSemicolon(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        return input.EndsWith(";")
            ? input.Substring(0, input.Length - 1)
            : input;
    }

    // =====================================================
    // FEEDBACK
    // =====================================================

    private void OnAnswerSelected(string selectedText)
    {
        ClearFeedback();
    }

    private void ClearFeedback()
    {
        if (feedbackText != null)
            feedbackText.text = string.Empty;

        if (box2FeedbackText != null)
            box2FeedbackText.text = string.Empty;

        if (box3FeedbackText != null)
            box3FeedbackText.text = string.Empty;
    }

    private void ShowFeedback(string message, Color color)
    {
        TMP_Text target = null;

        switch (activeChallenge)
        {
            case ChallengeType.Box1:
                target = feedbackText;
                break;

            case ChallengeType.Box2:
                target = box2FeedbackText;
                break;

            case ChallengeType.Box3:
                target = box3FeedbackText;
                break;
        }

        if (target != null)
        {
            target.text = message;
            target.color = color;
        }
    }

    // =====================================================
    // GAME COMPLETION
    // =====================================================

    private void CompleteGame()
    {
        if (gameCompleted)
            return;

        if (gameCompletionPanel == null)
        {
            Debug.LogError(
                "GameCompletionPanel is not assigned. " +
                "The final ending screen cannot be displayed.",
                this
            );
            return;
        }

        gameCompleted = true;

        // Stop the shared background music only after the final correct answer.
        // This runs once because CompleteGame returns early when gameCompleted is true.
        if (roomBackgroundMusic != null)
        {
            roomBackgroundMusic.Stop();
            Debug.Log("Background music stopped because CODEBREAK COMPLETE was triggered.", this);
        }
        else
        {
            Debug.LogWarning(
                "Background Music AudioSource is not assigned. Drag Room1Music's AudioSource into Room Background Music.",
                this
            );
        }

        if (room3Timer != null)
        {
            room3Timer.StopTimer();
            room3Timer.HideTimerDisplay();
        }

        HideCodingPanelsForGameOver();
        SetPanelActive(hintHistoryPanel, false);

        SetPanelActive(centerDot, false);
        SetPanelActive(controlsPanel, false);
        SetPanelActive(pauseButton, false);
        SetPanelActive(pauseMenu, false);

        SetPlayerControls(false);

        gameCompletionPanel.SetActive(true);
        gameCompletionPanel.transform.SetAsLastSibling();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log(
            "CODEBREAK COMPLETE: final ending panel shown.",
            this
        );
    }

    // =====================================================
    // ROOM 3 RESTART
    // =====================================================

    public void ResetForRoomRestart()
    {
        activeChallenge = ChallengeType.None;

        // Allow the player to choose a new box.
        selectedChallenge = ChallengeType.None;
        ApplySelectedBoxVisibility();

        gameCompleted = false;

        box1Completed = false;
        box2Completed = false;
        box3Completed = false;

        box2ConditionCorrect = false;
        box2IncrementCorrect = false;

        box3ConditionCorrect = false;
        box3IncrementCorrect = false;

        box1HintsUsed = 0;
        box2HintsUsed = 0;
        box3HintsUsed = 0;

        box1CurrentHint = "Hints will appear here.";
        box2CurrentHint = "Hints will appear here.";
        box3CurrentHint = box3InitialHintText;

        box1HintHistory.Clear();
        box2HintHistory.Clear();
        box3HintHistory.Clear();

        SetPanelActive(hintHistoryPanel, false);

        if (room3Timer != null)
            room3Timer.StopTimer();

        SetPanelActive(codingPanel, false);
        SetPanelActive(codingPanelBox2, false);
        SetPanelActive(codingPanelBox3, false);
        SetPanelActive(gameCompletionPanel, false);

        SetBox1Content();
        SetBox2Content();
        SetBox3Content();

        if (answerInput != null)
        {
            answerInput.text = string.Empty;
            answerInput.interactable = true;
        }

        if (box2AnswerInput1 != null)
        {
            box2AnswerInput1.text = string.Empty;
            box2AnswerInput1.interactable = true;
        }

        if (box2AnswerInput2 != null)
        {
            box2AnswerInput2.text = string.Empty;
            box2AnswerInput2.interactable = true;
        }

        if (box3AnswerInput1 != null)
        {
            box3AnswerInput1.text = string.Empty;
            box3AnswerInput1.interactable = true;
        }

        if (box3AnswerInput2 != null)
        {
            box3AnswerInput2.text = string.Empty;
            box3AnswerInput2.interactable = true;
        }

        ApplyBox2InputStates();
        ApplyBox3InputStates();

        if (submitButton != null)
            submitButton.interactable = true;

        if (closeButton != null)
            closeButton.interactable = true;

        if (hintButton != null)
            hintButton.interactable = true;

        if (box2SubmitButton != null)
            box2SubmitButton.interactable = true;

        if (box2CloseButton != null)
            box2CloseButton.interactable = true;

        if (box2HintButton != null)
            box2HintButton.interactable = true;

        if (box3SubmitButton != null)
            box3SubmitButton.interactable = true;

        if (box3CloseButton != null)
            box3CloseButton.interactable = true;

        if (box3HintButton != null)
            box3HintButton.interactable = true;

        ClearFeedback();
        UpdateHintsRemainingUI();

        SetPanelActive(centerDot, true);
        SetPanelActive(controlsPanel, true);
        SetPanelActive(pauseButton, true);
        SetPanelActive(pauseMenu, false);

        Debug.Log(
            "ROOM 3 CHALLENGE STATE RESET. The player can choose a new box.",
            this
        );
    }

    // =====================================================
    // GAME OVER SUPPORT
    // =====================================================

    public void HideCodingPanelsForGameOver()
    {
        SetPanelActive(codingPanel, false);
        SetPanelActive(codingPanelBox2, false);
        SetPanelActive(codingPanelBox3, false);
        SetPanelActive(hintHistoryPanel, false);

        // Keep the selected box locked until TRY AGAIN.
        activeChallenge = ChallengeType.None;
    }

    // =====================================================
    // PLAYER CONTROLS
    // =====================================================

    private void SetPlayerControls(bool enabledState)
    {
        if (playerMovement != null)
            playerMovement.enabled = enabledState;

        if (playerController != null)
        {
            if (enabledState)
            {
                playerController.enabled = true;
                playerController.EnableLook();
            }
            else
            {
                playerController.DisableLook();
                playerController.enabled = false;
            }
        }
    }

    // =====================================================
    // CLOSE CODING PANEL
    // =====================================================

    public void ClosePanel()
    {
        if (gameCompleted)
            return;

        HideCodingPanelsForGameOver();
        SetPanelActive(hintHistoryPanel, false);

        SetPanelActive(centerDot, true);

        SetPlayerControls(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log(
            "Coding panel closed. Selected box remains locked for this attempt.",
            this
        );
    }

    // =====================================================
    // GAME COMPLETION BUTTONS
    // =====================================================

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (string.IsNullOrWhiteSpace(mainMenuSceneName))
        {
            Debug.LogError(
                "Main Menu scene name is empty.",
                this
            );
            return;
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OpenFeedbackForm()
    {
        if (string.IsNullOrWhiteSpace(feedbackFormURL))
        {
            Debug.LogWarning(
                "Feedback Form URL is empty. Add your published Google Forms URL.",
                this
            );
            return;
        }

        Application.OpenURL(feedbackFormURL);
    }

    public void ExitGame()
    {
        Debug.Log("Exiting CodeBreak.");

        Time.timeScale = 1f;
        AudioListener.pause = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // =====================================================
    // REFERENCE VALIDATION
    // =====================================================

    private void ValidateReferences()
    {
        if (codingPanel == null)
            Debug.LogError("Assign CodingPanelBox1.", this);

        if (room3Root == null)
            Debug.LogError("Assign Room 3 Root.", this);

        if (box1Object == null)
            Debug.LogError("Assign Box 1 Object.", this);

        if (playerMovement == null)
            Debug.LogError("Assign PlayerMovement.", this);

        if (playerController == null)
            Debug.LogError("Assign PlayerController.", this);

        if (codeText == null)
            Debug.LogError("Assign Box 1 CodeText.", this);

        if (answerInput == null)
            Debug.LogError("Assign Box 1 AnswerInput.", this);

        if (gameCompletionPanel == null)
            Debug.LogError("Assign GameCompletionPanel.", this);

        if (codingPanelBox2 == null)
            Debug.LogWarning("Assign CodingPanelBox2.", this);

        if (box2Object == null)
            Debug.LogWarning("Assign Box 2 Object.", this);

        if (box2AnswerInput1 == null)
            Debug.LogWarning("Assign Box 2 AnswerInput1.", this);

        if (box2AnswerInput2 == null)
            Debug.LogWarning("Assign Box 2 AnswerInput2.", this);

        if (box2SubmitButton == null)
            Debug.LogWarning("Assign Box 2 SubmitButton.", this);

        if (box2HintButton == null)
            Debug.LogWarning("Assign Box 2 HintButton.", this);

        if (box2CloseButton == null)
            Debug.LogWarning("Assign Box 2 CloseButton.", this);

        if (codingPanelBox3 == null)
            Debug.LogWarning("Assign CodingPanelBox3.", this);

        if (box3Object == null)
            Debug.LogWarning("Assign Box 3 Object.", this);

        if (box3AnswerInput1 == null)
            Debug.LogWarning("Assign Box 3 AnswerInput1.", this);

        if (box3AnswerInput2 == null)
            Debug.LogWarning("Assign Box 3 AnswerInput2.", this);

        if (box3SubmitButton == null)
            Debug.LogWarning("Assign Box 3 SubmitButton.", this);

        if (box3HintButton == null)
            Debug.LogWarning("Assign Box 3 HintButton.", this);

        if (box3CloseButton == null)
            Debug.LogWarning("Assign Box 3 CloseButton.", this);

        if (hintHistoryPanel == null)
            Debug.LogWarning("Assign HintHistoryPanel.", this);

        if (hintHistoryListText == null)
            Debug.LogWarning("Assign HintHistoryListText.", this);

        if (closeHistoryButton == null)
            Debug.LogWarning("Assign CloseHistoryButton.", this);

        if (box1ViewHintsButton == null)
            Debug.LogWarning("Assign Box 1 ViewHintsButton.", this);

        if (box2ViewHintsButton == null)
            Debug.LogWarning("Assign Box 2 ViewHintsButton.", this);

        if (box3ViewHintsButton == null)
            Debug.LogWarning("Assign Box 3 ViewHintsButton.", this);
    }
}