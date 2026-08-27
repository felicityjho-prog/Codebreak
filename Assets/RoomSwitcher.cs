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

    [Header("Checklists")]
    public GameObject currentChecklist;
    public GameObject nextChecklist;

    [Header("Game Managers")]
    public GameObject currentGameManager;
    public GameObject nextGameManager;

    [Header("Next Room Manager")]
    public GameObject nextRoomManager;

    [Header("Settings")]
    public bool canProceed = false;

    private bool switched = false;

    void Update()
    {
        // Detect E key
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E Pressed");

            // Proceed only if allowed
            if (canProceed && !switched)
            {
                SwitchRoom();
            }
        }
    }

    // Unlock room transition
    public void EnableProceed()
    {
        canProceed = true;

        Debug.Log("Room transition unlocked!");
    }

    void SwitchRoom()
    {
        Debug.Log("Switching Room...");

        switched = true;

        // Disable current room
        if (currentRoom != null)
        {
            currentRoom.SetActive(false);
        }

        // Enable next room
        if (nextRoom != null)
        {
            nextRoom.SetActive(true);
        }

        // Disable current game manager
        if (currentGameManager != null)
        {
            currentGameManager.SetActive(false);
        }

        // Enable next game manager
        if (nextGameManager != null)
        {
            nextGameManager.SetActive(true);
        }

        // Disable character controller before teleport
        CharacterController cc = player.GetComponent<CharacterController>();

        if (cc != null)
        {
            cc.enabled = false;
        }

        // Teleport player
        if (spawnPoint != null)
        {
            player.transform.position = spawnPoint.position;
        }

        // Re-enable character controller
        if (cc != null)
        {
            cc.enabled = true;
        }

        // Hide task panel
        if (taskPanel != null)
        {
            taskPanel.SetActive(false);
        }

        // Hide current checklist
        if (currentChecklist != null)
        {
            currentChecklist.SetActive(false);
        }

        // Show next checklist
        if (nextChecklist != null)
        {
            nextChecklist.SetActive(true);
        }

        // Enable next room manager
        if (nextRoomManager != null)
        {
            nextRoomManager.SetActive(true);
        }

        Debug.Log("Switched to next room!");

        // Disable this room switcher after use
        gameObject.SetActive(false);
    }
}