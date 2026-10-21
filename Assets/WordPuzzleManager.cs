
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
    public int totalWords = 3;
    public TMP_Text progressText;
    public TMP_Text resultText;
    public TMP_Text clueText;

    private readonly string[] answers =
    {
        "DOMAIN",
        "MAIN",
        "ADMIN"
    };

    // Six letters per round, including distractor letters.
    private readonly string[] letterSets =
    {
        "DOMAIN",
        "MAINET",
        "ADMINE"
    };

    private readonly string[] clues =
    {
        "A name used to identify a website on the internet.",
        "The primary or central part of a system.",
        "A user with special control or management privileges."
    };

    private readonly List<GameObject> answerSlotObjects =
        new List<GameObject>();

    private readonly List<TMP_Text> answerTexts =
        new List<TMP_Text>();

    private readonly List<int> selectedButtonIndices =
        new List<int>();

    private int currentWordIndex;
    private int completedWords;
    private bool isComplete;

    private void Start()
    {
        totalWords = Mathf.Clamp(totalWords, 1, answers.Length);

        SetupAnswerSlots();
        SetupLetterButtons();
        SetupActionButtons();

        LoadCurrentWord();
        UpdateProgress();
        ShowResult("");
    }

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
                    "No TMP_Text found inside answer slot: "
                    + slot.name
                );
            }
        }

        if (answerSlotObjects.Count < 6)
        {
            Debug.LogError(
                "WordPuzzleManager needs 6 answer slots. Found: "
                + answerSlotObjects.Count
            );
        }
    }

    private void SetupLetterButtons()
    {
        if (letterButtons == null)
        {
            Debug.LogError(
                "WordPuzzleManager: Letter Buttons are not assigned."
            );
            return;
        }

        if (letterButtons.Length < 6)
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

    private void LoadCurrentWord()
    {
        if (currentWordIndex >= totalWords)
        {
            FinishPuzzle();
            return;
        }

        ClearAnswer();

        string answer = answers[currentWordIndex];
        char[] letters = letterSets[currentWordIndex].ToCharArray();

        if (letterButtons == null || letterButtons.Length < letters.Length)
        {
            Debug.LogError(
                "Not enough letter buttons for " + answer + "."
            );
            return;
        }

        // Shuffle the six letters.
        for (int i = letters.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            char temp = letters[i];
            letters[i] = letters[j];
            letters[j] = temp;
        }

        // Always display six letter buttons.
        for (int i = 0; i < letterButtons.Length; i++)
        {
            Button button = letterButtons[i];

            if (button == null)
                continue;

            if (i < letters.Length)
            {
                TMP_Text letterText =
                    button.GetComponentInChildren<TMP_Text>(true);

                button.gameObject.SetActive(true);
                button.interactable = true;

                if (letterText != null)
                    letterText.text = letters[i].ToString();
            }
            else
            {
                button.gameObject.SetActive(false);
            }
        }

        // Display only the answer slots required for this word.
        for (int i = 0; i < answerSlotObjects.Count; i++)
        {
            bool shouldShow = i < answer.Length;

            answerSlotObjects[i].SetActive(shouldShow);

            if (i < answerTexts.Count && answerTexts[i] != null)
                answerTexts[i].text = "";
        }

        if (clueText != null)
            clueText.text = "CLUE: " + clues[currentWordIndex];

        if (submitButton != null)
            submitButton.interactable = true;

        if (shuffleButton != null)
            shuffleButton.interactable = true;

        UpdateProgress();
        ShowResult("");
    }

    private void SelectLetter(int index)
    {
        if (isComplete || currentWordIndex >= totalWords)
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

        int requiredLetters = answers[currentWordIndex].Length;

        if (selectedButtonIndices.Count >= requiredLetters)
            return;

        selectedButtonIndices.Add(index);
        button.interactable = false;

        RefreshAnswerSlots();
        ShowResult("");
    }

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

    public void ShuffleLetters()
    {
        if (isComplete || currentWordIndex >= totalWords)
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

        // Shuffle the letters without hiding any of the six buttons.
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

    private void SubmitAnswer()
    {
        if (isComplete || currentWordIndex >= totalWords)
            return;

        int requiredLetters = answers[currentWordIndex].Length;

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
            if (index < 0 || index >= letterButtons.Length ||
                letterButtons[index] == null)
                return;

            TMP_Text text =
                letterButtons[index].GetComponentInChildren<TMP_Text>(true);

            if (text != null)
                playerAnswer += text.text;
        }

        if (playerAnswer == answers[currentWordIndex])
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
            ShowResult("Incorrect! Try again.");
        }
    }

    private void UpdateProgress()
    {
        if (progressText == null)
            return;

        if (isComplete)
        {
            progressText.text =
                completedWords + " / " + totalWords + " WORDS COMPLETED";
        }
        else
        {
            progressText.text =
                "WORD " + (currentWordIndex + 1) + " / " + totalWords;
        }
    }

    private void ShowResult(string message)
    {
        if (resultText != null)
            resultText.text = message;
    }

    private void FinishPuzzle()
    {
        isComplete = true;

        ClearAnswer();

        // Show all answer slots when the puzzle is complete.
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
    }
}
