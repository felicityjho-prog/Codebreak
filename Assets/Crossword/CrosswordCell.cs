using UnityEngine;
using TMPro;

public class CrosswordCell : MonoBehaviour
{
    [Header("Input")]
    public TMP_InputField letterInput;

    [Header("Correct Answer")]
    [SerializeField]
    private string correctLetter;

    private void Start()
    {
        if (letterInput != null)
        {
            letterInput.onValueChanged.AddListener(OnLetterChanged);
        }
    }

    private void OnLetterChanged(string value)
    {
        if (letterInput == null)
            return;

        if (string.IsNullOrEmpty(value))
            return;

        string upperLetter = value.ToUpper();

        if (value != upperLetter)
        {
            letterInput.SetTextWithoutNotify(upperLetter);
        }
    }

    public void SetCorrectLetter(string letter)
    {
        correctLetter = letter.ToUpper();
    }

    public bool IsCorrect()
    {
        if (letterInput == null)
            return false;

        return letterInput.text.ToUpper() == correctLetter;
    }

    public string GetPlayerLetter()
    {
        if (letterInput == null)
            return "";

        return letterInput.text.ToUpper();
    }

    public void ClearLetter()
    {
        if (letterInput != null)
        {
            letterInput.SetTextWithoutNotify("");
        }
    }
}