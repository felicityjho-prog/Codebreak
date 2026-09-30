using UnityEngine;

public class TableSudokuInteraction : MonoBehaviour
{
    [Header("Settings")]
    public float interactionDistance = 3f;

    [Header("UI")]
    public GameObject interactPrompt;

    [Header("Sudoku")]
    public SudokuPanelController sudokuPanelController;

    private Transform player;
    private bool isInteracting = false;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    private void Update()
    {
        if (player == null)
            return;

        // Stop checking interaction while solving
        if (isInteracting)
            return;

        float distance = Vector3.Distance(
            player.position,
            transform.position
        );

        if (distance <= interactionDistance)
        {
            if (interactPrompt != null)
                interactPrompt.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                OpenSudoku();
            }
        }
        else
        {
            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
    }

    private void OpenSudoku()
    {
        isInteracting = true;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (sudokuPanelController != null)
            sudokuPanelController.OpenSudoku();
    }

    public void FinishInteraction()
    {
        isInteracting = false;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }
}