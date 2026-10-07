using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class AutoDoor : MonoBehaviour
{
    public Transform doorPivot;
    public float openAngle = -90f;
    public float speed = 2f;
    public bool openRight = true;

    public Collider doorCollider;

    // Enable only for doors whose trigger reports player occupancy separately.
    // This keeps existing AutoDoor instances on other doors on their legacy behavior.
    public bool occupancyControlled = false;
    public float openSpeedDegreesPerSecond = 60f;
    public float closeSpeedDegreesPerSecond = 60f;
    public float closeGracePeriod = 0.45f;

    [Header("One-time manual interaction")]
    public bool manualOneTime = false;
    public float interactionDistance = 3.8f;
    public GameObject interactionPrompt;

    private Transform playerTransform;
    private TMP_Text interactionPromptText;
    private bool hasOpenedManually = false;

    
private Quaternion closedRotation;
    private Quaternion openRotation;

    private bool isOpen = false;
    private bool playerInside = false;
    private float closeTimer = 0f;

void Start()
    {
        if (doorPivot == null)
        {
            enabled = false;
            return;
        }

        closedRotation = doorPivot.localRotation;

        float direction = openRight ? 1f : -1f;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle * direction, 0);

        if (manualOneTime)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
                playerTransform = playerObject.transform;

            if (interactionPrompt != null)
            {
                interactionPromptText = interactionPrompt.GetComponentInChildren<TMP_Text>(true);
                if (interactionPromptText != null)
                    interactionPromptText.text = "[E] TO OPEN";
                interactionPrompt.SetActive(false);
            }
        }
    }

void Update()
    {
        if (manualOneTime)
        {
            UpdateManualOneTimeDoor();
            return;
        }

        if (occupancyControlled)
        {
            UpdateOccupancyControlledDoor();
            return;
        }

        if (isOpen)
        {
            doorPivot.localRotation = Quaternion.Lerp(
                doorPivot.localRotation,
                openRotation,
                Time.deltaTime * speed
            );
        }
        else
        {
            doorPivot.localRotation = Quaternion.Lerp(
                doorPivot.localRotation,
                closedRotation,
                Time.deltaTime * speed
            );
        }
    }

private void UpdateManualOneTimeDoor()
    {
        if (doorPivot == null)
            return;

        if (!hasOpenedManually)
        {
            if (playerTransform == null)
            {
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null)
                    playerTransform = playerObject.transform;
            }

            bool playerInRange = playerTransform != null &&
                Vector3.Distance(playerTransform.position, doorPivot.position) <= Mathf.Max(0f, interactionDistance);

            if (interactionPrompt != null)
                interactionPrompt.SetActive(playerInRange);

            if (playerInRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                hasOpenedManually = true;
                if (interactionPrompt != null)
                    interactionPrompt.SetActive(false);
            }
        }
        else if (interactionPrompt != null && interactionPrompt.activeSelf)
        {
            interactionPrompt.SetActive(false);
        }

        if (hasOpenedManually)
        {
            doorPivot.localRotation = Quaternion.RotateTowards(
                doorPivot.localRotation,
                openRotation,
                Mathf.Max(0f, openSpeedDegreesPerSecond) * Time.deltaTime
            );
        }
    }


    private void UpdateOccupancyControlledDoor()
    {
        if (doorPivot == null)
            return;

        bool shouldBeOpen = playerInside;
        if (!playerInside && closeTimer > 0f)
        {
            closeTimer = Mathf.Max(0f, closeTimer - Time.deltaTime);
            shouldBeOpen = closeTimer > 0f;
        }

        Quaternion targetRotation = shouldBeOpen ? openRotation : closedRotation;
        float rotationSpeed = shouldBeOpen ? openSpeedDegreesPerSecond : closeSpeedDegreesPerSecond;
        doorPivot.localRotation = Quaternion.RotateTowards(
            doorPivot.localRotation,
            targetRotation,
            Mathf.Max(0f, rotationSpeed) * Time.deltaTime
        );
    }

public void SetPlayerOccupied(bool occupied)
    {
        if (manualOneTime || !occupancyControlled || playerInside == occupied)
            return;

        playerInside = occupied;
        if (occupied)
        {
            closeTimer = 0f;
            if (doorCollider != null)
                doorCollider.enabled = false;
        }
        else
        {
            closeTimer = Mathf.Max(0f, closeGracePeriod);
            if (doorCollider != null)
                doorCollider.enabled = true;
        }
    }

void OnTriggerEnter(Collider other)
    {
        if (manualOneTime || occupancyControlled)
            return;

        if (other.CompareTag("Player"))
        {
            isOpen = true;
            if (doorCollider != null)
                doorCollider.enabled = false;
        }
    }

void OnTriggerExit(Collider other)
    {
        if (manualOneTime || occupancyControlled)
            return;

        if (other.CompareTag("Player"))
        {
            isOpen = false;
            if (doorCollider != null)
                doorCollider.enabled = true;
        }
    }
}
