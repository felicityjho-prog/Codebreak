using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GlobalCameraCollision : MonoBehaviour
{
    [Header("Camera obstruction")]
    [SerializeField, Min(0.01f)] private float cameraRadius = 0.18f;
    [SerializeField, Min(0f)] private float wallMargin = 0.12f;
    [SerializeField, Min(0.01f)] private float returnSmoothTime = 0.16f;
    [SerializeField, Min(0.05f)] private float wallRescanInterval = 0.25f;

    private readonly HashSet<Collider> structuralWalls = new HashSet<Collider>();
    private Transform cameraParent;
    private Vector3 normalLocalPosition;
    private Vector3 returnVelocity;
    private float nextWallRescanTime;

    private void Awake()
    {
        cameraParent = transform.parent;
        normalLocalPosition = transform.localPosition;

        if (cameraParent == null)
        {
            enabled = false;
            return;
        }

        RefreshStructuralWalls();
        nextWallRescanTime = Time.unscaledTime + wallRescanInterval;
    }

    private void LateUpdate()
    {
        if (cameraParent == null)
            return;

        if (Time.unscaledTime >= nextWallRescanTime)
        {
            RefreshStructuralWalls();
            nextWallRescanTime = Time.unscaledTime + wallRescanInterval;
        }

        Vector3 desiredWorldPosition = cameraParent.TransformPoint(normalLocalPosition);
        Vector3 castOrigin = cameraParent.TransformPoint(
            new Vector3(normalLocalPosition.x, normalLocalPosition.y, 0f));
        Vector3 castVector = desiredWorldPosition - castOrigin;
        float castDistance = castVector.magnitude;

        if (castDistance <= Mathf.Epsilon)
            return;

        Vector3 castDirection = castVector / castDistance;
        RaycastHit[] hits = Physics.SphereCastAll(
            castOrigin,
            cameraRadius,
            castDirection,
            castDistance,
            Physics.AllLayers,
            QueryTriggerInteraction.Ignore);

        float nearestWallDistance = castDistance;
        bool wallBlocked = false;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == null || !structuralWalls.Contains(hit.collider))
                continue;

            if (hit.distance < nearestWallDistance)
            {
                nearestWallDistance = hit.distance;
                wallBlocked = true;
            }
        }

        if (wallBlocked)
        {
            float safeDistance = Mathf.Max(0f, nearestWallDistance - wallMargin);
            Vector3 safeWorldPosition = castOrigin + castDirection * safeDistance;
            transform.localPosition = cameraParent.InverseTransformPoint(safeWorldPosition);
            returnVelocity = Vector3.zero;
        }
        else
        {
            transform.localPosition = Vector3.SmoothDamp(
                transform.localPosition,
                normalLocalPosition,
                ref returnVelocity,
                returnSmoothTime);
        }
    }

    private void RefreshStructuralWalls()
    {
        structuralWalls.Clear();

        Collider[] colliders = FindObjectsByType<Collider>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        foreach (Collider candidate in colliders)
        {
            if (candidate == null ||
                !candidate.enabled ||
                candidate.isTrigger ||
                !IsInGameRoomHierarchy(candidate.transform) ||
                !HasStructuralWallName(candidate.gameObject.name))
            {
                continue;
            }

            structuralWalls.Add(candidate);
        }
    }

    private static bool IsInGameRoomHierarchy(Transform candidate)
    {
        Transform current = candidate;

        while (current != null)
        {
            if (current.parent == null &&
                (current.name.Equals("room1", System.StringComparison.OrdinalIgnoreCase) ||
                 current.name.Equals("room2", System.StringComparison.OrdinalIgnoreCase) ||
                 current.name.Equals("room3", System.StringComparison.OrdinalIgnoreCase) ||
                 current.name.Equals("hallway", System.StringComparison.OrdinalIgnoreCase) ||
                 current.name.Equals("corridor", System.StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }

    private static bool HasStructuralWallName(string objectName)
    {
        string name = objectName.Trim();

        if (name.Equals("wall", System.StringComparison.OrdinalIgnoreCase))
            return true;

        if (name.StartsWith("wall_", System.StringComparison.OrdinalIgnoreCase))
            return ContainsOnlyDigits(name.Substring(5));

        if (name.StartsWith("wall ", System.StringComparison.OrdinalIgnoreCase))
        {
            string suffix = name.Substring(5).Trim();

            if (suffix.Length >= 2 && suffix[0] == '(' && suffix[suffix.Length - 1] == ')')
                suffix = suffix.Substring(1, suffix.Length - 2).Trim();

            return ContainsOnlyDigits(suffix);
        }

        return false;
    }

    private static bool ContainsOnlyDigits(string value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        foreach (char character in value)
        {
            if (!char.IsDigit(character))
                return false;
        }

        return true;
    }
}