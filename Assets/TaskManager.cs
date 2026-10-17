
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
        if (challengeCompleted)
            return;

        collectedObjects++;

        if (collectedObjects >= totalObjects)
        {
            challengeCompleted = true;

            if (completionAudio != null && completionSound != null)
            {
                completionAudio.PlayOneShot(completionSound);
            }

            if (gameTimer != null)
            {
                gameTimer.StopTimer();
            }

            if (checklistUI != null)
                checklistUI.SetActive(false);

            if (instructionPanel != null)
                instructionPanel.SetActive(false);

            if (controlsPanel != null)
                controlsPanel.SetActive(false);

            if (centerDot != null)
                centerDot.SetActive(false);

            if (playerMovement != null)
                playerMovement.enabled = false;

            if (playerController != null)
                playerController.enabled = false;

            if (taskCompletePanel != null)
                taskCompletePanel.SetActive(true);

            if (roomSwitcher != null)
                roomSwitcher.EnableProceed();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // ==========================================
    // RESTART ROOM 1 CHALLENGE
    // ==========================================

    public void ResetChallenge()
    {
        Debug.Log("RESETTING ROOM 1 CHALLENGE...");

        // Reset collection progress.
        collectedObjects = 0;
        challengeCompleted = false;

        // Restore every Collectible object in the scene.
        Collectible[] collectibles =
            FindObjectsByType<Collectible>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (Collectible collectible in collectibles)
        {
            if (collectible != null)
                collectible.ResetCollectible();
        }

        // Hide the completion panel.
        if (taskCompletePanel != null)
            taskCompletePanel.SetActive(false);

        // Restore the checklist and gameplay UI.
        if (checklistUI != null)
            checklistUI.SetActive(true);

        if (instructionPanel != null)
            instructionPanel.SetActive(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(true);

        if (centerDot != null)
            centerDot.SetActive(true);

        // Allow the player to complete the challenge again.
        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerController != null)
        {
            playerController.enabled = true;
            playerController.EnableLook();
        }

        // Prevent proceeding using the previous completion state.
        if (roomSwitcher != null)
            roomSwitcher.canProceed = false;

        // Stop any previous completion audio.
        if (completionAudio != null)
            completionAudio.Stop();

        // Reset and restart Room 1 timer.
        if (gameTimer != null)
        {
            gameTimer.RestartTimerForChallenge();
        }

        // Restore normal gameplay cursor.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("ROOM 1 CHALLENGE RESET COMPLETE!");
    }
}
