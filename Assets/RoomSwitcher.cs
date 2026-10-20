using UnityEngine;

public class RoomSwitcher : MonoBehaviour
{
    [Header("Rooms")]
    public GameObject currentRoom;
    public GameObject nextRoom;

    [Header("Player")]
    public GameObject player;
    public Transform spawnPoint;

    [Header("Player Controls")]
    public PlayerMovement playerMovement;
    public PlayerController playerController;

    [Header("UI")]
    public GameObject taskPanel;

    [Header("Hallway Instruction")]
    public GameObject hallwayInstruction;

    [Header("Room 2 Instruction")]
    public GameObject room2InstructionPanel;

    [Header("Room 2 Voice")]
    public Room2VoiceSubtitleManager room2VoiceManager;

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
    public SudokuManager sudokuManager;

    [Header("Teleport Transition")]
    public TeleportTransitionManager teleportTransitionManager;

    [Header("Room 1 Background Music")]
    public AudioSource room1MusicSource;

    [Header("Settings")]
    public bool canProceed = false;

    [Header("TEST MODE")]
    public bool testRoom2 = false;

    private bool switched = false;

    // ==========================================
    // START
    // ==========================================

    private void Start()
    {
        if (testRoom2)
        {
            StopRoom1Music();
            StartRoom2Test();
            return;
        }

        if (IsRoomNamed(currentRoom, "room1"))
        {
            PlayRoom1Music();
        }
        else
        {
            StopRoom1Music();
        }
    }

    // ==========================================
    // ROOM CHECK
    // ==========================================

    private bool IsRoomNamed(
        GameObject room,
        string expectedName)
    {
        return room != null &&
               string.Equals(
                   room.name,
                   expectedName,
                   System.StringComparison.OrdinalIgnoreCase
               );
    }

    // ==========================================
    // ROOM 1 BACKGROUND MUSIC
    // ==========================================

    private void PlayRoom1Music()
    {
        if (room1MusicSource == null)
        {
            Debug.LogWarning(
                "RoomSwitcher: Room 1 Music Source is not assigned.",
                this
            );
            return;
        }

        room1MusicSource.loop = true;
        room1MusicSource.spatialBlend = 0f;

        if (!room1MusicSource.isPlaying)
        {
            room1MusicSource.Play();
        }

        Debug.Log("Room 1 Background Music Started.", this);
    }

    private void StopRoom1Music()
    {
        if (room1MusicSource != null &&
            room1MusicSource.isPlaying)
        {
            room1MusicSource.Stop();

            Debug.Log(
                "Room 1 Background Music Stopped.",
                this
            );
        }
    }

    // ==========================================
    // SHOW ROOM 2 CHALLENGE PANEL
    // ==========================================

    private void ShowRoom2Instruction()
    {
        if (room2InstructionPanel == null)
        {
            Debug.LogError(
                "RoomSwitcher: Room 2 Instruction Panel " +
                "is not assigned in the Inspector.",
                this
            );
            return;
        }

        // Activate the Canvas that contains Room2ChallengePanel,
        // if that Canvas is currently inactive.
        Canvas parentCanvas =
            room2InstructionPanel.GetComponentInParent<Canvas>(true);

        if (parentCanvas != null &&
            !parentCanvas.gameObject.activeSelf)
        {
            parentCanvas.gameObject.SetActive(true);
        }

        // Show Room2ChallengePanel.
        room2InstructionPanel.SetActive(true);

        Debug.Log("ROOM 2 CHALLENGE PANEL SHOWN!", this);
    }

    // ==========================================
    // UPDATE
    // ==========================================

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E))
            return;

        Debug.Log("E Pressed");

        // Do not process E while the Room 2 instructions
        // are visible. The Got It button handles this panel.
        if (room2InstructionPanel != null &&
            room2InstructionPanel.activeInHierarchy)
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
            // Do not reopen Sudoku after completion.
            if (sudokuManager != null &&
                sudokuManager.IsPuzzleCompleted())
            {
                Debug.Log(
                    "Sudoku already completed. " +
                    "E interaction is disabled."
                );
            }
            else if (player != null)
            {
                float distance = Vector3.Distance(
                    player.transform.position,
                    sudokuTable.position
                );

                Debug.Log(
                    "Distance from Sudoku Table: " + distance
                );

                if (distance <= sudokuInteractionDistance)
                {
                    sudokuPanel.SetActive(true);

                    Debug.Log("Sudoku Panel Opened!");

                    // E is for Sudoku interaction here,
                    // not for switching to another room.
                    return;
                }
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

    // ==========================================
    // ENABLE PLAYER CONTROLS
    // ==========================================

    private void EnablePlayerControls()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        Debug.Log(
            "Player Movement + Camera Controls ENABLED."
        );
    }

    // ==========================================
    // ROOM 2 TEST MODE
    // ==========================================

    private void StartRoom2Test()
    {
        StopRoom1Music();

        Debug.Log("==========================================");
        Debug.Log("ROOM 2 TEST MODE ACTIVE");
        Debug.Log("Starting directly in Room 2...");
        Debug.Log("==========================================");

        // Disable Room 1.
        if (currentRoom != null)
        {
            currentRoom.SetActive(false);
        }

        // Enable Room 2.
        if (nextRoom != null)
        {
            nextRoom.SetActive(true);
        }

        // Switch game managers.
        if (currentGameManager != null)
        {
            currentGameManager.SetActive(false);
        }

        if (nextGameManager != null)
        {
            nextGameManager.SetActive(true);
        }

        // Teleport player to Room 2.
        if (player != null && spawnPoint != null)
        {
            CharacterController cc =
                player.GetComponent<CharacterController>();

            if (cc != null)
            {
                cc.enabled = false;
            }

            player.transform.SetPositionAndRotation(
                spawnPoint.position,
                spawnPoint.rotation
            );

            if (cc != null)
            {
                cc.enabled = true;
            }

            Debug.Log("Player moved to SpawnPointRoom2.");
        }
        else
        {
            Debug.LogWarning(
                "RoomSwitcher: Player or Room 2 Spawn Point " +
                "is not assigned."
            );
        }

        EnablePlayerControls();

        // Hide Room 1 UI.
        if (taskPanel != null)
        {
            taskPanel.SetActive(false);
        }

        if (hallwayInstruction != null)
        {
            hallwayInstruction.SetActive(false);
        }

        // Update checklists.
        if (currentChecklist != null)
        {
            currentChecklist.SetActive(false);
        }

        if (nextChecklist != null)
        {
            nextChecklist.SetActive(true);
        }

        // Show Room 2 instructions.
        ShowRoom2Instruction();

        // Enable Room 2 manager.
        if (nextRoomManager != null)
        {
            nextRoomManager.SetActive(true);
        }

        // Make Room 2 the current room.
        currentRoom = nextRoom;

        canProceed = false;
        switched = true;

        // Start Room 2 voice.
        StartRoom2Voice();

        Debug.Log("Room 2 Test Mode Ready!");
    }

    // ==========================================
    // ENABLE ROOM TRANSITION
    // ==========================================

    public void EnableProceed()
    {
        canProceed = true;

        Debug.Log("Room transition unlocked!");
    }

    // ==========================================
    // START ROOM 2 VOICE
    // ==========================================

    private void StartRoom2Voice()
    {
        if (IsRoomNamed(nextRoom, "room2"))
        {
            if (room2VoiceManager != null)
            {
                room2VoiceManager.StartVoiceSequence();

                Debug.Log(
                    "Room 2 Voice + Subtitle Started!"
                );
            }
            else
            {
                Debug.LogWarning(
                    "Room 2 Voice Manager is not assigned " +
                    "in RoomSwitcher."
                );
            }
        }
    }

    // ==========================================
    // NORMAL ROOM SWITCHING
    // ==========================================

    private void SwitchRoom()
    {
        if (switched)
            return;

        Debug.Log("Switching Room...");

        // Prevent duplicate room-switch requests.
        switched = true;

        if (teleportTransitionManager != null)
        {
            teleportTransitionManager.RunTransition(
                () => PerformRoomSwitch(false),
                StartRoom2Voice
            );
        }
        else
        {
            Debug.LogWarning(
                "Teleport Transition Manager is not assigned. " +
                "Switching rooms without a fade."
            );

            PerformRoomSwitch(true);
            StartRoom2Voice();
        }
    }

    // ==========================================
    // PERFORM ROOM SWITCH
    // ==========================================

    private void PerformRoomSwitch(
        bool enableControlsImmediately)
    {
        bool enteringRoom2 =
            IsRoomNamed(nextRoom, "room2");

        // Stop Room 1 music when entering Room 2.
        if (enteringRoom2)
        {
            StopRoom1Music();
        }

        // Disable the current room.
        if (currentRoom != null)
        {
            currentRoom.SetActive(false);
        }

        // Enable the destination room.
        if (nextRoom != null)
        {
            nextRoom.SetActive(true);
        }

        // Switch game managers.
        if (currentGameManager != null)
        {
            currentGameManager.SetActive(false);
        }

        if (nextGameManager != null)
        {
            nextGameManager.SetActive(true);
        }

        // Teleport the player while the screen is black.
        if (player != null)
        {
            CharacterController cc =
                player.GetComponent<CharacterController>();

            if (cc != null)
            {
                cc.enabled = false;
            }

            if (spawnPoint != null)
            {
                player.transform.SetPositionAndRotation(
                    spawnPoint.position,
                    spawnPoint.rotation
                );
            }
            else
            {
                Debug.LogWarning(
                    "RoomSwitcher: Spawn Point is not assigned."
                );
            }

            if (cc != null)
            {
                cc.enabled = true;
            }
        }
        else
        {
            Debug.LogError(
                "RoomSwitcher: Player reference is missing."
            );
        }

        // The transition manager restores controls after fade-in.
        if (enableControlsImmediately)
        {
            EnablePlayerControls();
        }

        // ==========================================
        // HIDE ROOM 1 TASK COMPLETED PANEL
        // ==========================================

        if (taskPanel != null)
        {
            taskPanel.SetActive(false);

            Debug.Log(
                "Room 1 Task Completed Panel Hidden."
            );
        }

        // ==========================================
        // PREPARE ROOM 2 UI
        // ==========================================

        if (enteringRoom2)
        {
            // Hide hallway instruction.
            if (hallwayInstruction != null)
            {
                hallwayInstruction.SetActive(false);
            }

            // Hide Room 1 checklist.
            if (currentChecklist != null)
            {
                currentChecklist.SetActive(false);
            }

            // Show Room 2 checklist.
            if (nextChecklist != null)
            {
                nextChecklist.SetActive(true);
            }

            // Show Room2ChallengePanel and its Canvas.
            if (room2InstructionPanel != null)
            {
                ShowRoom2Instruction();
            }

            // Enable Room 2 manager.
            if (nextRoomManager != null)
            {
                nextRoomManager.SetActive(true);
            }
        }

        // Update current room so Room 2 interactions continue working.
        currentRoom = nextRoom;

        // Prevent another room-switch request.
        canProceed = false;

        Debug.Log("Switched to next room!");
    }
}