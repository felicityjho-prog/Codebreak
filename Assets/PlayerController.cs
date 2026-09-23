using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform cameraTransform;

    float xRotation = 0f;
    bool cursorLocked = true;
    bool lookEnabled = true;

    void Start()
    {
        LockCursor();
    }

    void Update()
    {
        // Toggle cursor with F
        if (Input.GetKeyDown(KeyCode.F))
        {
            UnlockCursor();
        }

        if (Input.GetMouseButtonDown(0) && cursorLocked == false && lookEnabled)
        {
            LockCursor();
        }

        // Camera movement
        if (cursorLocked && lookEnabled)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            cameraTransform.localRotation =
                Quaternion.Euler(xRotation, 0f, 0f);

            transform.Rotate(Vector3.up * mouseX);
        }
    }

    // ==============================
    // DISABLE CAMERA LOOK
    // ==============================

    public void DisableLook()
    {
        lookEnabled = false;
    }

    // ==============================
    // ENABLE CAMERA LOOK
    // ==============================

    public void EnableLook()
    {
        lookEnabled = true;
        LockCursor();
    }

    // ==============================
    // CURSOR
    // ==============================

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cursorLocked = true;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cursorLocked = false;
    }
}