using UnityEngine;

public class Room3Trigger : MonoBehaviour
{
    public GameObject panel;

    public RoomSwitcher roomSwitcher;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            panel.SetActive(true);

            roomSwitcher.canProceed = true;
        }
    }
}