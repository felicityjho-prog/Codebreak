using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AutoDoorTrigger : MonoBehaviour
{
    public AutoDoor doorController;

    private readonly HashSet<Collider> playerColliders = new HashSet<Collider>();
    private bool reportedOccupied;

    private void OnTriggerEnter(Collider other)
    {
        TrackPlayerCollider(other);
    }

    private void OnTriggerStay(Collider other)
    {
        // Also catches a player already overlapping when this object becomes active.
        TrackPlayerCollider(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (BelongsToPlayer(other) && playerColliders.Remove(other))
            ReportOccupancy();
    }

    private void FixedUpdate()
    {
        int removed = playerColliders.RemoveWhere(
            collider => collider == null || !collider.enabled || !collider.gameObject.activeInHierarchy
        );

        if (removed > 0)
            ReportOccupancy();
    }

    private void OnDisable()
    {
        playerColliders.Clear();
        if (reportedOccupied)
        {
            reportedOccupied = false;
            if (doorController != null)
                doorController.SetPlayerOccupied(false);
        }
    }

    private void TrackPlayerCollider(Collider other)
    {
        if (BelongsToPlayer(other) && playerColliders.Add(other))
            ReportOccupancy();
    }

    private static bool BelongsToPlayer(Collider other)
    {
        Transform current = other.transform;
        while (current != null)
        {
            if (current.CompareTag("Player"))
                return true;

            current = current.parent;
        }

        return false;
    }

    private void ReportOccupancy()
    {
        bool occupied = playerColliders.Count > 0;
        if (reportedOccupied == occupied)
            return;

        reportedOccupied = occupied;
        if (doorController != null)
            doorController.SetPlayerOccupied(occupied);
    }
}