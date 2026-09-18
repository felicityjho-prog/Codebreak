using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public GameObject taskCompletePanel;

    public GameObject checklistUI;

    public RoomSwitcher roomSwitcher;

    public GameTimer gameTimer;

    [SerializeField] private int totalObjects;

    private int collectedObjects = 0;

    public void CollectObject()
    {
        collectedObjects++;

        if (collectedObjects >= totalObjects)
        {
            // Stop timer and ticking sound
            if (gameTimer != null)
            {
                gameTimer.StopTimer();
            }

            checklistUI.SetActive(false);

            taskCompletePanel.SetActive(true);

            roomSwitcher.EnableProceed();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}