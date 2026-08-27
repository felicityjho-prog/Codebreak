using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    public string itemName;
    public ChecklistManager checklistManager;

    [Header("Sound")]
    public AudioClip collectSound;

    void OnMouseDown()
    {
        // play collect sound
        AudioSource.PlayClipAtPoint(
            collectSound,
            transform.position
        );

        // collect item
        checklistManager.CollectItem(itemName);

        // mawala object
        gameObject.SetActive(false);
    }
}