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
        if (inputField == null)
        {
            inputField =
                GetComponentInChildren<TMP_InputField>();
        }

        if (inputField == null)
        {
            Debug.LogError(
                "TMP_InputField NOT FOUND on SudokuCell!"
            );
            return;
        }

        inputField.contentType =
            TMP_InputField.ContentType.IntegerNumber;

        inputField.characterLimit = 1;
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

        if (inputField == null)
        {
            inputField =
                GetComponentInChildren<TMP_InputField>();
        }

        if (inputField == null)
        {
            Debug.LogError(
                "TMP_InputField NOT FOUND!"
            );
            return;
        }

        // =====================================================
        // GIVEN NUMBER
        // =====================================================

        if (!editable)
        {
            inputField.text =
                givenNumber.ToString();

            inputField.interactable = false;
            inputField.readOnly = true;

            // Normal color for given numbers
            inputField.image.color = Color.white;
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

            // Reset color
            inputField.image.color = Color.white;

            // Remove previous listener
            inputField.onEndEdit.RemoveAllListeners();

            // Add checker
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

                inputField.image.color =
                    Color.green;

                // Check if ALL cells are now correct
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

                inputField.image.color =
                    Color.red;
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