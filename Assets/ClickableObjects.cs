
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableObject : MonoBehaviour
{
    public string itemName;
    public ChecklistManager checklistManager;

    [Header("Sound")]
    public AudioClip collectSound;

    [Range(0f, 1f)]
    public float collectVolume = 1f;

    private void OnMouseDown()
    {
        // Don't suppress normal clicks when the pointer is over UI.
        bool pointerOverUI =
            EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject();

        // Let a valid item with its own sound take priority.
        if (!pointerOverUI && WillPlayCollectSound())
        {
            MouseClickSound.SuppressClickSoundForCurrentFrame();
        }

        Interact();
    }

    public bool WillPlayCollectSound()
    {
        return collectSound != null &&
               checklistManager != null &&
               checklistManager.CanCollectItem(itemName);
    }

    // Can also be called by the Center Dot interaction system.
    public void Interact()
    {
        bool shouldPlayCollectSound = WillPlayCollectSound();

        // Preserve the existing checklist behavior.
        if (checklistManager != null)
        {
            checklistManager.CollectItem(itemName);
        }

        // Play the item's sound only when it is valid.
        if (shouldPlayCollectSound)
        {
            PlayCollectSound();
        }

        // Preserve the existing object hiding behavior.
        gameObject.SetActive(false);
    }

    private void PlayCollectSound()
    {
        GameObject soundObject = new GameObject("CollectSound2D");

        AudioSource source = soundObject.AddComponent<AudioSource>();

        source.clip = collectSound;
        source.volume = collectVolume;
        source.pitch = 1f;
        source.loop = false;
        source.playOnAwake = false;

        // 2D audio avoids distance-based volume reduction.
        source.spatialBlend = 0f;

        source.Play();

        Destroy(soundObject, collectSound.length + 0.1f);
    }
}
