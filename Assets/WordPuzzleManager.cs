
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

    private readonly List<string> selectedLetters = new List<string>();
    private readonly List<TMP_Text> answerTexts = new List<TMP_Text>();

    private void Start()
    {
        SetupAnswerSlots();
        SetupLetterButtons();
        SetupActionButtons();

        Debug.Log("Word Puzzle Manager is ready!");
    }

    private void SetupAnswerSlots()
    {
        answerTexts.Clear();

        if (answerSlotsArea == null)
        {
            Debug.LogWarning("Assign Answer Slots Area in the Inspector.");
            return;
        }

        foreach (Transform slot in answerSlotsArea)
        {
            TMP_Text text = slot.GetComponentInChildren<TMP_Text>(true);

            if (text != null)
            {
                answerTexts.Add(text);
                text.text = "";
            }
        }
    }

    private void SetupLetterButtons()
    {
        if (letterButtons == null)
            return;

        foreach (Button button in letterButtons)
        {
            if (button != null)
                button.onClick.RemoveAllListeners();
        }

        for (int i = 0; i < letterButtons.Length; i++)
        {
            int index = i;

            if (letterButtons[index] != null)
            {
                letterButtons[index].onClick.AddListener(
                    () => SelectLetter(index)
                );
            }
        }
    }

    private void SetupActionButtons()
    {
        if (clearButton != null)
            clearButton.onClick.AddListener(ClearAnswer);

        if (shuffleButton != null)
            shuffleButton.onClick.AddListener(ShuffleLetters);

        if (submitButton != null)
            submitButton.onClick.AddListener(SubmitAnswer);
    }

    private void SelectLetter(int index)
    {
        if (index < 0 || index >= letterButtons.Length)
            return;

        if (selectedLetters.Count >= answerTexts.Count)
            return;

        TMP_Text sourceText =
            letterButtons[index].GetComponentInChildren<TMP_Text>();

        if (sourceText == null)
            return;

        selectedLetters.Add(sourceText.text);
        RefreshAnswerSlots();

        Debug.Log("Selected letter: " + sourceText.text);
    }

    private void RefreshAnswerSlots()
    {
        for (int i = 0; i < answerTexts.Count; i++)
        {
            answerTexts[i].text =
                i < selectedLetters.Count ? selectedLetters[i] : "";
        }
    }

    public void ClearAnswer()
    {
        selectedLetters.Clear();
        RefreshAnswerSlots();
    }

    public void ShuffleLetters()
    {
        if (letterButtons == null || letterButtons.Length < 2)
            return;

        List<Vector2> positions = new List<Vector2>();

        foreach (Button button in letterButtons)
        {
            if (button != null)
                positions.Add(button.GetComponent<RectTransform>().anchoredPosition);
        }

        for (int i = positions.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            Vector2 temp = positions[i];
            positions[i] = positions[j];
            positions[j] = temp;
        }

        int positionIndex = 0;

        foreach (Button button in letterButtons)
        {
            if (button != null)
            {
                button.GetComponent<RectTransform>().anchoredPosition =
                    positions[positionIndex];

                positionIndex++;
            }
        }
    }

    private void SubmitAnswer()
    {
        string answer = string.Join("", selectedLetters);

        Debug.Log("Submitted answer: " + answer);
        Debug.Log("Answer checking will be configured with the puzzle words next.");
    }
}
