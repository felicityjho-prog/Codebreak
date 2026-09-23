using UnityEngine;

public class Room2InstructionPanel : MonoBehaviour
{
    [Header("Instruction Panel")]
    public GameObject instructionPanel;

    [Header("Player")]
    public MonoBehaviour playerMovement;
    public PlayerController playerController;

    private void OnEnable()
    {
        ShowInstruction();
    }

    public void ShowInstruction()
    {
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(true);
        }

        // Stop WASD movement
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Stop mouse/camera movement
        if (playerController != null)
        {
            playerController.DisableLook();
        }

        // Make cursor available for clicking
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseInstruction()
    {
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

        // Enable camera/mouse movement
        if (playerController != null)
        {
            playerController.EnableLook();
        }
    }
}