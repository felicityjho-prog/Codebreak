using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    public string itemName;
    public ChecklistManager checklistManager;

    [Header("Sound")]
    public AudioClip collectSound;

    // Existing mouse click still works
    void OnMouseDown()
    {
        Interact();
    }

    // Can now be called by the Center Dot interaction system
    public void Interact()
    {
        // Play collect sound
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(
                collectSound,
                transform.position
            );
        }

        // Collect item
        if (checklistManager != null)
        {
            checklistManager.CollectItem(itemName);
        }

        // Hide object
        gameObject.SetActive(false);
    }
}