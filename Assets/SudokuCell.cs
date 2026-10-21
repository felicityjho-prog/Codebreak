
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
            inputField =
                GetComponentInChildren<TMP_InputField>(true);
        }

        if (inputField == null)
        {
            Debug.LogError(
                "TMP_InputField NOT FOUND on SudokuCell!",
                this
            );
        }
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

        // Allow only Sudoku numbers 1 to 6.
        inputField.onValidateInput = ValidateSudokuInput;

        // One number per cell.
        inputField.characterLimit = 1;

        if (inputField.textComponent != null)
        {
            inputField.textComponent.gameObject.SetActive(true);
            inputField.textComponent.color = Color.black;
            inputField.textComponent.fontSize = 36;
            inputField.textComponent.alignment =
                TextAlignmentOptions.Center;
            inputField.textComponent.fontStyle =
                FontStyles.Bold;
        }
    }

    // =========================================================
    // VALIDATE INPUT: ALLOW ONLY NUMBERS 1 TO 6
    // =========================================================

    private char ValidateSudokuInput(
        string text,
        int charIndex,
        char addedChar
    )
    {
        if (addedChar >= '1' && addedChar <= '6')
        {
            return addedChar;
        }

        // Reject 0, 7, 8, 9, letters, and other characters.
        return '\0';
    }

    // =========================================================
    // SETUP CELL
    // =========================================================

    public void Setup(
        int given,
        int answer,
        bool canEdit,
        SudokuManager manager
    )
    {
        givenNumber = given;
        correctAnswer = answer;
        editable = canEdit;
        sudokuManager = manager;

        FindInputField();

        if (inputField == null)
        {
            Debug.LogError(
                "TMP_InputField NOT FOUND!",
                this
            );

            return;
        }

        PrepareInputField();

        // =====================================================
        // GIVEN NUMBER: CANNOT BE EDITED
        // =====================================================

        if (!editable)
        {
            inputField.onEndEdit.RemoveListener(CheckAnswer);

            inputField.text = givenNumber.ToString();
            inputField.interactable = false;
            inputField.readOnly = true;

            if (inputField.image != null)
            {
                inputField.image.color = Color.white;
            }

            if (inputField.textComponent != null)
            {
                inputField.textComponent.gameObject.SetActive(true);
                inputField.textComponent.text =
                    givenNumber.ToString();
                inputField.textComponent.color = Color.black;
                inputField.textComponent.fontSize = 36;
                inputField.textComponent.alignment =
                    TextAlignmentOptions.Center;
                inputField.textComponent.fontStyle =
                    FontStyles.Bold;
            }
        }

        // =====================================================
        // PLAYER CELL: EDITABLE
        // =====================================================

        else
        {
            inputField.text = "";
            inputField.interactable = true;
            inputField.readOnly = false;

            inputField.contentType =
                TMP_InputField.ContentType.IntegerNumber;

            inputField.characterLimit = 1;

            // Reapply validation after setting contentType.
            inputField.onValidateInput = ValidateSudokuInput;

            if (inputField.image != null)
            {
                inputField.image.color = Color.white;
            }

            if (inputField.textComponent != null)
            {
                inputField.textComponent.gameObject.SetActive(true);
                inputField.textComponent.color = Color.black;
                inputField.textComponent.fontSize = 36;
                inputField.textComponent.alignment =
                    TextAlignmentOptions.Center;
                inputField.textComponent.fontStyle =
                    FontStyles.Normal;
            }

            // Prevent duplicate listeners without removing
            // unrelated listeners belonging to other systems.
            inputField.onEndEdit.RemoveListener(CheckAnswer);
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

        // Empty input does not count as a wrong answer.
        if (string.IsNullOrEmpty(playerAnswer))
            return;

        if (!int.TryParse(playerAnswer, out int playerNumber))
            return;

        // Extra safety check: Sudoku accepts numbers 1 to 6 only.
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

            if (inputField.image != null)
            {
                inputField.image.color = Color.green;
            }

            if (inputField.textComponent != null)
            {
                inputField.textComponent.color = Color.black;
            }

            // No timer penalty for a correct answer.

            if (sudokuManager != null)
            {
                sudokuManager.CheckSudokuComplete();
            }
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

            if (inputField.image != null)
            {
                inputField.image.color = Color.red;
            }

            if (inputField.textComponent != null)
            {
                inputField.textComponent.color = Color.black;
            }

            // Play wrong-answer sound.
            if (sudokuManager != null)
            {
                sudokuManager.PlayWrongAnswerSound();
            }

            // Deduct 10 seconds through the existing timer.
            if (Room2ChallengeTimer.Instance != null)
            {
                Room2ChallengeTimer.Instance.WrongAnswer();
            }
        }
    }

    // =========================================================
    // CHECK WHETHER THIS CELL IS CORRECT
    // =========================================================

    public bool IsCorrect()
    {
        // Given numbers are fixed and already correct.
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
