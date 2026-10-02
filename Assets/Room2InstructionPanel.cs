using UnityEngine;

public class Room2InstructionPanel : MonoBehaviour
{
    [Header("Instruction Panel")]
    public GameObject instructionPanel;

    [Header("Player")]
    public MonoBehaviour playerMovement;
    public PlayerController playerController;

    private bool instructionClosed = false;

    // =========================================================
    // ON ENABLE
    // =========================================================

    private void OnEnable()
    {
        instructionClosed = false;
        ShowInstruction();
    }

    // =========================================================
    // SHOW INSTRUCTION
    // =========================================================

    public void ShowInstruction()
    {
        instructionClosed = false;

        // Show instruction panel
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(true);
        }

        // Stop WASD movement
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Stop camera / mouse look
        if (playerController != null)
        {
            playerController.DisableLook();
        }

        // Unlock cursor so player can click GOT IT
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("ROOM 2 INSTRUCTION OPENED.");
    }

    // =========================================================
    // CLOSE INSTRUCTION / GOT IT
    // =========================================================

    public void CloseInstruction()
    {
        if (instructionClosed)
            return;

        instructionClosed = true;

        // Hide instruction panel
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
        }

        // Enable WASD movement
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        // Enable camera / mouse look
        if (playerController != null)
        {
            playerController.EnableLook();
        }

        // IMPORTANT:
        // Return cursor to normal FPS gameplay mode
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("ROOM 2 INSTRUCTION CLOSED - PLAYER CONTROL RESTORED.");
    }
}