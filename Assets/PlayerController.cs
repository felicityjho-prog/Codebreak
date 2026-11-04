
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("Camera Settings")]
    public float mouseSensitivity = 100f;
    public Transform cameraTransform;

    private float baseMouseSensitivity;
    private float xRotation = 0f;
    private bool cursorLocked = true;
    private bool lookEnabled = true;

    public bool IsLookEnabled => lookEnabled;

    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        baseMouseSensitivity = mouseSensitivity;
    }

    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        float sensitivityMultiplier =
            PlayerPrefs.GetFloat(
                "MouseSensitivityMultiplier",
                1f
            );

        SetSensitivityMultiplier(sensitivityMultiplier);

        LockCursor();

        if (cameraTransform != null)
        {
            xRotation = cameraTransform.localEulerAngles.x;

            if (xRotation > 180f)
                xRotation -= 360f;
        }
        else
        {
            Debug.LogWarning(
                "PlayerController: Camera Transform is NOT assigned!"
            );
        }

        Debug.Log(
            "PlayerController started. " +
            "Look Enabled: " + lookEnabled +
            " | Camera Assigned: " + (cameraTransform != null) +
            " | Mouse Sensitivity: " + mouseSensitivity
        );
    }

    // =====================================================
    // APPLY SENSITIVITY MULTIPLIER
    // =====================================================

    public void SetSensitivityMultiplier(float multiplier)
    {
        multiplier = Mathf.Clamp(multiplier, 0.5f, 3f);

        mouseSensitivity = baseMouseSensitivity * multiplier;

        Debug.Log(
            "Mouse sensitivity updated. Multiplier: " +
            multiplier +
            " | Effective Sensitivity: " +
            mouseSensitivity
        );
    }

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        // Press F to show the cursor.
        if (Input.GetKeyDown(KeyCode.F))
        {
            UnlockCursor();
        }

        // Lock cursor when clicking outside UI.
        if (Input.GetMouseButtonDown(0) &&
            !cursorLocked &&
            lookEnabled)
        {
            bool pointerOverUI = false;

            if (EventSystem.current != null)
            {
                pointerOverUI =
                    EventSystem.current.IsPointerOverGameObject();
            }

            if (!pointerOverUI)
            {
                LockCursor();
            }
        }

        // Camera movement.
        if (cursorLocked &&
            lookEnabled &&
            cameraTransform != null)
        {
            float mouseX =
                Input.GetAxis("Mouse X") *
                mouseSensitivity *
                Time.deltaTime;

            float mouseY =
                Input.GetAxis("Mouse Y") *
                mouseSensitivity *
                Time.deltaTime;

            xRotation -= mouseY;

            xRotation = Mathf.Clamp(
                xRotation,
                -90f,
                90f
            );

            cameraTransform.localRotation =
                Quaternion.Euler(
                    xRotation,
                    0f,
                    0f
                );

            transform.Rotate(
                Vector3.up * mouseX
            );
        }
    }

    // =====================================================
    // LATE UPDATE
    // KEEP CURSOR HIDDEN DURING NORMAL GAMEPLAY
    // =====================================================

    private void LateUpdate()
    {
        // Only enforce the hidden cursor when:
        // 1. Camera look is enabled.
        // 2. The player has not unlocked the cursor using F.
        //
        // UI panels can still show the cursor while look is disabled.

        if (lookEnabled && cursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // =====================================================
    // DISABLE CAMERA LOOK
    // =====================================================

    public void DisableLook()
    {
        lookEnabled = false;

        Debug.Log(
            "CAMERA LOOK DISABLED. " +
            "Called on: " + gameObject.name
        );
    }

    // =====================================================
    // ENABLE CAMERA LOOK
    // =====================================================

    public void EnableLook()
    {
        lookEnabled = true;

        LockCursor();

        Debug.Log(
            "CAMERA LOOK ENABLED. " +
            "LookEnabled: " + lookEnabled +
            " | Camera Assigned: " + (cameraTransform != null) +
            " | Cursor Locked: " + cursorLocked +
            " | Cursor State: " + Cursor.lockState
        );
    }

    // =====================================================
    // SET CAMERA ROTATION
    // =====================================================

    public void SetCameraRotation(Quaternion rotation)
    {
        if (cameraTransform == null)
        {
            Debug.LogWarning(
                "SetCameraRotation failed: Camera Transform is missing."
            );

            return;
        }

        cameraTransform.localRotation = rotation;

        xRotation = rotation.eulerAngles.x;

        if (xRotation > 180f)
            xRotation -= 360f;
    }

    // =====================================================
    // LOCK CURSOR
    // =====================================================

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cursorLocked = true;
    }

    // =====================================================
    // UNLOCK CURSOR
    // =====================================================

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cursorLocked = false;
    }
}
