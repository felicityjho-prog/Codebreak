using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class WordPuzzleManager : MonoBehaviour
{
    [Header("Letter Wheel")]
    public Button[] letterButtons;

    [Header("Answer Slots")]
    public Transform answerSlotsArea;

    [Header("Buttons")]
    public Button clearButton;
    public Button shuffleButton;
    public Button submitButton;

    [Header("Puzzle Settings")]
    [Min(1)]
    public int totalWords = 3;

    public TMP_Text progressText;
    public TMP_Text resultText;
    public TMP_Text clueText;

    [Header("Room 2 Final Task Completion")]
    public GameObject room2TaskCompletePanel;
    public GameObject room2ChecklistUI;
    public RoomSwitcher room2RoomSwitcher;
    public Room2ChallengeTimer room2Timer;
    public Room2InstructionManager room2InstructionManager;
    public WordPuzzleInteraction wordPuzzleInteraction;

    [Header("Player Controls During Completion")]
    public PlayerMovement playerMovement;
    public PlayerController playerController;

    // =========================================================
    // ICT WORD BANK
    // All answers fit the existing six-letter wheel.
    // =========================================================

    private class PuzzleWord
    {
        public readonly string answer;
        public readonly string clue;

        public PuzzleWord(string answer, string clue)
        {
            this.answer = answer;
            this.clue = clue;
        }
    }

    private static readonly PuzzleWord[] wordBank =
{
    new PuzzleWord("CPU",
        "The main part of a computer that processes instructions."),

    new PuzzleWord("IDE",
        "A tool developers use to write, run, and debug code."),

    new PuzzleWord("HTML",
        "A language used to create the structure of web pages."),

    new PuzzleWord("WIFI",
        "Lets devices connect to the internet without a cable."),

    new PuzzleWord("ARRAY",
        "A collection of values stored under one variable name."),

    new PuzzleWord("DEBUG",
        "The process of finding and fixing errors in a program."),
};

    // =========================================================
    // RUNTIME DATA
    // =========================================================

    private readonly List<PuzzleWord> roundWords =
        new List<PuzzleWord>();

    private readonly List<GameObject> answerSlotObjects =
        new List<GameObject>();

    private readonly List<TMP_Text> answerTexts =
        new List<TMP_Text>();

    private readonly List<int> selectedButtonIndices =
        new List<int>();

    private static string previousRoundSignature = "";

    private int currentWordIndex;
    private int completedWords;

    private bool isComplete;
    private bool isInitialized;
    private bool hasStartedRound;
    private bool room2CompletionShown;

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        InitializeIfNeeded();
        ResolveOptionalReferences();

        if (!hasStartedRound)
        {
            StartNewRound();
        }
    }

    private void ResolveOptionalReferences()
    {
        if (room2Timer == null)
            room2Timer = Room2ChallengeTimer.Instance;

        if (wordPuzzleInteraction == null)
        {
            wordPuzzleInteraction =
                FindFirstObjectByType<WordPuzzleInteraction>(
                    FindObjectsInactive.Include
                );
        }

        if (room2InstructionManager == null)
        {
            room2InstructionManager =
                FindFirstObjectByType<Room2InstructionManager>(
                    FindObjectsInactive.Include
                );
        }

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            if (playerMovement == null)
            {
                playerMovement =
                    playerObject.GetComponent<PlayerMovement>();
            }

            if (playerController == null)
            {
                playerController =
                    playerObject.GetComponent<PlayerController>();
            }
        }
    }

    private void InitializeIfNeeded()
    {
        if (isInitialized)
            return;

        SetupAnswerSlots();
        SetupLetterButtons();
        SetupActionButtons();

        isInitialized = true;
    }

    // =========================================================
    // RESTART RANDOM ROUND
    // Can be called by an existing Try Again button.
    // =========================================================

    public void RestartPuzzle()
    {
        InitializeIfNeeded();
        ResolveOptionalReferences();

        StartNewRound();

        Debug.Log("WORD SCRABBLE: New random round started.");
    }

    private void StartNewRound()
    {
        currentWordIndex = 0;
        completedWords = 0;
        isComplete = false;
        room2CompletionShown = false;

        selectedButtonIndices.Clear();

        SelectRandomRoundWords();

        totalWords = roundWords.Count;

        if (clearButton != null)
            clearButton.interactable = true;

        if (shuffleButton != null)
            shuffleButton.interactable = true;

        if (submitButton != null)
            submitButton.interactable = true;

        LoadCurrentWord();
        UpdateProgress();
        ShowResult("");

        hasStartedRound = true;
    }

    // =========================================================
    // RANDOM WORD SELECTION
    // Selects unique answers for the current round.
    // =========================================================

    private void SelectRandomRoundWords()
    {
        roundWords.Clear();

        List<PuzzleWord> shuffledBank =
            new List<PuzzleWord>(wordBank);

        ShuffleList(shuffledBank);

        int requestedWords = Mathf.Clamp(
            totalWords,
            1,
            wordBank.Length
        );

        List<PuzzleWord> chosenWords =
            shuffledBank.GetRange(0, requestedWords);

        string signature = BuildSignature(chosenWords);

        // Try not to use the exact same ordered round twice.
        for (int attempt = 0;
             signature == previousRoundSignature && attempt < 10;
             attempt++)
        {
            shuffledBank = new List<PuzzleWord>(wordBank);
            ShuffleList(shuffledBank);

            chosenWords =
                shuffledBank.GetRange(0, requestedWords);

            signature = BuildSignature(chosenWords);
        }

        // If the same sequence still occurs, change its order.
        if (signature == previousRoundSignature &&
            chosenWords.Count > 1)
        {
            int lastIndex = chosenWords.Count - 1;

            PuzzleWord temp = chosenWords[0];
            chosenWords[0] = chosenWords[lastIndex];
            chosenWords[lastIndex] = temp;
        }
        else if (signature == previousRoundSignature &&
                 chosenWords.Count == 1)
        {
            // If there is only one question, select a different one.
            foreach (PuzzleWord candidate in wordBank)
            {
                if (candidate.answer != chosenWords[0].answer)
                {
                    chosenWords[0] = candidate;
                    break;
                }
            }
        }

        roundWords.AddRange(chosenWords);

        previousRoundSignature = BuildSignature(roundWords);

        Debug.Log(
            "Word Scrabble selected " +
            roundWords.Count +
            " random words for this round."
        );
    }

    private string BuildSignature(List<PuzzleWord> words)
    {
        string signature = "";

        for (int i = 0; i < words.Count; i++)
        {
            if (i > 0)
                signature += "|";

            signature += words[i].answer;
        }

        return signature;
    }

    private void ShuffleList<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    // =========================================================
    // SETUP ANSWER SLOTS
    // =========================================================

    private void SetupAnswerSlots()
    {
        answerSlotObjects.Clear();
        answerTexts.Clear();

        if (answerSlotsArea == null)
        {
            Debug.LogError(
                "WordPuzzleManager: Answer Slots Area is not assigned."
            );
            return;
        }

        foreach (Transform slot in answerSlotsArea)
        {
            TMP_Text text =
                slot.GetComponentInChildren<TMP_Text>(true);

            answerSlotObjects.Add(slot.gameObject);
            answerTexts.Add(text);

            if (text == null)
            {
                Debug.LogWarning(
                    "No TMP_Text found inside answer slot: " +
                    slot.name
                );
            }
        }

        if (answerSlotObjects.Count < 6)
        {
            Debug.LogError(
                "WordPuzzleManager needs at least 6 answer slots. Found: " +
                answerSlotObjects.Count
            );
        }
    }

    // =========================================================
    // SETUP LETTER BUTTONS
    // =========================================================

    private void SetupLetterButtons()
    {
        if (letterButtons == null || letterButtons.Length < 6)
        {
            Debug.LogError(
                "WordPuzzleManager needs at least 6 letter buttons."
            );
            return;
        }

        for (int i = 0; i < letterButtons.Length; i++)
        {
            if (letterButtons[i] == null)
            {
                Debug.LogError(
                    "Letter Button Element " + i + " is not assigned."
                );
                continue;
            }

            int index = i;

            letterButtons[index].onClick.AddListener(
                () => SelectLetter(index)
            );
        }
    }

    // =========================================================
    // SETUP ACTION BUTTONS
    // =========================================================

    private void SetupActionButtons()
    {
        if (clearButton != null)
            clearButton.onClick.AddListener(ClearAnswer);
        else
            Debug.LogWarning("Clear Button is not assigned.");

        if (shuffleButton != null)
            shuffleButton.onClick.AddListener(ShuffleLetters);
        else
            Debug.LogWarning("Shuffle Button is not assigned.");

        if (submitButton != null)
            submitButton.onClick.AddListener(SubmitAnswer);
        else
            Debug.LogWarning("Submit Button is not assigned.");
    }

    // =========================================================
    // LOAD CURRENT WORD
    // =========================================================

    private void LoadCurrentWord()
    {
        if (currentWordIndex >= roundWords.Count)
        {
            FinishPuzzle();
            return;
        }

        if (letterButtons == null || letterButtons.Length < 6)
        {
            Debug.LogError(
                "WordPuzzleManager: Letter Buttons are missing."
            );
            return;
        }

        PuzzleWord currentWord =
            roundWords[currentWordIndex];

        string answer = currentWord.answer;

        if (answer.Length > letterButtons.Length)
        {
            Debug.LogError(
                "Not enough letter buttons for answer: " + answer
            );
            return;
        }

        if (answer.Length > answerSlotObjects.Count)
        {
            Debug.LogError(
                "Not enough answer slots for answer: " + answer
            );
            return;
        }

        ClearAnswer();

        // Add random distractor letters to fill the wheel.
        List<char> letters =
            new List<char>(answer.ToCharArray());

        while (letters.Count < letterButtons.Length)
        {
            char randomLetter =
                (char)('A' + Random.Range(0, 26));

            letters.Add(randomLetter);
        }

        ShuffleList(letters);

        for (int i = 0; i < letterButtons.Length; i++)
        {
            Button button = letterButtons[i];

            if (button == null)
                continue;

            if (i < letters.Count)
            {
                TMP_Text letterText =
                    button.GetComponentInChildren<TMP_Text>(true);

                button.gameObject.SetActive(true);
                button.interactable = true;

                if (letterText != null)
                {
                    letterText.text = letters[i].ToString();
                }
            }
            else
            {
                button.gameObject.SetActive(false);
            }
        }

        // Show only the slots needed for the current answer.
        for (int i = 0; i < answerSlotObjects.Count; i++)
        {
            bool shouldShow = i < answer.Length;

            answerSlotObjects[i].SetActive(shouldShow);

            if (i < answerTexts.Count && answerTexts[i] != null)
            {
                answerTexts[i].text = "";
            }
        }

        if (clueText != null)
            clueText.text = "CLUE: " + currentWord.clue;

        if (submitButton != null)
            submitButton.interactable = true;

        if (shuffleButton != null)
            shuffleButton.interactable = true;

        if (clearButton != null)
            clearButton.interactable = true;

        UpdateProgress();
        ShowResult("");
    }

    // =========================================================
    // SELECT LETTER
    // =========================================================

    private void SelectLetter(int index)
    {
        if (isComplete || currentWordIndex >= roundWords.Count)
            return;

        if (letterButtons == null ||
            index < 0 ||
            index >= letterButtons.Length)
            return;

        Button button = letterButtons[index];

        if (button == null ||
            !button.gameObject.activeSelf ||
            !button.interactable)
            return;

        int requiredLetters =
            roundWords[currentWordIndex].answer.Length;

        if (selectedButtonIndices.Count >= requiredLetters)
            return;

        selectedButtonIndices.Add(index);
        button.interactable = false;

        RefreshAnswerSlots();
        ShowResult("");
    }

    // =========================================================
    // REFRESH ANSWER SLOTS
    // =========================================================

    private void RefreshAnswerSlots()
    {
        for (int i = 0; i < answerTexts.Count; i++)
        {
            TMP_Text answerText = answerTexts[i];

            if (answerText == null)
                continue;

            if (i < selectedButtonIndices.Count)
            {
                int buttonIndex = selectedButtonIndices[i];

                if (letterButtons == null ||
                    buttonIndex < 0 ||
                    buttonIndex >= letterButtons.Length ||
                    letterButtons[buttonIndex] == null)
                {
                    answerText.text = "";
                    continue;
                }

                TMP_Text sourceText =
                    letterButtons[buttonIndex]
                    .GetComponentInChildren<TMP_Text>(true);

                answerText.text =
                    sourceText != null ? sourceText.text : "";
            }
            else
            {
                answerText.text = "";
            }
        }
    }

    // =========================================================
    // CLEAR ANSWER
    // =========================================================

    public void ClearAnswer()
    {
        selectedButtonIndices.Clear();

        if (letterButtons != null)
        {
            foreach (Button button in letterButtons)
            {
                if (button != null && button.gameObject.activeSelf)
                    button.interactable = true;
            }
        }

        RefreshAnswerSlots();
    }

    // =========================================================
    // SHUFFLE LETTERS
    // =========================================================

    public void ShuffleLetters()
    {
        if (isComplete || currentWordIndex >= roundWords.Count)
            return;

        if (letterButtons == null)
            return;

        ClearAnswer();

        List<Button> activeButtons = new List<Button>();
        List<string> letters = new List<string>();

        foreach (Button button in letterButtons)
        {
            if (button == null || !button.gameObject.activeSelf)
                continue;

            TMP_Text text =
                button.GetComponentInChildren<TMP_Text>(true);

            if (text == null)
                continue;

            activeButtons.Add(button);
            letters.Add(text.text);
        }

        for (int i = letters.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            string temp = letters[i];
            letters[i] = letters[j];
            letters[j] = temp;
        }

        for (int i = 0; i < activeButtons.Count; i++)
        {
            TMP_Text text =
                activeButtons[i].GetComponentInChildren<TMP_Text>(true);

            if (text != null)
                text.text = letters[i];
        }

        ShowResult("");
    }

    // =========================================================
    // SUBMIT ANSWER
    // =========================================================

    private void SubmitAnswer()
    {
        if (isComplete || currentWordIndex >= roundWords.Count)
            return;

        int requiredLetters =
            roundWords[currentWordIndex].answer.Length;

        if (selectedButtonIndices.Count != requiredLetters)
        {
            ShowResult(
                "Select all " + requiredLetters + " letters first!"
            );
            return;
        }

        string playerAnswer = "";

        foreach (int index in selectedButtonIndices)
        {
            if (index < 0 ||
                index >= letterButtons.Length ||
                letterButtons[index] == null)
                return;

            TMP_Text text =
                letterButtons[index].GetComponentInChildren<TMP_Text>(true);

            if (text != null)
                playerAnswer += text.text;
        }

        string correctAnswer =
            roundWords[currentWordIndex].answer;

        if (playerAnswer == correctAnswer)
        {
            completedWords++;
            currentWordIndex++;

            if (completedWords >= totalWords)
            {
                FinishPuzzle();
            }
            else
            {
                LoadCurrentWord();
                ShowResult("Correct! Next word.");
            }
        }
        else
        {
            // No time penalty is added here.
            ShowResult("Incorrect! Try again.");
        }
    }

    // =========================================================
    // PROGRESS
    // =========================================================

    private void UpdateProgress()
    {
        if (progressText == null)
            return;

        if (isComplete)
        {
            progressText.text =
                completedWords + " / " +
                totalWords + " WORDS COMPLETED";
        }
        else
        {
            // Show completed words instead of the current word index.
            progressText.text =
                "WORD " + completedWords +
                " / " + totalWords;
        }
    }

    // =========================================================
    // RESULT MESSAGE
    // =========================================================

    private void ShowResult(string message)
    {
        if (resultText != null)
            resultText.text = message;
    }

    // =========================================================
    // FINISH PUZZLE
    // =========================================================

    private void FinishPuzzle()
    {
        if (isComplete)
            return;

        isComplete = true;

        ClearAnswer();

        foreach (GameObject slot in answerSlotObjects)
        {
            if (slot != null)
                slot.SetActive(true);
        }

        UpdateProgress();
        ShowResult("ALL WORDS COMPLETED!");

        if (letterButtons != null)
        {
            foreach (Button button in letterButtons)
            {
                if (button != null)
                    button.interactable = false;
            }
        }

        if (submitButton != null)
            submitButton.interactable = false;

        if (shuffleButton != null)
            shuffleButton.interactable = false;

        if (clearButton != null)
            clearButton.interactable = false;

        Debug.Log("All Word Scrabble challenges completed!");

        // Register Table 3 completion.
        ChallengeProgressManager progress =
            ChallengeProgressManager.Instance;

        if (progress == null)
        {
            Debug.LogError(
                "WordPuzzleManager: ChallengeProgressManager.Instance " +
                "is missing. Table 3 completion could not be registered."
            );
            return;
        }

        progress.CompleteTable(3);

        // Check whether all three Room 2 tables are completed.
        bool allTablesCompleted =
            progress.IsTableCompleted(1) &&
            progress.IsTableCompleted(2) &&
            progress.IsTableCompleted(3);

        if (allTablesCompleted)
        {
            ShowRoom2TaskCompletion();
        }
        else
        {
            Debug.LogWarning(
                "Word Scrabble is complete, but not all Room 2 tables " +
                "are registered as completed. Check Tables 1 and 2."
            );
        }
    }

    // =========================================================
    // ROOM 2 FINAL TASK COMPLETION
    // =========================================================

    private void ShowRoom2TaskCompletion()
    {
        if (room2CompletionShown)
            return;

        room2CompletionShown = true;

        // Resolve timer if not assigned.
        if (room2Timer == null)
            room2Timer = Room2ChallengeTimer.Instance;

        // Stop the shared Room 2 timer.
        if (room2Timer != null)
            room2Timer.StopTimer();

        // Hide Room 2 checklist and instruction.
        if (room2ChecklistUI != null)
            room2ChecklistUI.SetActive(false);

        if (room2InstructionManager != null)
            room2InstructionManager.HideInstruction();

        // Close Word Scrabble and reset its interaction state.
        if (wordPuzzleInteraction == null)
        {
            wordPuzzleInteraction =
                FindFirstObjectByType<WordPuzzleInteraction>(
                    FindObjectsInactive.Include
                );
        }

        if (wordPuzzleInteraction != null)
        {
            wordPuzzleInteraction.CloseWordPuzzle();
        }
        else
        {
            Debug.LogWarning(
                "WordPuzzleInteraction was not found. " +
                "Check the Table 3 interaction reference."
            );
        }

        // Freeze player controls during completion.
        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerController != null)
            playerController.DisableLook();

        // Show the assigned Task Completion panel.
        if (room2TaskCompletePanel != null)
        {
            Canvas parentCanvas =
                room2TaskCompletePanel.GetComponentInParent<Canvas>(true);

            if (parentCanvas != null &&
                !parentCanvas.gameObject.activeSelf)
            {
                parentCanvas.gameObject.SetActive(true);
            }

            room2TaskCompletePanel.SetActive(true);
            room2TaskCompletePanel.transform.SetAsLastSibling();

            CanvasGroup canvasGroup =
                room2TaskCompletePanel.GetComponent<CanvasGroup>();

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }
        else
        {
            Debug.LogWarning(
                "Room 2 Task Complete Panel is not assigned in the " +
                "WordPuzzleManager Inspector."
            );
        }

        // Unlock E-to-proceed on RoomManager2.
        if (room2RoomSwitcher != null)
        {
            room2RoomSwitcher.EnableProceed();
        }
        else
        {
            Debug.LogWarning(
                "Room 2 RoomSwitcher is not assigned. " +
                "Assign RoomManager2 so E can proceed to Room 3."
            );
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log(
            "ROOM 2 COMPLETED: All three tables are finished. " +
            "Task Completion panel shown; E-to-proceed unlocked."
        );
    }
}