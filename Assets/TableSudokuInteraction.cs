using UnityEngine;
using UnityEngine.InputSystem;

public class TableSudokuInteraction : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject sudokuPanel;

    [Header("Interaction Settings")]
    public float interactionDistance = 3f;

    private void Update()
    {
        if (player == null || sudokuPanel == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= interactionDistance)
        {
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                OpenSudoku();
            }
        }
    }

    private void OpenSudoku()
    {
        sudokuPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}