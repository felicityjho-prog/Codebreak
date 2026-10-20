
using UnityEngine;

public class Room1GameOverController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform player;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerController playerLook;

    [Header("Hallway Spawn")]
    [SerializeField] private Transform hallwaySpawnPoint;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Room 1 Managers")]
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private ChecklistManager checklistManager;
    [SerializeField] private GameTimer room1Timer;
    [SerializeField] private RoomEntryTrigger roomEntryTrigger;

    [Header("Room 1 Door")]
    [SerializeField] private AutoDoor room1Door;

    [Header("Pause Menu")]
    [SerializeField] private PauseMenuController pauseMenuController;

    // ==========================================
    // RESTART ROOM 1 FROM HALLWAY
    // ==========================================

    public void RestartFromHallway()
    {
        Debug.Log("ROOM 1 RESTART: RETURNING TO HALLWAY");

        // Restore normal time and audio first.
        Time.timeScale = 1f;
        AudioListener.pause = false;

        // Hide Room 1 Game Over panel.
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Reset TaskManager progress, collectibles and completion UI.
        if (taskManager != null)
        {
            taskManager.ResetChallenge();
        }
        else
        {
            Debug.LogError(
                "Room1GameOverController: TaskManager is not assigned!"
            );
        }

        // Reset checklist text colors, checkmarks and counter.
        if (checklistManager != null)
        {
            checklistManager.ResetChecklist();
        }
        else
        {
            Debug.LogError(
                "Room1GameOverController: ChecklistManager is not assigned!"
            );
        }

        // Stop the Room 1 timer while in the hallway,
        // then restart the Room 1 background music.
        if (room1Timer != null)
        {
            room1Timer.StopTimer();
            room1Timer.PlayRoom1Music();
        }

        // Return the player to the hallway spawn point.
        if (player != null && hallwaySpawnPoint != null)
        {
            CharacterController characterController =
                player.GetComponent<CharacterController>();

            if (characterController != null)
            {
                characterController.enabled = false;
            }

            player.SetPositionAndRotation(
                hallwaySpawnPoint.position,
                hallwaySpawnPoint.rotation
            );

            if (characterController != null)
            {
                characterController.enabled = true;
            }
        }
        else
        {
            Debug.LogError(
                "Room1GameOverController: Player or Hallway Spawn Point is missing!"
            );
        }

        // Reset the door to its original closed state.
        if (room1Door != null)
        {
            room1Door.ResetDoorToClosed();
        }
        else
        {
            Debug.LogWarning(
                "Room1GameOverController: Room 1 Door is not assigned!"
            );
        }

        // Restore player movement and camera controls.
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        if (playerLook != null)
        {
            playerLook.enabled = true;
            playerLook.EnableLook();
        }

        // Reset and replay the original hallway introduction.
        // This handles the instruction, welcome voice,
        // entry trigger and Room 1 interaction state.
        if (roomEntryTrigger != null)
        {
            roomEntryTrigger.ResetForHallwayRestart();
        }
        else
        {
            Debug.LogError(
                "Room1GameOverController: RoomEntryTrigger is not assigned!"
            );
        }

        // Show the Pause button again.
        if (pauseMenuController != null)
        {
            pauseMenuController.ShowPauseButton();
        }

        // Restore normal gameplay cursor state.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("ROOM 1 RESTART COMPLETE: HALLWAY");
    }
}
