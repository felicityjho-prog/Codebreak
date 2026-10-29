using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ClickableObject : MonoBehaviour
{
    [Header("Room 1 Item")]
    public string itemName;
    public ChecklistManager checklistManager;

    [Header("Pickup Animation")]
    public Animator pickupAnimator;
    public bool playPickupAnimation = true;
    public string pickupTriggerName = "Pickup";
    public float pickupAnimationDuration = 3.43f;

    [Header("Sound")]
    public AudioClip collectSound;

    [Range(0f, 1f)]
    public float collectVolume = 1f;

    private bool interactionInProgress;

    private void OnMouseDown()
    {
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (WillPlayCollectSound())
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

    public void Interact()
    {
        Debug.Log(
            $"[PICKUP DEBUG] Interact: {itemName}"
        );

        if (interactionInProgress)
            return;

        if (checklistManager == null)
        {
            Debug.LogWarning(
                $"ChecklistManager missing on {name}!", this
            );
            return;
        }

        if (!checklistManager.CanCollectItem(itemName))
        {
            Debug.LogWarning(
                $"Item cannot be collected: {itemName}", this
            );
            return;
        }

        StartCoroutine(CollectAfterPickup());
    }

    private IEnumerator CollectAfterPickup()
    {
        interactionInProgress = true;

        bool hasCollectible =
            GetComponent<Collectible>() != null;

        bool canAnimate =
            playPickupAnimation &&
            pickupAnimator != null &&
            HasPickupTrigger();

        if (playPickupAnimation && !canAnimate)
        {
            Debug.LogWarning(
                $"Pickup animation unavailable on {name}. " +
                "Check the Animator and Pickup trigger.",
                this
            );
        }

        // Play animation before collecting the item.
        if (canAnimate)
        {
            pickupAnimator.ResetTrigger(pickupTriggerName);
            pickupAnimator.SetTrigger(pickupTriggerName);

            Debug.Log(
                $"[PICKUP DEBUG] Animation triggered: {itemName}"
            );

            yield return new WaitForSeconds(
                Mathf.Max(0f, pickupAnimationDuration)
            );
        }

        // Revalidate before completing collection.
        if (checklistManager == null ||
            !checklistManager.CanCollectItem(itemName))
        {
            Debug.LogWarning(
                $"[PICKUP DEBUG] Collection cancelled: {itemName}"
            );

            interactionInProgress = false;
            yield break;
        }

        // Preserve checklist behavior.
        checklistManager.CollectItem(itemName);

        // Preserve TaskManager counting for objects that
        // originally had a Collectible component.
        if (hasCollectible)
        {
            TaskManager taskManager =
                FindFirstObjectByType<TaskManager>();

            if (taskManager != null)
            {
                taskManager.CollectObject();
            }
            else
            {
                Debug.LogWarning("TaskManager not found!");
            }
        }

        // Play collection sound once, after successful collection.
        if (collectSound != null)
        {
            PlayCollectSound();
        }

        Debug.Log(
            $"[PICKUP DEBUG] Collection completed: {itemName}"
        );

        gameObject.SetActive(false);
    }

    private bool HasPickupTrigger()
    {
        if (pickupAnimator == null ||
            pickupAnimator.runtimeAnimatorController == null)
        {
            return false;
        }

        foreach (AnimatorControllerParameter parameter
                 in pickupAnimator.parameters)
        {
            if (parameter.name == pickupTriggerName &&
                parameter.type ==
                AnimatorControllerParameterType.Trigger)
            {
                return true;
            }
        }

        return false;
    }

    public void ResetInteractionState()
    {
        StopAllCoroutines();
        interactionInProgress = false;
    }

    private void PlayCollectSound()
    {
        GameObject soundObject =
            new GameObject("CollectSound2D");

        AudioSource source =
            soundObject.AddComponent<AudioSource>();

        source.clip = collectSound;
        source.volume = collectVolume;
        source.pitch = 1f;
        source.loop = false;
        source.playOnAwake = false;
        source.spatialBlend = 0f;

        source.Play();

        Destroy(
            soundObject,
            collectSound.length + 0.1f
        );
    }
}