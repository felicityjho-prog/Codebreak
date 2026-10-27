using UnityEngine;
using TMPro;

public class SudokuCell : MonoBehaviour
{
    [Header("Input Field")]
    public TMP_InputField inputField;

    private int givenNumber;
    private int correctAnswer;
    private bool editable;

    private SudokuManager sudokuManager;

    // =========================================================
    // INITIALIZE
    // =========================================================

    private void Awake()
    {
        FindInputField();
        PrepareInputField();
    }

    // =========================================================
    // FIND INPUT FIELD
    // =========================================================

    private void FindInputField()
    {
        if (inputField == null)
        {
            inputField = GetComponentInChildren<TMP_InputField>(true);
        }

        if (inputField == null)
        {
            Debug.LogError("TMP_InputField NOT FOUND on SudokuCell!", this);
        }
    }

    // =========================================================
    // PREPARE INPUT FIELD
    // =========================================================

    private void PrepareInputField()
    {
        if (inputField == null)
            return;

        inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
        inputField.onValidateInput = ValidateSudokuInput;
        inputField.characterLimit = 1;

        if (inputField.textComponent != null)
        {
            inputField.textComponent.gameObject.SetActive(true);
            inputField.textComponent.color = Color.black;
            inputField.textComponent.fontSize = 36;
            inputField.textComponent.alignment = TextAlignmentOptions.Center;
            inputField.textComponent.fontStyle = FontStyles.Bold;
        }
    }

    // =========================================================
    // VALIDATE INPUT: ALLOW ONLY NUMBERS 1 TO 6
    // =========================================================

    private char ValidateSudokuInput(string text, int charIndex, char addedChar)
    {
        if (addedChar >= '1' && addedChar <= '6')
            return addedChar;

        return '\0';
    }

    // =========================================================
    // SETUP CELL
    // =========================================================

    public void Setup(int given, int answer, bool canEdit, SudokuManager manager)
    {
        givenNumber = given;
        correctAnswer = answer;
        editable = canEdit;
        sudokuManager = manager;

        FindInputField();

        if (inputField == null)
        {
            Debug.LogError("TMP_InputField NOT FOUND!", this);
            return;
        }

        PrepareInputField();

        // GIVEN NUMBER: CANNOT BE EDITED
        if (!editable)
        {
            inputField.onEndEdit.RemoveListener(CheckAnswer);
            inputField.SetTextWithoutNotify(givenNumber.ToString());
            inputField.interactable = false;
            inputField.readOnly = true;

            if (inputField.image != null)
                inputField.image.color = Color.white;

            if (inputField.textComponent != null)
            {
                inputField.textComponent.gameObject.SetActive(true);
                inputField.textComponent.text = givenNumber.ToString();
                inputField.textComponent.color = Color.black;
                inputField.textComponent.fontSize = 36;
                inputField.textComponent.alignment = TextAlignmentOptions.Center;
                inputField.textComponent.fontStyle = FontStyles.Bold;
            }
        }
        // PLAYER CELL: EDITABLE
        else
        {
            inputField.onEndEdit.RemoveListener(CheckAnswer);
            inputField.SetTextWithoutNotify(string.Empty);
            inputField.interactable = true;
            inputField.readOnly = false;
            inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
            inputField.characterLimit = 1;
            inputField.onValidateInput = ValidateSudokuInput;

            if (inputField.image != null)
                inputField.image.color = Color.white;

            if (inputField.textComponent != null)
            {
                inputField.textComponent.gameObject.SetActive(true);
                inputField.textComponent.color = Color.black;
                inputField.textComponent.fontSize = 36;
                inputField.textComponent.alignment = TextAlignmentOptions.Center;
                inputField.textComponent.fontStyle = FontStyles.Normal;
            }

            inputField.onEndEdit.AddListener(CheckAnswer);
        }
    }

    // =========================================================
    // CHECK PLAYER ANSWER
    // =========================================================

    private void CheckAnswer(string playerAnswer)
    {
        if (!editable)
            return;

        if (string.IsNullOrEmpty(playerAnswer))
            return;

        if (!int.TryParse(playerAnswer, out int playerNumber))
            return;

        if (playerNumber < 1 || playerNumber > 6)
            return;

        if (playerNumber == correctAnswer)
        {
            Debug.Log("CORRECT Sudoku answer: " + playerNumber, this);

            if (inputField.image != null)
                inputField.image.color = Color.green;

            if (inputField.textComponent != null)
                inputField.textComponent.color = Color.black;

            if (sudokuManager != null)
                sudokuManager.CheckSudokuComplete();
        }
        else
        {
            Debug.Log("WRONG Sudoku answer: " + playerNumber, this);

            if (inputField.image != null)
                inputField.image.color = Color.red;

            if (inputField.textComponent != null)
                inputField.textComponent.color = Color.black;

            if (sudokuManager != null)
                sudokuManager.PlayWrongAnswerSound();

            if (Room2ChallengeTimer.Instance != null)
                Room2ChallengeTimer.Instance.WrongAnswer();
        }
    }

    // =========================================================
    // HINT HELPERS
    // =========================================================

    // Used by SudokuManager to prefer empty cells for hints.
    public bool IsEditableAndEmpty()
    {
        return editable && inputField != null && string.IsNullOrWhiteSpace(inputField.text);
    }

    // Returns true when this editable cell is blank or incorrect.
    public bool NeedsHint()
    {
        return editable && !IsCorrect();
    }

    // Fills this cell with its correct answer and locks it.
    // This deliberately does not invoke CheckAnswer, so a hint
    // cannot trigger wrong-answer audio or timer penalties.
    public bool ApplyHint()
    {
        if (!editable || inputField == null || IsCorrect())
            return false;

        inputField.onEndEdit.RemoveListener(CheckAnswer);
        inputField.SetTextWithoutNotify(correctAnswer.ToString());
        inputField.interactable = false;
        inputField.readOnly = true;
        editable = false;

        if (inputField.image != null)
            inputField.image.color = new Color32(199, 185, 149, 255); // Muted beige-gold

        if (inputField.textComponent != null)
        {
            inputField.textComponent.gameObject.SetActive(true);
            inputField.textComponent.text = correctAnswer.ToString();
            inputField.textComponent.color = Color.black;
            inputField.textComponent.fontSize = 36;
            inputField.textComponent.alignment = TextAlignmentOptions.Center;
            inputField.textComponent.fontStyle = FontStyles.Bold;
        }

        return true;
    }

    // =========================================================
    // CHECK WHETHER THIS CELL IS CORRECT
    // =========================================================

    public bool IsCorrect()
    {
        if (!editable)
            return true;

        if (inputField == null)
            return false;

        if (string.IsNullOrEmpty(inputField.text))
            return false;

        if (!int.TryParse(inputField.text, out int playerNumber))
            return false;

        if (playerNumber < 1 || playerNumber > 6)
            return false;

        return playerNumber == correctAnswer;
    }
}
