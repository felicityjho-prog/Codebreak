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
            // Only allow letters A-Z
            letterInput.characterValidation =
                TMP_InputField.CharacterValidation.Alphanumeric;

            letterInput.characterLimit = 1;

            letterInput.onValueChanged.AddListener(OnLetterChanged);
        }
    }

    private void OnLetterChanged(string value)
    {
        if (letterInput == null)
            return;

        if (string.IsNullOrEmpty(value))
            return;

        // Remove anything that is NOT a letter
        string filtered = "";

        foreach (char c in value)
        {
            if (char.IsLetter(c))
            {
                filtered += c;
            }
        }

        // Keep only the first letter
        if (filtered.Length > 1)
        {
            filtered = filtered.Substring(0, 1);
        }

        // Convert to uppercase
        filtered = filtered.ToUpper();

        letterInput.SetTextWithoutNotify(filtered);
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