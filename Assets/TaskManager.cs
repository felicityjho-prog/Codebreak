using UnityEngine;

public class TaskManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject taskCompletePanel;
    public GameObject checklistUI;
    public GameObject instructionPanel;
    public GameObject controlsPanel;
    public GameObject centerDot;

    [Header("Room")]
    public RoomSwitcher roomSwitcher;

    [Header("Player Freeze")]
    public PlayerMovement playerMovement;
    public PlayerController playerController;

    [Header("Timer")]
    public GameTimer gameTimer;

    [Header("Completion Audio")]
    public AudioSource completionAudio;
    public AudioClip completionSound;

    [SerializeField] private int totalObjects;

    private int collectedObjects = 0;
    private bool challengeCompleted = false;

    public void CollectObject()
    {
        // Prevent collecting again after challenge is completed
        if (challengeCompleted)
            return;

        collectedObjects++;

        if (collectedObjects >= totalObjects)
        {
            challengeCompleted = true;

            // =========================================
            // PLAY COMPLETION SOUND
            // =========================================
            if (completionAudio != null && completionSound != null)
            {
                completionAudio.PlayOneShot(completionSound);
            }

            // =========================================
            // STOP AND HIDE TIMER
            // =========================================
            if (gameTimer != null)
            {
                gameTimer.StopTimer();
            }

            // =========================================
            // HIDE CHECKLIST
            // =========================================
            if (checklistUI != null)
            {
                checklistUI.SetActive(false);
            }

            // =========================================
            // HIDE ROOM 1 INSTRUCTION
            // =========================================
            if (instructionPanel != null)
            {
                instructionPanel.SetActive(false);
            }

            // =========================================
            // HIDE CONTROLS
            // =========================================
            if (controlsPanel != null)
            {
                controlsPanel.SetActive(false);
            }

            // =========================================
            // HIDE CENTER DOT
            // =========================================
            if (centerDot != null)
            {
                centerDot.SetActive(false);
            }

            // =========================================
            // FREEZE PLAYER MOVEMENT
            // =========================================
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }

            // =========================================
            // FREEZE CAMERA LOOK
            // =========================================
            if (playerController != null)
            {
                playerController.enabled = false;
            }

            // =========================================
            // SHOW TASK COMPLETE PANEL
            // =========================================
            if (taskCompletePanel != null)
            {
                taskCompletePanel.SetActive(true);
            }

            // =========================================
            // ALLOW PLAYER TO PROCEED
            // =========================================
            if (roomSwitcher != null)
            {
                roomSwitcher.EnableProceed();
            }

            // =========================================
            // UNLOCK CURSOR
            // =========================================
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}