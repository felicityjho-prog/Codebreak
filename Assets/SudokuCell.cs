using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SudokuCell : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField inputField;
    public Image cellBackground;

    [Header("Checker")]
    public TMP_Text checkerText;

    private int correctAnswer;
    private bool editable;

    private Color normalColor;

    public void Setup(int givenNumber, int answer, bool canEdit)
    {
        correctAnswer = answer;
        editable = canEdit;

        // Save original cell color
        if (cellBackground != null)
        {
            normalColor = cellBackground.color;
        }

        if (editable)
        {
            // Empty cell - player can type
            inputField.text = "";
            inputField.interactable = true;

            // Listen for Enter / Submit
            inputField.onEndEdit.RemoveAllListeners();
            inputField.onEndEdit.AddListener(CheckAnswer);

            if (checkerText != null)
            {
                checkerText.text = "";
            }
        }
        else
        {
            // Given number
            inputField.text = givenNumber.ToString();
            inputField.interactable = false;

            if (checkerText != null)
            {
                checkerText.text = "";
            }
        }
    }

    private void CheckAnswer(string playerInput)
    {
        // Ignore given cells
        if (!editable)
            return;

        // Empty input
        if (string.IsNullOrWhiteSpace(playerInput))
            return;

        int playerAnswer;

        // Check if input is a number
        if (!int.TryParse(playerInput, out playerAnswer))
        {
            WrongAnswer();
            return;
        }

        if (playerAnswer == correctAnswer)
        {
            CorrectAnswer();
        }
        else
        {
            WrongAnswer();
        }
    }

    private void CorrectAnswer()
    {
        // Green checker
        if (checkerText != null)
        {
            checkerText.text = "✓";
            checkerText.color = Color.green;
        }

        // Normal cell color
        if (cellBackground != null)
        {
            cellBackground.color = normalColor;
        }
    }

    private void WrongAnswer()
    {
        // Red cell
        if (cellBackground != null)
        {
            cellBackground.color = new Color(1f, 0.3f, 0.3f);
        }

        // Red X checker
        if (checkerText != null)
        {
            checkerText.text = "✗";
            checkerText.color = Color.red;
        }
    }
}