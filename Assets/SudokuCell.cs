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
            Debug.LogError(
                "TMP_InputField NOT FOUND on SudokuCell!"
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

        inputField.characterLimit = 1;

        // Make sure Text component is active
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
                "TMP_InputField NOT FOUND!"
            );
            return;
        }

        PrepareInputField();

        // =====================================================
        // GIVEN NUMBER
        // =====================================================

        if (!editable)
        {
            // Set the given number
            inputField.text =
                givenNumber.ToString();

            // Make sure it is visible
            inputField.interactable = false;
            inputField.readOnly = true;

            // Make background visible
            if (inputField.image != null)
            {
                inputField.image.color = Color.white;
            }

            // FORCE TEXT VISIBLE
            if (inputField.textComponent != null)
            {
                inputField.textComponent.gameObject.SetActive(true);

                inputField.textComponent.color =
                    Color.black;

                inputField.textComponent.fontSize =
                    36;

                inputField.textComponent.alignment =
                    TextAlignmentOptions.Center;

                inputField.textComponent.fontStyle =
                    FontStyles.Bold;

                inputField.textComponent.text =
                    givenNumber.ToString();
            }

            Debug.Log(
                "Given Sudoku number displayed: " +
                givenNumber
            );
        }

        // =====================================================
        // EMPTY / PLAYER CELL
        // =====================================================

        else
        {
            inputField.text = "";

            inputField.interactable = true;
            inputField.readOnly = false;

            inputField.contentType =
                TMP_InputField.ContentType.IntegerNumber;

            inputField.characterLimit = 1;

            // White background
            if (inputField.image != null)
            {
                inputField.image.color =
                    Color.white;
            }

            // Player text settings
            if (inputField.textComponent != null)
            {
                inputField.textComponent.gameObject.SetActive(true);

                inputField.textComponent.color =
                    Color.black;

                inputField.textComponent.fontSize =
                    36;

                inputField.textComponent.alignment =
                    TextAlignmentOptions.Center;

                inputField.textComponent.fontStyle =
                    FontStyles.Normal;
            }

            // Remove old listeners
            inputField.onEndEdit.RemoveAllListeners();

            // Add answer checker
            inputField.onEndEdit.AddListener(
                CheckAnswer
            );
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

        int playerNumber;

        if (int.TryParse(
            playerAnswer,
            out playerNumber))
        {
            // =================================================
            // CORRECT
            // =================================================

            if (playerNumber == correctAnswer)
            {
                Debug.Log(
                    "Correct answer: " +
                    playerNumber
                );

                if (inputField.image != null)
                {
                    inputField.image.color =
                        Color.green;
                }

                if (inputField.textComponent != null)
                {
                    inputField.textComponent.color =
                        Color.black;
                }

                // Check whole Sudoku
                if (sudokuManager != null)
                {
                    sudokuManager.CheckSudokuComplete();
                }
            }

            // =================================================
            // WRONG
            // =================================================

            else
            {
                Debug.Log(
                    "Wrong answer. Correct answer is: " +
                    correctAnswer
                );

                if (inputField.image != null)
                {
                    inputField.image.color =
                        Color.red;
                }

                if (inputField.textComponent != null)
                {
                    inputField.textComponent.color =
                        Color.black;
                }
            }
        }
    }

    // =========================================================
    // CHECK THIS CELL
    // =========================================================

    public bool IsCorrect()
    {
        // Given numbers are automatically correct
        if (!editable)
            return true;

        if (inputField == null)
            return false;

        if (string.IsNullOrEmpty(inputField.text))
            return false;

        int playerNumber;

        if (int.TryParse(
            inputField.text,
            out playerNumber))
        {
            return playerNumber == correctAnswer;
        }

        return false;
    }
}