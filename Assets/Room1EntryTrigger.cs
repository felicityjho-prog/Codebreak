using UnityEngine;
using System.Collections;

public class RoomEntryTrigger : MonoBehaviour
{
    [Header("Hallway")]
    public GameObject hallwayInstruction;

    [Header("Room 1 Challenge")]
    public GameObject challengeUI;

    [Header("Room 1 Instruction")]
    public GameObject instructionPanel;

    [Header("Timer")]
    public GameTimer gameTimer;

    [Header("Voice Over")]
    public AudioSource voiceOver;

    [Header("Room 1 Interaction")]
    public Room1InteractionController room1Interaction;

    [Header("Test Mode")]
    public RoomSwitcher roomSwitcher;

    [Header("Opening Cinematic")]
    public OpeningCinematicController openingCinematic;

    [Header("Hallway Instruction Animation")]
    public HallwayInstructionController hallwayInstructionController;

    [Header("HUD Hidden During Cinematic / Hallway Instruction")]
    public GameObject playerControlsUI;
    public GameObject centerDotUI;
    public PauseMenuController pauseMenuController;

    private bool triggered = false;

    private bool hallwayUIHidden;
    private bool hudStatesCached;

    private bool controlsOriginalState;
    private bool centerDotOriginalState;

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        // Do not affect Room 1 HUD during Room 2 Test Mode.
        if (roomSwitcher != null && roomSwitcher.testRoom2)
        {
            if (voiceOver != null)
                voiceOver.Stop();

            if (hallwayInstruction != null)
                hallwayInstruction.SetActive(false);

            if (challengeUI != null)
                challengeUI.SetActive(false);

            if (instructionPanel != null)
                instructionPanel.SetActive(false);

            SetHallwayUIHidden(false);
            return;
        }

        // The opening cinematic will start the hallway sequence later.
        if (openingCinematic != null &&
            openingCinematic.ShouldDelayHallwaySequence)
        {
            if (voiceOver != null)
                voiceOver.Stop();

            if (hallwayInstruction != null)
                hallwayInstruction.SetActive(false);

            if (challengeUI != null)
                challengeUI.SetActive(false);

            if (instructionPanel != null)
                instructionPanel.SetActive(false);

            // Hide the HUD immediately during the cinematic.
            SetHallwayUIHidden(true);
            return;
        }

        BeginHallwaySequence();
    }

    // =====================================================
    // UPDATE
    // Detect when the opening cinematic or hallway sequence
    // requires the HUD and pause menu to stay hidden.
    // =====================================================

    private void Update()
    {
        // Room 2 Test Mode must not use Room 1 HUD locking.
        if (roomSwitcher != null && roomSwitcher.testRoom2)
        {
            SetHallwayUIHidden(false);
            return;
        }

        bool openingCinematicRunning =
            openingCinematic != null &&
            openingCinematic.ShouldDelayHallwaySequence;

        bool hallwayVoicePlaying =
            voiceOver != null &&
            voiceOver.isPlaying;

        bool hallwayInstructionVisible =
            hallwayInstruction != null &&
            hallwayInstruction.activeInHierarchy;

        bool animatedInstructionVisible =
            hallwayInstructionController != null &&
            hallwayInstructionController.instructionPanel != null &&
            hallwayInstructionController.instructionPanel.activeInHierarchy;

        bool shouldHideHUD =
            !triggered &&
            (
                openingCinematicRunning ||
                hallwayVoicePlaying ||
                hallwayInstructionVisible ||
                animatedInstructionVisible
            );

        SetHallwayUIHidden(shouldHideHUD);
    }

    // =====================================================
    // HIDE / RESTORE HUD
    // =====================================================

    private void SetHallwayUIHidden(bool hide)
    {
        if (hallwayUIHidden == hide)
            return;

        hallwayUIHidden = hide;

        if (hide)
        {
            // Cache the original HUD visibility only once.
            if (!hudStatesCached)
            {
                controlsOriginalState =
                    playerControlsUI != null &&
                    playerControlsUI.activeSelf;

                centerDotOriginalState =
                    centerDotUI != null &&
                    centerDotUI.activeSelf;

                hudStatesCached = true;
            }

            // Hide the keyboard and mouse control labels.
            if (playerControlsUI != null)
                playerControlsUI.SetActive(false);

            // Hide the center dot / crosshair.
            if (centerDotUI != null)
                centerDotUI.SetActive(false);

            // Hide and lock the pause menu.
            if (pauseMenuController != null)
                pauseMenuController.SetCinematicLock(true);

            Debug.Log(
                "Hallway cinematic active: HUD and pause menu hidden."
            );
        }
        else
        {
            // Restore the same visibility states as before.
            if (hudStatesCached)
            {
                if (playerControlsUI != null)
                    playerControlsUI.SetActive(controlsOriginalState);

                if (centerDotUI != null)
                    centerDotUI.SetActive(centerDotOriginalState);

                hudStatesCached = false;
            }

            // Allow the pause menu to work again.
            if (pauseMenuController != null)
                pauseMenuController.SetCinematicLock(false);

            Debug.Log(
                "Hallway sequence finished: HUD visibility restored."
            );
        }
    }

    // =====================================================
    // BEGIN HALLWAY SEQUENCE
    // =====================================================

    public void BeginHallwaySequence()
    {
        if (roomSwitcher != null && roomSwitcher.testRoom2)
            return;

        if (triggered)
            return;

        // Avoid multiple welcome voice coroutines.
        StopAllCoroutines();

        if (hallwayInstruction != null)
            hallwayInstruction.SetActive(true);

        if (challengeUI != null)
            challengeUI.SetActive(false);

        if (instructionPanel != null)
            instructionPanel.SetActive(false);

        // Hide the HUD before the voice begins.
        SetHallwayUIHidden(true);

        if (voiceOver != null)
        {
            voiceOver.Stop();

            if (voiceOver.clip != null)
            {
                voiceOver.Play();
                StartCoroutine(WaitForVoiceToFinish());
            }
        }
    }

    // =====================================================
    // WAIT FOR WELCOME VOICE
    // =====================================================

    private IEnumerator WaitForVoiceToFinish()
    {
        while (voiceOver != null && voiceOver.isPlaying)
            yield return null;

        if (!triggered && hallwayInstruction != null)
            hallwayInstruction.SetActive(false);

        // Update() will restore the HUD when the cinematic
        // and any active hallway instruction visuals are done.
    }

    // =====================================================
    // ENTER ROOM 1
    // =====================================================

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        // Prevent entry during the opening cinematic.
        if (openingCinematic != null &&
            openingCinematic.ShouldDelayHallwaySequence)
        {
            return;
        }

        if (roomSwitcher != null && roomSwitcher.testRoom2)
            return;

        triggered = true;

        Debug.Log("PLAYER ENTERED ROOM 1!");

        if (hallwayInstruction != null)
            hallwayInstruction.SetActive(false);

        if (voiceOver != null && voiceOver.isPlaying)
            voiceOver.Stop();

        // Restore HUD and unlock pause functionality.
        SetHallwayUIHidden(false);

        if (challengeUI != null)
            challengeUI.SetActive(true);

        if (instructionPanel != null)
            instructionPanel.SetActive(true);

        if (gameTimer != null)
            gameTimer.StartTimer();

        if (room1Interaction != null)
            room1Interaction.EnableRoom1Interactions();
    }

    // =====================================================
    // RESET HALLWAY SEQUENCE
    // =====================================================

    public void ResetForHallwayRestart()
    {
        if (roomSwitcher != null && roomSwitcher.testRoom2)
            return;

        StopAllCoroutines();

        triggered = false;

        if (room1Interaction != null)
            room1Interaction.ResetRoom1Interactions();

        if (gameTimer != null)
            gameTimer.StopTimer();

        if (hallwayInstruction != null)
            hallwayInstruction.SetActive(true);

        if (challengeUI != null)
            challengeUI.SetActive(false);

        if (instructionPanel != null)
            instructionPanel.SetActive(false);

        // Hide HUD again during the restarted sequence.
        SetHallwayUIHidden(true);

        if (voiceOver != null)
        {
            voiceOver.Stop();

            if (voiceOver.clip != null)
            {
                voiceOver.Play();
                StartCoroutine(WaitForVoiceToFinish());
            }
        }

        Debug.Log("ROOM 1 HALLWAY SEQUENCE RESET!");
    }
}