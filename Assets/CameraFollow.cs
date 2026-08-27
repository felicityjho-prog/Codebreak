using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Mixamo Character
    public Vector3 offset = new Vector3(0, 2.5f, -4f);
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        // Follow position
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.position = smoothedPosition;

        // Always look at character
        transform.LookAt(target);
    }
}