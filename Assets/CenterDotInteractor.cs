using UnityEngine;
using UnityEngine.InputSystem;

public class CenterDotInteractor : MonoBehaviour
{
    [Header("Interaction")]
    public Camera playerCamera;
    public float interactionDistance = 5f;
    public LayerMask interactableLayer;

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    void Update()
    {
        if (playerCamera == null)
            return;

        // Ray starts from the exact center of the camera
        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        RaycastHit hit;

        if (Physics.Raycast(
            ray,
            out hit,
            interactionDistance,
            interactableLayer))
        {
            if (Mouse.current != null &&
                Mouse.current.leftButton.wasPressedThisFrame)
            {
                hit.collider.gameObject.SendMessage(
                    "Interact",
                    SendMessageOptions.DontRequireReceiver
                );
            }
        }
    }
}