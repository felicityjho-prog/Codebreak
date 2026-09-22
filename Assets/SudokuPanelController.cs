using UnityEngine;

public class SudokuPanelController : MonoBehaviour
{
    [Header("Player Controls")]
    public MonoBehaviour playerMovement;
    public MonoBehaviour playerLook;

    private void OnEnable()
    {
        // Freeze player movement and camera
        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerLook != null)
            playerLook.enabled = false;

        // Allow mouse to interact with Sudoku
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnDisable()
    {
        // Enable player controls again
        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerLook != null)
            playerLook.enabled = true;

        // Lock mouse back to the game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}