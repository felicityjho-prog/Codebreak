using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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

    public Color givenLetterTextColor =
        new Color(0.25f, 0.25f, 0.25f);

    public Color correctColor =
        new Color(0.3f, 1f, 0.3f);

    public Color wrongColor =
        new Color(1f, 0.3f, 0.3f);

    private bool isGivenLetter = false;

    private Image cellImage;
    private Graphic inputBackground;
    private TMP_Text inputText;

    // =========================================================
    // TAB NAVIGATION
    // =========================================================

    private CrosswordCell nextCell;
    private CrosswordCell previousCell;

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        cellImage = GetComponent<Image>();

        if (letterInput != null)
        {
            letterInput.characterLimit = 1;

            letterInput.onValueChanged.AddListener(
                OnLetterChanged
            );

            inputBackground = letterInput.targetGraphic;
            inputText = letterInput.textComponent;

            // Disable Unity automatic navigation
            Navigation navigation =
                letterInput.navigation;

            navigation.mode =
                Navigation.Mode.None;

            letterInput.navigation =
                navigation;
        }

        ResetColor();
    }

    // =========================================================
    // TAB NAVIGATION SETUP
    // =========================================================

    public void SetNavigation(
        CrosswordCell previous,
        CrosswordCell next)
    {
        previousCell = previous;
        nextCell = next;
    }

    // =========================================================
    // NEXT CELL
    // =========================================================

    public CrosswordCell GetNextCell()
    {
        return nextCell;
    }

    // =========================================================
    // PREVIOUS CELL
    // =========================================================

    public CrosswordCell GetPreviousCell()
    {
        return previousCell;
    }

    // =========================================================
    // LETTER CHANGED
    // =========================================================

    private void OnLetterChanged(string value)
    {
        // Do nothing if this cell is already locked
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
            filtered =
                filtered.Substring(0, 1);
        }

        filtered = filtered.ToUpper();

        letterInput.SetTextWithoutNotify(
            filtered
        );

        // White muna habang hindi pa CHECK ANSWER
        ResetColor();
    }

    // =========================================================
    // CORRECT LETTER
    // =========================================================

    public void SetCorrectLetter(string letter)
    {
        if (string.IsNullOrEmpty(letter))
            return;

        correctLetter =
            letter.ToUpper();
    }

    // =========================================================
    // GIVEN LETTER
    // =========================================================

    public void SetGivenLetter(string letter)
    {
        if (letterInput == null ||
            string.IsNullOrEmpty(letter))
            return;

        isGivenLetter = true;

        letterInput.SetTextWithoutNotify(
            letter.ToUpper()
        );

        // GIVEN LETTER = PERMANENTLY NOT EDITABLE
        letterInput.interactable = false;

        if (cellImage != null)
        {
            cellImage.color =
                normalColor;
        }

        if (inputBackground != null)
        {
            inputBackground.color =
                normalColor;
        }

        if (inputText != null)
        {
            inputText.color =
                givenLetterTextColor;
        }
    }

    public bool IsGivenLetter()
    {
        return isGivenLetter;
    }

    // =========================================================
    // CHECK ANSWER
    // =========================================================

    public bool IsCorrect()
    {
        if (letterInput == null)
            return false;

        return letterInput.text.ToUpper() ==
               correctLetter;
    }

    public bool IsEmpty()
    {
        if (letterInput == null)
            return true;

        return string.IsNullOrEmpty(
            letterInput.text
        );
    }

    public string GetPlayerLetter()
    {
        if (letterInput == null)
            return "";

        return letterInput.text.ToUpper();
    }

    // =========================================================
    // CORRECT
    // =========================================================

    public void ShowCorrect()
    {
        if (isGivenLetter)
            return;

        // Turn cell green
        ApplyCellColor(correctColor);

        // LOCK THE CELL
        if (letterInput != null)
        {
            letterInput.interactable = false;
        }
    }

    // =========================================================
    // WRONG
    // =========================================================

    public void ShowWrong()
    {
        if (isGivenLetter)
            return;

        // Wrong cells must remain editable
        if (letterInput != null)
        {
            letterInput.interactable = true;
        }

        ApplyCellColor(wrongColor);
    }

    // =========================================================
    // NORMAL
    // =========================================================

    public void ResetColor()
    {
        // Given letter
        if (isGivenLetter)
        {
            if (cellImage != null)
            {
                cellImage.color =
                    normalColor;
            }

            if (inputBackground != null)
            {
                inputBackground.color =
                    normalColor;
            }

            if (inputText != null)
            {
                inputText.color =
                    givenLetterTextColor;
            }

            return;
        }

        // Normal editable cell
        ApplyCellColor(normalColor);

        if (inputText != null)
        {
            inputText.color =
                Color.black;
        }
    }

    // =========================================================
    // APPLY WHOLE CELL COLOR
    // =========================================================

    private void ApplyCellColor(Color color)
    {
        if (cellImage != null)
        {
            cellImage.color =
                color;
        }

        if (inputBackground != null)
        {
            inputBackground.color =
                color;
        }
    }

    // =========================================================
    // CLEAR LETTER
    // =========================================================

    public void ClearLetter()
    {
        if (letterInput != null &&
            !isGivenLetter)
        {
            // Unlock again when crossword is reset
            letterInput.interactable = true;

            letterInput.SetTextWithoutNotify("");

            ResetColor();
        }
    }

    // =========================================================
    // CAN EDIT?
    // =========================================================

    public bool CanEdit()
    {
        if (isGivenLetter)
            return false;

        if (letterInput == null)
            return false;

        return letterInput.interactable;
    }
}