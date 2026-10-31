
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SudokuCell : MonoBehaviour
{
    [Header("Input Field")]
    public TMP_InputField inputField;

    private int givenNumber;
    private int correctAnswer;
    private bool editable;

    private SudokuManager sudokuManager;

    private Image cellImage;
    private Graphic inputBackground;
    private TMP_Text inputText;

    // =========================================================
    // COLORS - SAME AS CROSSWORD
    // =========================================================

    private readonly Color normalColor = Color.white;

    private readonly Color correctColor =
        new Color(0.3f, 1f, 0.3f, 1f);

    private readonly Color wrongColor =
        new Color(1f, 0.3f, 0.3f, 1f);

    private readonly Color hintColor =
        new Color32(199, 185, 149, 255);

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
        cellImage = GetComponent<Image>();

        if (inputField == null)
        {
            inputField =
                GetComponentInChildren<TMP_InputField>(true);
        }

        if (inputField == null)
        {
            Debug.LogError(
                "TMP_InputField NOT FOUND on SudokuCell!",
                this
            );
            return;
        }

        // SAME AS CROSSWORD
        inputBackground = inputField.targetGraphic;
        inputText = inputField.textComponent;
    }

    // =========================================================
    // PREPARE INPUT FIELD
    // =========================================================

    private void PrepareInputField()
    {
        if (inputField == null)
            return;

        inputField.contentType =
            TMP_InputField.ContentType.IntegerNumber;

        inputField.onValidateInput =
            ValidateSudokuInput;

        inputField.characterLimit = 1;

        if (inputText != null)
        {
            inputText.gameObject.SetActive(true);
            inputText.color = Color.black;
            inputText.fontSize = 36;
            inputText.alignment =
                TextAlignmentOptions.Center;
            inputText.fontStyle = FontStyles.Bold;
        }
    }

    // =========================================================
    // VALIDATE INPUT: NUMBERS 1 TO 6 ONLY
    // =========================================================

    private char ValidateSudokuInput(
        string text,
        int charIndex,
        char addedChar)
    {
        if (addedChar >= '1' && addedChar <= '6')
            return addedChar;

        return '\0';
    }

    // =========================================================
    // SETUP CELL
    // =========================================================

    public void Setup(
        int given,
        int answer,
        bool canEdit,
        SudokuManager manager)
    {
        givenNumber = given;
        correctAnswer = answer;
        editable = canEdit;
        sudokuManager = manager;

        FindInputField();

        if (inputField == null)
            return;

        PrepareInputField();

        inputField.onEndEdit.RemoveListener(CheckAnswer);

        inputField.transition =
            Selectable.Transition.ColorTint;

        // GIVEN NUMBER
        if (!editable)
        {
            inputField.SetTextWithoutNotify(
                givenNumber.ToString()
            );

            inputField.interactable = false;
            inputField.readOnly = true;

            ApplyCellColor(normalColor);

            if (inputText != null)
            {
                inputText.text = givenNumber.ToString();
                inputText.color = Color.black;
                inputText.fontSize = 36;
                inputText.alignment =
                    TextAlignmentOptions.Center;
                inputText.fontStyle = FontStyles.Bold;
            }
        }

        // PLAYER CELL
        else
        {
            inputField.SetTextWithoutNotify(string.Empty);

            inputField.interactable = true;
            inputField.readOnly = false;

            inputField.contentType =
                TMP_InputField.ContentType.IntegerNumber;

            inputField.characterLimit = 1;
            inputField.onValidateInput =
                ValidateSudokuInput;

            ApplyCellColor(normalColor);

            if (inputText != null)
            {
                inputText.color = Color.black;
                inputText.fontSize = 36;
                inputText.alignment =
                    TextAlignmentOptions.Center;
                inputText.fontStyle = FontStyles.Normal;
            }

            inputField.onEndEdit.AddListener(CheckAnswer);
        }
    }

    // =========================================================
    // CHECK PLAYER ANSWER
    // =========================================================

    private void CheckAnswer(string playerAnswer)
    {
        if (!editable || inputField == null)
            return;

        if (string.IsNullOrEmpty(playerAnswer))
            return;

        if (!int.TryParse(
            playerAnswer,
            out int playerNumber))
            return;

        if (playerNumber < 1 || playerNumber > 6)
            return;

        // =====================================================
        // CORRECT ANSWER
        // =====================================================

        if (playerNumber == correctAnswer)
        {
            Debug.Log(
                "CORRECT Sudoku answer: " + playerNumber,
                this
            );

            // LOCK CELL
            editable = false;
            inputField.readOnly = true;

            // Prevent selection tint from changing color
            inputField.transition =
                Selectable.Transition.None;

            // SAME COLOR APPLICATION AS CROSSWORD
            ApplyCellColor(correctColor);

            if (inputText != null)
                inputText.color = Color.black;

            inputField.onEndEdit.RemoveListener(CheckAnswer);

            Debug.Log(
                "CORRECT SUDOKU CELL LOCKED!",
                this
            );

            if (sudokuManager != null)
                sudokuManager.CheckSudokuComplete();
        }

        // =====================================================
        // WRONG ANSWER
        // =====================================================

        else
        {
            Debug.Log(
                "WRONG Sudoku answer: " + playerNumber,
                this
            );

            inputField.transition =
                Selectable.Transition.None;

            // SAME WRONG COLOR AS CROSSWORD
            ApplyCellColor(wrongColor);

            if (inputText != null)
                inputText.color = Color.black;

            // Remains editable
            if (sudokuManager != null)
                sudokuManager.PlayWrongAnswerSound();

            if (Room2ChallengeTimer.Instance != null)
                Room2ChallengeTimer.Instance.WrongAnswer();
        }
    }

    // =========================================================
    // APPLY CELL COLOR - SAME AS CROSSWORD
    // =========================================================

    private void ApplyCellColor(Color color)
    {
        if (cellImage != null)
            cellImage.color = color;

        if (inputBackground != null)
            inputBackground.color = color;
    }

    // =========================================================
    // HINT HELPERS
    // =========================================================

    public bool IsEditableAndEmpty()
    {
        return editable &&
               inputField != null &&
               string.IsNullOrWhiteSpace(inputField.text);
    }

    public bool NeedsHint()
    {
        return editable && !IsCorrect();
    }

    // =========================================================
    // APPLY HINT
    // =========================================================

    public bool ApplyHint()
    {
        if (!editable || inputField == null || IsCorrect())
            return false;

        inputField.onEndEdit.RemoveListener(CheckAnswer);

        inputField.SetTextWithoutNotify(
            correctAnswer.ToString()
        );

        editable = false;
        inputField.readOnly = true;

        inputField.transition =
            Selectable.Transition.None;

        ApplyCellColor(hintColor);

        if (inputText != null)
        {
            inputText.gameObject.SetActive(true);
            inputText.text = correctAnswer.ToString();
            inputText.color = Color.black;
            inputText.fontSize = 36;
            inputText.alignment =
                TextAlignmentOptions.Center;
            inputText.fontStyle = FontStyles.Bold;
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

        if (!int.TryParse(
            inputField.text,
            out int playerNumber))
            return false;

        if (playerNumber < 1 || playerNumber > 6)
            return false;

        return playerNumber == correctAnswer;
    }
}
