using UnityEngine;

public class RoomSwitcher : MonoBehaviour
{
    [Header("Rooms")]
    public GameObject currentRoom;
    public GameObject nextRoom;

    [Header("Player")]
    public GameObject player;
    public Transform spawnPoint;

    [Header("UI")]
    public GameObject taskPanel;

    [Header("Room 2 Instruction")]
    public GameObject room2InstructionPanel;

    [Header("Checklists")]
    public GameObject currentChecklist;
    public GameObject nextChecklist;

    [Header("Game Managers")]
    public GameObject currentGameManager;
    public GameObject nextGameManager;

    [Header("Next Room Manager")]
    public GameObject nextRoomManager;

    [Header("Sudoku Interaction")]
    public Transform sudokuTable;
    public GameObject sudokuPanel;
    public float sudokuInteractionDistance = 3f;

    [Header("Settings")]
    public bool canProceed = false;

    private bool switched = false;

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E))
            return;

        Debug.Log("E Pressed");

        // ==========================================
        // ROOM 2 INSTRUCTION
        // ==========================================

        if (room2InstructionPanel != null &&
            room2InstructionPanel.activeSelf)
        {
            Debug.Log("Room 2 Instruction is still open.");
            return;
        }

        // ==========================================
        // ROOM 2 - SUDOKU INTERACTION
        // ==========================================

        if (currentRoom != null &&
            currentRoom.name == "room2" &&
            sudokuTable != null &&
            sudokuPanel != null)
        {
            float distance = Vector3.Distance(
                player.transform.position,
                sudokuTable.position
            );

            Debug.Log("Distance from Sudoku Table: " + distance);

            if (distance <= sudokuInteractionDistance)
            {
                sudokuPanel.SetActive(true);

                Debug.Log("Sudoku Panel Opened!");

                // Stop here so E does NOT switch to Room 3
                return;
            }
        }

        // ==========================================
        // NORMAL ROOM SWITCHING
        // ==========================================

        if (canProceed && !switched)
        {
            SwitchRoom();
        }
    }

    public void EnableProceed()
    {
        canProceed = true;

        Debug.Log("Room transition unlocked!");
    }

    void SwitchRoom()
    {
        Debug.Log("Switching Room...");

        switched = true;

        // ==========================================
        // Disable current room
        // ==========================================

        if (currentRoom != null)
        {
            currentRoom.SetActive(false);
        }

        // ==========================================
        // Enable next room
        // ==========================================

        if (nextRoom != null)
        {
            nextRoom.SetActive(true);
        }

        // ==========================================
        // Disable current Game Manager
        // ==========================================

        if (currentGameManager != null)
        {
            currentGameManager.SetActive(false);
        }

        // ==========================================
        // Enable next Game Manager
        // ==========================================

        if (nextGameManager != null)
        {
            nextGameManager.SetActive(true);
        }

        // ==========================================
        // TELEPORT PLAYER
        // ==========================================

        CharacterController cc =
    player.GetComponent<CharacterController>();

        if (cc != null)
        {
            cc.enabled = false;
        }

        if (spawnPoint != null)
        {
            player.transform.position = spawnPoint.position;
            player.transform.rotation = spawnPoint.rotation;
        }

        if (cc != null)
        {
            cc.enabled = true;
        }

        // ==========================================
        // Hide task panel
        // ==========================================

        if (taskPanel != null)
        {
            taskPanel.SetActive(false);
        }

        // ==========================================
        // Hide current checklist
        // ==========================================

        if (currentChecklist != null)
        {
            currentChecklist.SetActive(false);
        }

        // ==========================================
        // Show next checklist
        // ==========================================

        if (nextChecklist != null)
        {
            nextChecklist.SetActive(true);
        }

        // ==========================================
        // ROOM 2 INSTRUCTION
        // ==========================================

        if (room2InstructionPanel != null &&
            nextRoom != null &&
            nextRoom.name == "room2")
        {
            room2InstructionPanel.SetActive(true);

            Debug.Log("Room 2 Instruction Panel Shown!");
        }

        // ==========================================
        // Enable next Room Manager
        // ==========================================

        if (nextRoomManager != null)
        {
            nextRoomManager.SetActive(true);
        }

        Debug.Log("Switched to next room!");

        // Disable this RoomSwitcher
        gameObject.SetActive(false);
    }
}