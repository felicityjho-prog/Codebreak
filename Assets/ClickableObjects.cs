using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    public string itemName;
    public ChecklistManager checklistManager;

    [Header("Sound")]
    public AudioClip collectSound;

    void OnMouseDown()
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