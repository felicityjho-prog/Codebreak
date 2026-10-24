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

    [Header("Room 2 Completion UI")]
    public GameObject room2TaskCompletePanel;

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

    [Header("Room 1 AI Voice")]
    public AudioSource room1VoiceSource;

    [Header("Settings")]
    public bool canProceed = false;

    [Header("TEST MODE")]
    public bool testRoom2 = false;
    public bool testRoom3 = false;

    [Header("Room 3 Test Mode")]
    public GameObject room3TestRoom;
    public Transform room3SpawnPoint;
    public GameObject room3GameManager;
    public GameObject room3Checklist;

    [Header("Room 3 Instruction")]
    public Room3InstructionManager room3InstructionManager;

    private bool switched = false;

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        // Room 3 Test Mode
        if (testRoom3)
        {
            StopRoom1VoiceForRoom3Test();
            StartRoom3Test();
            return;
        }

        // Room 2 Test Mode
        if (testRoom2)
        {
            StartRoom2Test();
            return;
        }

        // Normal gameplay: start Room 1 music only when appropriate.
        if (IsRoomNamed(currentRoom, "room1"))
        {
            PlayRoom1Music();
        }

        // Do not stop Room 1 music here when this manager belongs
        // to Room 2. Another RoomSwitcher may use the same AudioSource.
    }

    // =========================================================
    // ROOM CHECK
    // =========================================================

    private bool IsRoomNamed(GameObject room, string expectedName)
    {
        return room != null &&
               string.Equals(
                   room.name,
                   expectedName,
                   System.StringComparison.OrdinalIgnoreCase
               );
    }

    // =========================================================
    // ROOM 1 BACKGROUND MUSIC
    // =========================================================

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

    // =========================================================
    // SHOW ROOM 2 INSTRUCTION
    // =========================================================

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

        Canvas parentCanvas =
            room2InstructionPanel.GetComponentInParent<Canvas>(true);

        if (parentCanvas != null &&
            !parentCanvas.gameObject.activeSelf)
        {
            parentCanvas.gameObject.SetActive(true);
        }

        room2InstructionPanel.SetActive(true);

        Debug.Log("ROOM 2 CHALLENGE PANEL SHOWN!", this);
    }

    // =========================================================
    // SHOW ROOM 3 INSTRUCTION
    // =========================================================

    private void ShowRoom3Instructions()
    {
        if (room3InstructionManager != null)
        {
            // Ensure the Room 3 Canvas is active.
            Canvas room3Canvas =
                room3InstructionManager.GetComponentInParent<Canvas>(true);

            if (room3Canvas != null &&
                !room3Canvas.gameObject.activeSelf)
            {
                room3Canvas.gameObject.SetActive(true);
            }

            // Ensure the instruction manager's GameObject is active.
            if (!room3InstructionManager.gameObject.activeSelf)
            {
                room3InstructionManager.gameObject.SetActive(true);
            }

            // This method shows the panel, disables player controls,
            // and unlocks the cursor for the OKAY button.
            room3InstructionManager.ShowInstructions();

            return;
        }

        // Fail safely if the Inspector reference is missing.
        Debug.LogError(
            "RoomSwitcher: Room 3 Instruction Manager is not assigned. " +
            "Assign Room3InstructionManager from Room3Canvas.",
            this
        );

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E))
        {
            return;
        }

        Debug.Log("E Pressed");

        // Do not process room interactions while Room 2 instructions
        // are visible.
        if (room2InstructionPanel != null &&
            room2InstructionPanel.activeInHierarchy)
        {
            Debug.Log("Room 2 Instruction is still open.");
            return;
        }

        // =====================================================
        // ROOM 2 - SUDOKU INTERACTION
        // =====================================================

        if (currentRoom != null &&
            currentRoom.name == "room2" &&
            sudokuTable != null &&
            sudokuPanel != null)
        {
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
                    return;
                }
            }
        }

        // =====================================================
        // NORMAL ROOM SWITCHING
        // =====================================================

        if (canProceed && !switched)
        {
            SwitchRoom();
        }
    }

    // =========================================================
    // ENABLE PLAYER CONTROLS
    // =========================================================

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

    // =========================================================
    // ROOM 2 TEST MODE
    // =========================================================

    private void StartRoom2Test()
    {
        StopRoom1Music();

        Debug.Log("==========================================");
        Debug.Log("ROOM 2 TEST MODE ACTIVE");
        Debug.Log("Starting directly in Room 2...");
        Debug.Log("==========================================");

        if (currentRoom != null)
        {
            currentRoom.SetActive(false);
        }

        if (nextRoom != null)
        {
            nextRoom.SetActive(true);
        }

        if (currentGameManager != null)
        {
            currentGameManager.SetActive(false);
        }

        if (nextGameManager != null)
        {
            nextGameManager.SetActive(true);
        }

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

        if (taskPanel != null)
        {
            taskPanel.SetActive(false);
        }

        if (hallwayInstruction != null)
        {
            hallwayInstruction.SetActive(false);
        }

        if (currentChecklist != null)
        {
            currentChecklist.SetActive(false);
        }

        if (nextChecklist != null)
        {
            nextChecklist.SetActive(true);
        }

        ShowRoom2Instruction();

        if (nextRoomManager != null)
        {
            nextRoomManager.SetActive(true);
        }

        currentRoom = nextRoom;

        canProceed = false;
        switched = true;

        StartRoom2Voice();

        Debug.Log("Room 2 Test Mode Ready!");
    }

    // =========================================================
    // ROOM 3 TEST MODE
    // =========================================================

    private void StartRoom3Test()
    {
        Debug.Log("==========================================");
        Debug.Log("ROOM 3 TEST MODE ACTIVE");
        Debug.Log("Starting directly in Room 3...");
        Debug.Log("==========================================");

        StopRoom1Music();

        // Verify essential Room 3 references before switching.
        if (player == null ||
            room3TestRoom == null ||
            room3SpawnPoint == null)
        {
            Debug.LogError(
                "ROOM 3 TEST MODE FAILED. Assign Player, " +
                "Room 3 Test Room, and Room 3 Spawn Point " +
                "in the RoomSwitcher Inspector.",
                this
            );
            return;
        }

        // Stop the Room 2 global timer if it is running.
        if (Room2ChallengeTimer.Instance != null)
        {
            Room2ChallengeTimer.Instance.StopTimer();
        }

        // Disable Room 1 and Room 2.
        if (currentRoom != null &&
            currentRoom != room3TestRoom)
        {
            currentRoom.SetActive(false);
        }

        if (nextRoom != null &&
            nextRoom != room3TestRoom)
        {
            nextRoom.SetActive(false);
        }

        // Enable Room 3.
        room3TestRoom.SetActive(true);

        // Disable Room 1 and Room 2 game managers.
        if (currentGameManager != null)
        {
            currentGameManager.SetActive(false);
        }

        if (nextGameManager != null)
        {
            nextGameManager.SetActive(false);
        }

        // Enable Room 3 game manager.
        if (room3GameManager != null)
        {
            room3GameManager.SetActive(true);
        }
        else
        {
            Debug.LogWarning(
                "Room 3 Game Manager is not assigned. " +
                "Assign GameManagerRoom3 if Room 3 requires it.",
                this
            );
        }

        // Disable the Room 2 -> Room 3 manager while testing directly.
        if (nextRoomManager != null &&
            nextRoomManager != gameObject)
        {
            nextRoomManager.SetActive(false);
        }

        // Teleport player to Room 3.
        CharacterController cc =
            player.GetComponent<CharacterController>();

        if (cc != null)
        {
            cc.enabled = false;
        }

        player.transform.SetPositionAndRotation(
            room3SpawnPoint.position,
            room3SpawnPoint.rotation
        );

        if (cc != null)
        {
            cc.enabled = true;
        }

        // IMPORTANT:
        // Do not enable movement/camera here.
        // Room 3 instructions must open and freeze controls until OKAY.
        ShowRoom3Instructions();

        // Hide Room 1 task UI and hallway instructions.
        if (taskPanel != null)
        {
            taskPanel.SetActive(false);
        }

        if (hallwayInstruction != null)
        {
            hallwayInstruction.SetActive(false);
        }

        // Hide Room 2 instruction panel and its Canvas.
        Canvas room2Canvas = null;

        if (room2InstructionPanel != null)
        {
            room2Canvas =
                room2InstructionPanel.GetComponentInParent<Canvas>(true);

            room2InstructionPanel.SetActive(false);
        }

        if (room2Canvas != null)
        {
            room2Canvas.gameObject.SetActive(false);
        }

        // Hide Room 1 and Room 2 checklists.
        if (currentChecklist != null)
        {
            currentChecklist.SetActive(false);
        }

        if (nextChecklist != null)
        {
            nextChecklist.SetActive(false);
        }

        // Enable the optional Room 3 checklist.
        if (room3Checklist != null)
        {
            room3Checklist.SetActive(true);
        }

        // Update runtime state.
        currentRoom = room3TestRoom;
        canProceed = false;
        switched = true;

        Debug.Log(
            "ROOM 3 TEST MODE READY. " +
            "Player teleported to Room 3 spawn."
        );
    }

    // =========================================================
    // ENABLE ROOM TRANSITION
    // =========================================================

    public void EnableProceed()
    {
        canProceed = true;

        Debug.Log("Room transition unlocked!");
    }

    // =========================================================
    // START ROOM 2 VOICE
    // =========================================================

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
                    "in RoomSwitcher.",
                    this
                );
            }
        }
    }

    // =========================================================
    // NORMAL ROOM SWITCHING
    // =========================================================

    private void SwitchRoom()
    {
        if (switched)
        {
            return;
        }

        Debug.Log("Switching Room...");

        switched = true;

        // Hide Room 2 completion panel before leaving Room 2.
        if (IsRoomNamed(nextRoom, "room3") &&
            room2TaskCompletePanel != null)
        {
            room2TaskCompletePanel.SetActive(false);
        }

        if (teleportTransitionManager != null)
        {
            teleportTransitionManager.RunTransition(
                () => PerformRoomSwitch(false),
                OnRoomTransitionComplete
            );
        }
        else
        {
            Debug.LogWarning(
                "Teleport Transition Manager is not assigned. " +
                "Switching without fade.",
                this
            );

            PerformRoomSwitch(false);
            OnRoomTransitionComplete();
        }
    }

    // =========================================================
    // PERFORM ROOM SWITCH
    // =========================================================

    private void PerformRoomSwitch(bool enableControlsImmediately)
    {
        bool enteringRoom2 = IsRoomNamed(nextRoom, "room2");
        bool enteringRoom3 = IsRoomNamed(nextRoom, "room3");

        if (enteringRoom2)
        {
            StopRoom1Music();
        }

        // Disable the current room.
        if (currentRoom != null)
        {
            currentRoom.SetActive(false);
        }

        // Hide Room 2 completion panel when entering Room 3.
        if (enteringRoom3 && room2TaskCompletePanel != null)
        {
            room2TaskCompletePanel.SetActive(false);

            Debug.Log(
                "Room 2 Task Completion panel hidden after entering Room 3."
            );
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
                    "RoomSwitcher: Spawn Point is not assigned.",
                    this
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
                "RoomSwitcher: Player reference is missing.",
                this
            );
        }

        if (enableControlsImmediately)
        {
            EnablePlayerControls();
        }

        // Hide Room 1 task completion panel.
        if (taskPanel != null)
        {
            taskPanel.SetActive(false);

            Debug.Log(
                "Room 1 Task Completed Panel Hidden."
            );
        }

        // Prepare Room 2 UI if entering Room 2.
        if (enteringRoom2)
        {
            if (hallwayInstruction != null)
            {
                hallwayInstruction.SetActive(false);
            }

            if (currentChecklist != null)
            {
                currentChecklist.SetActive(false);
            }

            if (nextChecklist != null)
            {
                nextChecklist.SetActive(true);
            }

            if (room2InstructionPanel != null)
            {
                ShowRoom2Instruction();
            }

            if (nextRoomManager != null)
            {
                nextRoomManager.SetActive(true);
            }
        }

        currentRoom = nextRoom;
        canProceed = false;

        Debug.Log("Switched to next room!");
    }

    // =========================================================
    // STOP ROOM 1 VOICE FOR ROOM 3 TEST
    // =========================================================

    private void StopRoom1VoiceForRoom3Test()
    {
        if (room1VoiceSource == null)
        {
            Debug.LogWarning(
                "Room 1 Voice Source is not assigned.",
                this
            );
            return;
        }

        room1VoiceSource.Stop();
        room1VoiceSource.enabled = false;

        Debug.Log(
            "Room 1 AI Voice stopped for Room 3 Test Mode."
        );
    }

    // =========================================================
    // TRANSITION COMPLETE
    // =========================================================

    private void OnRoomTransitionComplete()
    {
        // Room 3: show instructions and keep controls disabled.
        // The OKAY button restores them through HideInstructions().
        if (IsRoomNamed(currentRoom, "room3"))
        {
            ShowRoom3Instructions();
        }
        else
        {
            // Preserve the existing Room 1 -> Room 2 behavior.
            if (playerMovement != null)
            {
                playerMovement.enabled = true;
            }
            else
            {
                Debug.LogWarning(
                    "PlayerMovement is not assigned in RoomSwitcher.",
                    this
                );
            }

            if (playerController != null)
            {
                playerController.enabled = true;
                playerController.EnableLook();
            }
            else
            {
                Debug.LogWarning(
                    "PlayerController is not assigned in RoomSwitcher.",
                    this
                );
            }

            // Restore the normal first-person cursor.
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Start Room 2 voice only when the destination is Room 2.
        StartRoom2Voice();

        Debug.Log(
            IsRoomNamed(currentRoom, "room3")
                ? "ROOM 3 TRANSITION COMPLETE: Instructions shown; controls frozen."
                : "ROOM TRANSITION COMPLETE: Player controls restored."
        );
    }
}
