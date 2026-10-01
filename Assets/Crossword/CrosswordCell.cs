using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrosswordCell : MonoBehaviour
{
    [Header("Input")]
    public TMP_InputField letterInput;

    [Header("Correct Answer")]
    [SerializeField]
    private string correctLetter;

    [Header("Colors")]
    public Color normalColor = Color.white;

    // Color ng GIVEN LETTER mismo
    public Color givenLetterTextColor = new Color(0.25f, 0.25f, 0.25f);

    // Whole-cell feedback colors
    public Color correctColor = new Color(0.3f, 1f, 0.3f);
    public Color wrongColor = new Color(1f, 0.3f, 0.3f);

    private bool isGivenLetter = false;

    private Image cellImage;
    private Graphic inputBackground;
    private TMP_Text inputText;

    private void Start()
    {
        // Main cell Image
        cellImage = GetComponent<Image>();

        if (letterInput != null)
        {
            letterInput.characterLimit = 1;

            letterInput.onValueChanged.AddListener(OnLetterChanged);

            // Background ng InputField
            inputBackground = letterInput.targetGraphic;

            // Text component ng InputField
            inputText = letterInput.textComponent;
        }

        ResetColor();
    }

    private void OnLetterChanged(string value)
    {
        if (letterInput == null || isGivenLetter)
            return;

        if (string.IsNullOrEmpty(value))
        {
            ResetColor();
            return;
        }

        string filtered = "";

        foreach (char c in value)
        {
            if ((c >= 'A' && c <= 'Z') ||
                (c >= 'a' && c <= 'z'))
            {
                filtered += c;
            }
        }

        if (filtered.Length > 1)
        {
            filtered = filtered.Substring(0, 1);
        }

        filtered = filtered.ToUpper();

        letterInput.SetTextWithoutNotify(filtered);

        // White muna habang hindi pa CHECK ANSWER
        ResetColor();
    }

    public void SetCorrectLetter(string letter)
    {
        if (string.IsNullOrEmpty(letter))
            return;

        correctLetter = letter.ToUpper();
    }

    // =========================
    // GIVEN LETTER
    // =========================
    public void SetGivenLetter(string letter)
    {
        if (letterInput == null || string.IsNullOrEmpty(letter))
            return;

        isGivenLetter = true;

        letterInput.SetTextWithoutNotify(
            letter.ToUpper()
        );

        letterInput.interactable = false;

        // WHITE PA RIN ANG CELL
        if (cellImage != null)
        {
            cellImage.color = normalColor;
        }

        if (inputBackground != null)
        {
            inputBackground.color = normalColor;
        }

        // DARKER ANG LETTER MISMO
        if (inputText != null)
        {
            inputText.color = givenLetterTextColor;
        }
    }

    public bool IsGivenLetter()
    {
        return isGivenLetter;
    }

    // =========================
    // CHECK ANSWER
    // =========================
    public bool IsCorrect()
    {
        if (letterInput == null)
            return false;

        return letterInput.text.ToUpper() == correctLetter;
    }

    public bool IsEmpty()
    {
        if (letterInput == null)
            return true;

        return string.IsNullOrEmpty(letterInput.text);
    }

    public string GetPlayerLetter()
    {
        if (letterInput == null)
            return "";

        return letterInput.text.ToUpper();
    }

    // =========================
    // CORRECT
    // =========================
    public void ShowCorrect()
    {
        if (isGivenLetter)
            return;

        ApplyCellColor(correctColor);
    }

    // =========================
    // WRONG
    // =========================
    public void ShowWrong()
    {
        if (isGivenLetter)
            return;

        ApplyCellColor(wrongColor);
    }

    // =========================
    // NORMAL
    // =========================
    public void ResetColor()
    {
        // Given letter
        if (isGivenLetter)
        {
            // Cell stays WHITE
            if (cellImage != null)
            {
                cellImage.color = normalColor;
            }

            if (inputBackground != null)
            {
                inputBackground.color = normalColor;
            }

            // Letter stays dark
            if (inputText != null)
            {
                inputText.color = givenLetterTextColor;
            }

            return;
        }

        // Normal editable cell
        ApplyCellColor(normalColor);

        if (inputText != null)
        {
            inputText.color = Color.black;
        }
    }

    // =========================
    // APPLY WHOLE CELL COLOR
    // =========================
    private void ApplyCellColor(Color color)
    {
        // Parent cell
        if (cellImage != null)
        {
            cellImage.color = color;
        }

        // InputField background
        if (inputBackground != null)
        {
            inputBackground.color = color;
        }
    }

    // =========================
    // CLEAR LETTER
    // =========================
    public void ClearLetter()
    {
        if (letterInput != null && !isGivenLetter)
        {
            letterInput.SetTextWithoutNotify("");

            ResetColor();
        }
    }
}