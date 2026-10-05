using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

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

    [Header("Wrong Answer Audio")]
    public AudioSource wrongAnswerAudio;
    public AudioClip wrongAnswerSound;

    [Header("Completion Controller")]
    public CrosswordPanelController crosswordPanelController;

    // =========================================================
    // TAB NAVIGATION
    // =========================================================

    private List<CrosswordCell> editableCells =
        new List<CrosswordCell>();

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

    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        GenerateGrid();

        // Disable Unity automatic UI navigation.
        // TAB will be handled here manually.
        if (EventSystem.current != null)
        {
            EventSystem.current.sendNavigationEvents = false;
        }

        if (resultText != null)
        {
            resultText.text = "";
        }
    }

    // =========================================================
    // UPDATE - HANDLE TAB
    // =========================================================

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (!Keyboard.current.tabKey.wasPressedThisFrame)
            return;

        if (EventSystem.current == null)
            return;

        GameObject selectedObject =
            EventSystem.current.currentSelectedGameObject;

        if (selectedObject == null)
            return;

        CrosswordCell currentCell = null;

        // Find which crossword cell is currently selected
        foreach (CrosswordCell cell in editableCells)
        {
            if (cell != null &&
                cell.letterInput != null &&
                cell.letterInput.gameObject == selectedObject)
            {
                currentCell = cell;
                break;
            }
        }

        if (currentCell == null)
            return;

        bool shiftHeld =
            Keyboard.current.leftShiftKey.isPressed ||
            Keyboard.current.rightShiftKey.isPressed;

        if (shiftHeld)
        {
            FocusPreviousCell(currentCell);
        }
        else
        {
            FocusNextCell(currentCell);
        }
    }

    // =========================================================
    // NEXT CELL
    // =========================================================

    private void FocusNextCell(CrosswordCell currentCell)
    {
        int currentIndex =
            editableCells.IndexOf(currentCell);

        if (currentIndex < 0)
            return;

        int nextIndex =
            currentIndex + 1;

        if (nextIndex >= editableCells.Count)
        {
            nextIndex = 0;
        }

        FocusCell(editableCells[nextIndex]);
    }

    // =========================================================
    // PREVIOUS CELL
    // =========================================================

    private void FocusPreviousCell(CrosswordCell currentCell)
    {
        int currentIndex =
            editableCells.IndexOf(currentCell);

        if (currentIndex < 0)
            return;

        int previousIndex =
            currentIndex - 1;

        if (previousIndex < 0)
        {
            previousIndex =
                editableCells.Count - 1;
        }

        FocusCell(editableCells[previousIndex]);
    }

    // =========================================================
    // FOCUS CELL
    // =========================================================

    private void FocusCell(CrosswordCell cell)
    {
        if (cell == null)
            return;

        if (cell.letterInput == null)
            return;

        if (cell.IsGivenLetter())
            return;

        if (!cell.letterInput.interactable)
            return;

        cell.letterInput.Select();
        cell.letterInput.ActivateInputField();

        cell.letterInput.caretPosition =
            cell.letterInput.text.Length;
    }

    // =========================================================
    // GENERATE GRID
    // =========================================================

    private void GenerateGrid()
    {
        editableCells.Clear();

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                GameObject cell =
                    Instantiate(
                        crosswordCellPrefab,
                        transform
                    );

                Image cellImage =
                    cell.GetComponent<Image>();

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

                // =================================================
                // BLOCKED CELL
                // =================================================

                if (blocked)
                {
                    if (cellImage != null)
                    {
                        cellImage.color =
                            blockedCellColor;
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

                // =================================================
                // PLAYABLE CELL
                // =================================================

                else
                {
                    if (cellImage != null)
                    {
                        cellImage.color =
                            whiteCellColor;
                    }

                    if (input != null)
                    {
                        input.gameObject.SetActive(true);
                    }

                    if (crosswordCell != null)
                    {
                        crosswordCell.enabled = true;

                        crosswordCell.SetCorrectLetter(
                            answerGrid[row, col]
                        );

                        if (givenLetters[row, col])
                        {
                            crosswordCell.SetGivenLetter(
                                answerGrid[row, col]
                            );
                        }
                        else
                        {
                            editableCells.Add(
                                crosswordCell
                            );
                        }
                    }
                }
            }
        }

        // =========================================================
        // SET CELL NAVIGATION
        // =========================================================

        SetupTabNavigation();
    }

    // =========================================================
    // SET TAB NAVIGATION
    // =========================================================

    private void SetupTabNavigation()
    {
        for (int i = 0; i < editableCells.Count; i++)
        {
            CrosswordCell previousCell = null;
            CrosswordCell nextCell = null;

            if (i > 0)
            {
                previousCell =
                    editableCells[i - 1];
            }
            else if (editableCells.Count > 0)
            {
                previousCell =
                    editableCells[
                        editableCells.Count - 1
                    ];
            }

            if (i < editableCells.Count - 1)
            {
                nextCell =
                    editableCells[i + 1];
            }
            else if (editableCells.Count > 0)
            {
                nextCell =
                    editableCells[0];
            }

            editableCells[i].SetNavigation(
                previousCell,
                nextCell
            );
        }

        Debug.Log(
            "Crossword TAB navigation setup complete. " +
            "Editable cells: " +
            editableCells.Count
        );
    }

    // =========================================================
    // CHECK ANSWER
    // =========================================================

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
            if (!cell.enabled)
                continue;

            if (cell.IsEmpty())
            {
                hasEmpty = true;
                allCorrect = false;

                cell.ResetColor();
            }
            else if (!cell.IsCorrect())
            {
                hasWrong = true;
                allCorrect = false;

                cell.ShowWrong();
            }
            else
            {
                cell.ShowCorrect();
            }
        }

        // =====================================================
        // WRONG ANSWERS
        // =====================================================

        if (hasWrong)
        {
            if (resultText != null)
            {
                resultText.text =
                    "SOME ANSWERS ARE INCORRECT. -10 SECONDS";
            }

            // =========================================
            // PLAY WRONG ANSWER SOUND
            // =========================================

            if (wrongAnswerAudio != null &&
                wrongAnswerSound != null)
            {
                wrongAnswerAudio.PlayOneShot(
                    wrongAnswerSound
                );
            }

            // =========================================
            // REMOVE 10 SECONDS
            // =========================================

            if (Room2ChallengeTimer.Instance != null)
            {
                Room2ChallengeTimer.Instance.WrongAnswer();
            }

            return;
        }

        // =====================================================
        // EMPTY BOXES
        // =====================================================

        if (hasEmpty)
        {
            if (resultText != null)
            {
                resultText.text =
                    "PLEASE COMPLETE ALL THE BOXES.";
            }

            return;
        }

        // =====================================================
        // EVERYTHING CORRECT
        // =====================================================

        if (allCorrect)
        {
            if (resultText != null)
            {
                resultText.text =
                    "CORRECT!";
            }

            CompleteChallenge();
        }
    }

    // =========================================================
    // COMPLETE CHALLENGE
    // =========================================================

    private void CompleteChallenge()
    {
        challengeCompleted = true;

        Debug.Log(
            "CROSSWORD CHALLENGE COMPLETED!"
        );

        if (resultText != null)
        {
            resultText.text = "";
        }

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

    // =========================================================
    // RESET CROSSWORD
    // =========================================================

    public void ResetCrossword()
    {
        Debug.Log("=================================");
        Debug.Log("RESETTING CROSSWORD");
        Debug.Log("=================================");

        challengeCompleted = false;

        if (resultText != null)
        {
            resultText.text = "";
        }

        CrosswordCell[] cells =
            GetComponentsInChildren<CrosswordCell>();

        foreach (CrosswordCell cell in cells)
        {
            if (cell == null)
                continue;

            if (!cell.enabled)
                continue;

            cell.ClearLetter();
            cell.ResetColor();
        }

        Debug.Log(
            "CROSSWORD RESET COMPLETE!"
        );
    }
}