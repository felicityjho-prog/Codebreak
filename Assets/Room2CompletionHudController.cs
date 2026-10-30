
using System;
using UnityEngine;

public class Room2CompletionHudController : MonoBehaviour
{
    [Header("GlobalCanvas HUD Objects")]
    [SerializeField] private GameObject centerDot;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject pauseButtonRoom1;

    [Header("Room 2 Timer")]
    [SerializeField] private GameObject room2TimerText;

    [Header("Room 3 Instruction Gate")]
    [SerializeField] private GameObject room3InstructionPanel;

    private RoomSwitcher roomSwitcherToWatch;

    private bool hiddenForRoom2Completion;
    private bool centerDotWasActive;
    private bool controlsPanelWasActive;
    private bool pauseButtonWasActive;
    private bool warnedMissingRoom3Panel;

    // =========================================================
    // HIDE HUD AFTER ROOM 2 COMPLETION
    // =========================================================

    public void HideForRoom2Completion(RoomSwitcher roomSwitcher)
    {
        if (hiddenForRoom2Completion)
        {
            roomSwitcherToWatch = roomSwitcher;
            return;
        }

        // Save original HUD visibility.
        centerDotWasActive =
            centerDot != null && centerDot.activeSelf;

        controlsPanelWasActive =
            controlsPanel != null && controlsPanel.activeSelf;

        pauseButtonWasActive =
            pauseButtonRoom1 != null && pauseButtonRoom1.activeSelf;

        roomSwitcherToWatch = roomSwitcher;
        hiddenForRoom2Completion = true;
        warnedMissingRoom3Panel = false;

        // Hide GlobalCanvas HUD.
        SetHudVisible(false);

        // Hide Room 2 timer text.
        if (room2TimerText != null)
        {
            room2TimerText.SetActive(false);
        }

        Debug.Log(
            "ROOM 2 COMPLETED: HUD and Room 2 Timer hidden.",
            this
        );

        if (roomSwitcherToWatch == null)
        {
            Debug.LogWarning(
                "RoomSwitcher is missing. HUD will remain hidden.",
                this
            );
        }

        if (room3InstructionPanel == null)
        {
            Debug.LogWarning(
                "Room 3 Instruction Panel is not assigned. " +
                "HUD restoration is waiting for this reference.",
                this
            );
        }
    }

    // =========================================================
    // MONITOR ROOM 3 INSTRUCTION PANEL
    // =========================================================

    private void Update()
    {
        if (!hiddenForRoom2Completion ||
            roomSwitcherToWatch == null)
        {
            return;
        }

        GameObject activeRoom =
            roomSwitcherToWatch.currentRoom;

        if (activeRoom == null)
            return;

        // Wait until Room 3 becomes the current room.
        if (!string.Equals(
            activeRoom.name,
            "room3",
            StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        // Keep HUD hidden if the reference is missing.
        if (room3InstructionPanel == null)
        {
            if (!warnedMissingRoom3Panel)
            {
                warnedMissingRoom3Panel = true;

                Debug.LogWarning(
                    "Room 3 Instruction Panel is not assigned. " +
                    "HUD remains hidden.",
                    this
                );
            }

            return;
        }

        // Keep HUD hidden while Room 3 instructions are visible.
        if (room3InstructionPanel.activeInHierarchy)
            return;

        // Restore GlobalCanvas HUD after instructions close.
        RestoreHud();
    }

    // =========================================================
    // SHOW / HIDE HUD
    // =========================================================

    private void SetHudVisible(bool visible)
    {
        if (centerDot != null)
            centerDot.SetActive(visible);

        if (controlsPanel != null)
            controlsPanel.SetActive(visible);

        if (pauseButtonRoom1 != null)
            pauseButtonRoom1.SetActive(visible);
    }

    // =========================================================
    // RESTORE HUD
    // =========================================================

    public void RestoreHud()
    {
        if (!hiddenForRoom2Completion)
            return;

        // Restore original GlobalCanvas HUD visibility.
        if (centerDot != null)
            centerDot.SetActive(centerDotWasActive);

        if (controlsPanel != null)
            controlsPanel.SetActive(controlsPanelWasActive);

        if (pauseButtonRoom1 != null)
            pauseButtonRoom1.SetActive(pauseButtonWasActive);

        // IMPORTANT:
        // Do not restore Room 2 timer in Room 3.

        hiddenForRoom2Completion = false;
        roomSwitcherToWatch = null;
        warnedMissingRoom3Panel = false;

        Debug.Log(
            "ROOM 3 INSTRUCTION CLOSED: Global HUD restored. " +
            "Room 2 Timer remains hidden.",
            this
        );
    }
}
