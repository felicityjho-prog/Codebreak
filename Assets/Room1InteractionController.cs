
using UnityEngine;

public class Room1InteractionController : MonoBehaviour
{
    [Header("Room 1 Objects")]
    public NotClickable[] room1Objects;

    private bool room1Active = false;

    // ==========================================
    // ENABLE ROOM 1 INTERACTIONS
    // ==========================================

    public void EnableRoom1Interactions()
    {
        if (room1Active)
            return;

        room1Active = true;

        if (room1Objects != null)
        {
            foreach (NotClickable obj in room1Objects)
            {
                if (obj != null)
                {
                    obj.EnableInteraction();
                }
            }
        }

        Debug.Log("ROOM 1 INTERACTIONS ENABLED!");
    }

    // ==========================================
    // RESET ROOM 1 INTERACTIONS
    // ==========================================

    public void ResetRoom1Interactions()
    {
        // Allow interactions to be enabled again on re-entry.
        room1Active = false;

        // Disable each interaction until the player re-enters Room 1.
        if (room1Objects != null)
        {
            foreach (NotClickable obj in room1Objects)
            {
                if (obj != null)
                {
                    obj.DisableInteraction();
                }
            }
        }

        Debug.Log("ROOM 1 INTERACTIONS RESET!");
    }
}
