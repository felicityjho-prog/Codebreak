using UnityEngine;

public class Collectible : MonoBehaviour
{
    private bool collected = false;

    private void OnMouseDown()
    {
        // ClickableObject handles items that use pickup animations.
        if (GetComponent<ClickableObject>() != null)
            return;

        if (collected)
            return;

        TaskManager taskManager =
            FindFirstObjectByType<TaskManager>();

        if (taskManager == null)
        {
            Debug.LogWarning("TaskManager not found!");
            return;
        }

        collected = true;

        taskManager.CollectObject();

        gameObject.SetActive(false);
    }

    // Called when restarting the Room 1 challenge.
    public void ResetCollectible()
    {
        collected = false;

        ClickableObject clickable =
            GetComponent<ClickableObject>();

        if (clickable != null)
        {
            clickable.ResetInteractionState();
        }

        gameObject.SetActive(true);
    }
}