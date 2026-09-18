using UnityEngine;
using TMPro;

public class SudokuCell : MonoBehaviour
{
    public TMP_InputField inputField;

    private int correctAnswer;
    private bool editable;

    public void Setup(int givenNumber, int answer, bool canEdit)
    {
        correctAnswer = answer;
        editable = canEdit;

        if (inputField == null)
        {
            inputField = GetComponentInChildren<TMP_InputField>();
        }

        if (editable)
        {
            // EMPTY CELL
            inputField.text = "";
            inputField.interactable = true;
        }
        else
        {
            // GIVEN NUMBER
            inputField.text = givenNumber.ToString();
            inputField.interactable = false;
        }
    }

    public bool IsCorrect()
    {
        if (!editable)
        {
            return true;
        }

        if (int.TryParse(inputField.text, out int playerAnswer))
        {
            return playerAnswer == correctAnswer;
        }

        return false;
    }
}