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

    // Start the hallway sequence when needed.
    private void Start()
    {
        // Skip the Room 1 setup during Room 2 testing.
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

        // Wait for the opening cinematic before showing the hallway intro.
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

            SetHallwayUIHidden(true);
            return;
        }

        BeginHallwaySequence();
    }

    // Keep the HUD hidden while the intro is active.
    private void Update()
    {
        // Room 2 testing should not use the Room 1 HUD lock.
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

    // Hide or restore the player's HUD.
    private void SetHallwayUIHidden(bool hide)
    {
        if (hallwayUIHidden == hide)
            return;

        hallwayUIHidden = hide;

        if (hide)
        {
            // Save the current HUD state before hiding it.
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

            if (playerControlsUI != null)
                playerControlsUI.SetActive(false);

            if (centerDotUI != null)
                centerDotUI.SetActive(false);

            if (pauseMenuController != null)
                pauseMenuController.SetCinematicLock(true);

            Debug.Log(
                "Hallway cinematic active: HUD and pause menu hidden."
            );
        }
        else
        {
            // Bring the HUD back to its previous state.
            if (hudStatesCached)
            {
                if (playerControlsUI != null)
                    playerControlsUI.SetActive(controlsOriginalState);

                if (centerDotUI != null)
                    centerDotUI.SetActive(centerDotOriginalState);

                hudStatesCached = false;
            }

            if (pauseMenuController != null)
                pauseMenuController.SetCinematicLock(false);

            Debug.Log(
                "Hallway sequence finished: HUD visibility restored."
            );
        }
    }

    // Show the hallway intro and play its voice-over.
    public void BeginHallwaySequence()
    {
        if (roomSwitcher != null && roomSwitcher.testRoom2)
            return;

        if (triggered)
            return;

        StopAllCoroutines();

        if (hallwayInstruction != null)
            hallwayInstruction.SetActive(true);

        if (challengeUI != null)
            challengeUI.SetActive(false);

        if (instructionPanel != null)
            instructionPanel.SetActive(false);

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

    // Hide the hallway message after the voice-over ends.
    private IEnumerator WaitForVoiceToFinish()
    {
        while (voiceOver != null && voiceOver.isPlaying)
            yield return null;

        if (!triggered && hallwayInstruction != null)
            hallwayInstruction.SetActive(false);
    }

    // Start the Room 1 challenge when the player enters.
    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        // Do not let the player enter while the opening cinematic is active.
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

    // Reset the hallway and Room 1 setup.
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
