using UnityEngine;

public class Room1InteractionController : MonoBehaviour
{
    [Header("Room 1 Objects")]
    public NotClickable[] room1Objects;

    private bool room1Active = false;

    public void EnableRoom1Interactions()
    {
        if (room1Active)
            return;

        room1Active = true;

        foreach (NotClickable obj in room1Objects)
        {
            if (obj != null)
            {
                obj.EnableInteraction();
            }
        }

        Debug.Log("ROOM 1 INTERACTIONS ENABLED!");
    }
}