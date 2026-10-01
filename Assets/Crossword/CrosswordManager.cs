using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrosswordManager : MonoBehaviour
{
    [Header("Grid")]
    public GameObject crosswordCellPrefab;
    public int gridSize = 6;

    [Header("Colors")]
    public Color whiteCellColor = Color.white;
    public Color blockedCellColor = Color.black;

    [Header("Answer Feedback")]
    public TMP_Text resultText;

    [Header("Completion Controller")]
    public CrosswordPanelController crosswordPanelController;

    private bool challengeCompleted = false;

    private int[,] crosswordPattern =
    {
        { 0, 0, 0, 1, 1, 1 },
        { 0, 0, 0, 1, 1, 1 },
        { 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 0, 0, 0 },
        { 1, 1, 1, 0, 0, 0 },
        { 1, 1, 1, 0, 0, 0 }
    };

    private string[,] answerGrid =
    {
        { "A", "P", "I", "",  "",  "" },
        { "R", "A", "M", "",  "",  "" },
        { "C", "L", "O", "U", "D", "" },
        { "",  "Q", "U", "E", "R", "Y" },
        { "",  "",  "",  "C", "P", "U" },
        { "",  "",  "",  "W", "E", "B" }
    };

    private bool[,] givenLetters =
    {
        { true,  false, false, false, false, false },
        { false, true,  false, false, false, false },
        { true,  false, false, false, true,  false },
        { false, true,  false, false, false, true  },
        { false, false, false, false, true,  true  },
        { false, false, false, true,  false, false }
    };

    // ==========================================
    // START
    // ==========================================

    void Start()
    {
        GenerateGrid();

        if (resultText != null)
        {
            resultText.text = "";
        }
    }

    // ==========================================
    // GENERATE GRID
    // ==========================================

    void GenerateGrid()
    {
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                GameObject cell = Instantiate(
                    crosswordCellPrefab,
                    transform
                );

                Image cellImage = cell.GetComponent<Image>();

                if (cellImage == null)
                {
                    cellImage =
                        cell.GetComponentInChildren<Image>(true);
                }

                TMP_InputField input =
                    cell.GetComponentInChildren<TMP_InputField>(true);

                CrosswordCell crosswordCell =
                    cell.GetComponent<CrosswordCell>();

                bool blocked =
                    crosswordPattern[row, col] == 1;

                // ==================================
                // BLOCKED CELL
                // ==================================

                if (blocked)
                {
                    if (cellImage != null)
                    {
                        cellImage.color = blockedCellColor;
                    }

                    if (input != null)
                    {
                        input.gameObject.SetActive(false);
                    }

                    if (crosswordCell != null)
                    {
                        crosswordCell.enabled = false;
                    }
                }

                // ==================================
                // PLAYABLE CELL
                // ==================================

                else
                {
                    if (cellImage != null)
                    {
                        cellImage.color = whiteCellColor;
                    }

                    if (input != null)
                    {
                        input.gameObject.SetActive(true);
                    }

                    if (crosswordCell != null)
                    {
                        crosswordCell.enabled = true;

                        string correctLetter =
                            answerGrid[row, col];

                        crosswordCell.SetCorrectLetter(
                            correctLetter
                        );

                        // Given letters
                        if (givenLetters[row, col])
                        {
                            crosswordCell.SetGivenLetter(
                                correctLetter
                            );
                        }
                    }
                }
            }
        }
    }

    // ==========================================
    // CHECK ANSWER
    // ==========================================

    public void CheckAnswer()
    {
        if (challengeCompleted)
            return;

        CrosswordCell[] cells =
            GetComponentsInChildren<CrosswordCell>();

        bool hasWrong = false;
        bool hasEmpty = false;
        bool allCorrect = true;

        foreach (CrosswordCell cell in cells)
        {
            // Ignore blocked cells
            if (!cell.enabled)
                continue;

            // EMPTY CELL
            if (cell.IsEmpty())
            {
                hasEmpty = true;
                allCorrect = false;

                cell.ResetColor();
            }

            // WRONG LETTER
            else if (!cell.IsCorrect())
            {
                hasWrong = true;
                allCorrect = false;

                cell.ShowWrong();
            }

            // CORRECT LETTER
            else
            {
                cell.ShowCorrect();
            }
        }

        // ==========================================
        // WRONG ANSWERS
        // ==========================================

        if (hasWrong)
        {
            if (resultText != null)
            {
                resultText.text =
                    "SOME ANSWERS ARE INCORRECT. TRY AGAIN.";
            }

            return;
        }

        // ==========================================
        // EMPTY BOXES
        // ==========================================

        if (hasEmpty)
        {
            if (resultText != null)
            {
                resultText.text =
                    "PLEASE COMPLETE ALL THE BOXES.";
            }

            return;
        }

        // ==========================================
        // EVERYTHING IS CORRECT
        // ==========================================

        if (allCorrect)
        {
            CompleteChallenge();
        }
    }

    // ==========================================
    // COMPLETE CHALLENGE
    // ==========================================

    private void CompleteChallenge()
    {
        challengeCompleted = true;

        Debug.Log("CROSSWORD CHALLENGE COMPLETED!");

        // Clear result text
        if (resultText != null)
        {
            resultText.text = "";
        }

        // ------------------------------------------
        // SEND TO PANEL CONTROLLER
        // ------------------------------------------

        if (crosswordPanelController != null)
        {
            crosswordPanelController.CompleteCrossword();
        }
        else
        {
            Debug.LogWarning(
                "CrosswordPanelController is NOT assigned!"
            );
        }
    }
}