
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public sealed class PlayerBodyCollisionSolver : MonoBehaviour
{
    // Room 1 wall object names verified in the project.
    private static readonly string[] Room1WallPaths =
    {
        "wall",
        "wall (1)",
        "wall (3)",
        "wall (4)",
        "wall (5)",
        "wall (6)",
        "wall (7)"
    };

    // Room 2 wall object names verified in the project.
    private static readonly string[] Room2WallPaths =
    {
        "Wall_1",
        "Wall_2",
        "Wall_3",
        "Wall_4"
    };

    [Header("Collision Settings")]
    [SerializeField, Min(0f)]
    private float collisionSkin = 0.03f;

    [Header("Optional Legacy Reference")]
    [Tooltip("Kept for compatibility with the existing Inspector assignment.")]
    [SerializeField]
    private Animator bodyAnimator;

    private const int MaxSlideIterations = 4;
    private const int MaxHits = 256;

    private const float MinDisplacementSqrMagnitude = 0.000001f;
    private const float MinHorizontalNormalSqrMagnitude = 0.000001f;
    private const float MinIntoSurfaceDot = -0.0001f;

    private readonly HashSet<Collider> blockingColliders =
        new HashSet<Collider>();

    private readonly RaycastHit[] hitBuffer = new RaycastHit[MaxHits];

    private CharacterController characterController;
    private int blockingLayerMask;
    private bool hasValidBlockingColliders;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        CacheTargetBlockingColliders();
    }

    /// <summary>
    /// Constrains horizontal movement against the cached colliders
    /// belonging only to the requested Room 1 walls, Room 2 walls,
    /// and Room 1 Shelf_Apt_01.
    ///
    /// Vertical movement and gravity remain handled by PlayerMovement
    /// and CharacterController.
    /// </summary>
    public Vector3 ConstrainHorizontalDisplacement(
        Vector3 proposedDisplacement)
    {
        proposedDisplacement.y = 0f;

        if (proposedDisplacement.sqrMagnitude <
            MinDisplacementSqrMagnitude)
        {
            return Vector3.zero;
        }

        if (characterController == null ||
            !characterController.enabled ||
            !hasValidBlockingColliders)
        {
            return proposedDisplacement;
        }

        Vector3 capsulePoint1;
        Vector3 capsulePoint2;
        float capsuleRadius;

        if (!TryBuildControllerCapsule(
                out capsulePoint1,
                out capsulePoint2,
                out capsuleRadius))
        {
            return proposedDisplacement;
        }

        Vector3 totalApplied = Vector3.zero;
        Vector3 remaining = proposedDisplacement;

        for (int iteration = 0;
             iteration < MaxSlideIterations;
             iteration++)
        {
            if (remaining.sqrMagnitude <
                MinDisplacementSqrMagnitude)
            {
                break;
            }

            Vector3 direction = remaining.normalized;
            float distance = remaining.magnitude;

            // Move the virtual capsule by the displacement already
            // accepted in the previous slide iterations.
            Vector3 castPoint1 = capsulePoint1 + totalApplied;
            Vector3 castPoint2 = capsulePoint2 + totalApplied;

            RaycastHit nearestHit;

            bool foundHit = TryFindNearestBlockingHit(
                castPoint1,
                castPoint2,
                capsuleRadius,
                direction,
                distance,
                out nearestHit);

            if (!foundHit)
            {
                totalApplied += remaining;
                remaining = Vector3.zero;
                break;
            }

            // Stop just before the closest blocking collider.
            float safeDistance = Mathf.Max(
                0f,
                nearestHit.distance - collisionSkin);

            Vector3 advance = direction * safeDistance;

            totalApplied += advance;
            remaining -= advance;

            // Only horizontal surface normals should constrain
            // horizontal movement. Ignore floor/ceiling-like normals.
            Vector3 horizontalNormal = nearestHit.normal;
            horizontalNormal.y = 0f;

            float normalSqrMagnitude =
                horizontalNormal.sqrMagnitude;

            if (normalSqrMagnitude <
                MinHorizontalNormalSqrMagnitude)
            {
                remaining = Vector3.zero;
                break;
            }

            horizontalNormal /= Mathf.Sqrt(normalSqrMagnitude);

            // Remove only the movement component going INTO the surface.
            // Keep tangential movement so the player can slide along walls.
            float movementIntoSurface =
                Vector3.Dot(remaining, horizontalNormal);

            if (movementIntoSurface < 0f)
            {
                remaining -=
                    horizontalNormal * movementIntoSurface;
            }
        }

        totalApplied.y = 0f;
        return totalApplied;
    }

    private bool TryBuildControllerCapsule(
        out Vector3 point1,
        out Vector3 point2,
        out float radius)
    {
        point1 = Vector3.zero;
        point2 = Vector3.zero;
        radius = 0f;

        if (characterController == null)
        {
            return false;
        }

        Vector3 scale = transform.lossyScale;

        float scaleX = Mathf.Abs(scale.x);
        float scaleY = Mathf.Abs(scale.y);
        float scaleZ = Mathf.Abs(scale.z);

        radius = characterController.radius *
                 Mathf.Max(scaleX, scaleZ);

        float height = characterController.height * scaleY;

        if (radius <= 0f || height <= 0f)
        {
            return false;
        }

        height = Mathf.Max(height, radius * 2f);

        Vector3 worldCenter =
            transform.TransformPoint(characterController.center);

        float segmentHalfLength =
            Mathf.Max(0f, (height * 0.5f) - radius);

        point1 = worldCenter +
                 Vector3.up * segmentHalfLength;

        point2 = worldCenter -
                 Vector3.up * segmentHalfLength;

        return true;
    }

    private bool TryFindNearestBlockingHit(
        Vector3 point1,
        Vector3 point2,
        float radius,
        Vector3 direction,
        float maxDistance,
        out RaycastHit nearestHit)
    {
        nearestHit = new RaycastHit();

        if (blockingLayerMask == 0 ||
            blockingColliders.Count == 0)
        {
            return false;
        }

        int hitCount = Physics.CapsuleCastNonAlloc(
            point1,
            point2,
            radius,
            direction,
            hitBuffer,
            maxDistance,
            blockingLayerMask,
            QueryTriggerInteraction.Ignore);

        bool foundBlockingHit = false;
        float nearestDistance = float.PositiveInfinity;

        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit hit = hitBuffer[i];
            Collider hitCollider = hit.collider;

            if (hitCollider == null)
            {
                continue;
            }

            // Only explicitly cached target colliders are accepted.
            // Unrelated colliders on the same physics layer are ignored.
            if (!blockingColliders.Contains(hitCollider))
            {
                continue;
            }

            if (!hitCollider.enabled ||
                !hitCollider.gameObject.activeInHierarchy ||
                hitCollider.isTrigger)
            {
                continue;
            }

            if (hit.distance < 0f ||
                hit.distance > maxDistance)
            {
                continue;
            }

            Vector3 horizontalNormal = hit.normal;
            horizontalNormal.y = 0f;

            if (horizontalNormal.sqrMagnitude <
                MinHorizontalNormalSqrMagnitude)
            {
                continue;
            }

            horizontalNormal.Normalize();

            // Ignore surfaces the player is not moving into.
            if (Vector3.Dot(direction, horizontalNormal) >=
                MinIntoSurfaceDot)
            {
                continue;
            }

            if (hit.distance < nearestDistance)
            {
                nearestDistance = hit.distance;
                nearestHit = hit;
                foundBlockingHit = true;
            }
        }

        return foundBlockingHit;
    }

    private void CacheTargetBlockingColliders()
    {
        blockingColliders.Clear();
        blockingLayerMask = 0;
        hasValidBlockingColliders = false;

        Scene activeScene = SceneManager.GetActiveScene();

        if (!activeScene.IsValid() || !activeScene.isLoaded)
        {
            Debug.LogWarning(
                "[PlayerBodyCollisionSolver] Active scene is not valid or loaded.",
                this);
            return;
        }

        // Search the complete active-scene hierarchy, not only
        // top-level GameObjects.
        Transform room1Root = FindSceneRoot(activeScene, "room1");
        Transform room2Root = FindSceneRoot(activeScene, "room2");

        int foundRoom1WallCount = 0;
        int foundRoom2WallCount = 0;
        bool foundShelf = false;

        if (room1Root != null)
        {
            for (int i = 0; i < Room1WallPaths.Length; i++)
            {
                Transform wallRoot =
                    room1Root.Find(Room1WallPaths[i]);

                if (wallRoot != null)
                {
                    AddBlockingColliderRoot(wallRoot);
                    foundRoom1WallCount++;
                }
            }

            Transform shelfRoot =
                room1Root.Find("Shelf_Set/Shelf_Apt_01");

            if (shelfRoot != null)
            {
                AddBlockingColliderRoot(shelfRoot);
                foundShelf = true;
            }
        }

        if (room2Root != null)
        {
            for (int i = 0; i < Room2WallPaths.Length; i++)
            {
                Transform wallRoot =
                    room2Root.Find(Room2WallPaths[i]);

                if (wallRoot != null)
                {
                    AddBlockingColliderRoot(wallRoot);
                    foundRoom2WallCount++;
                }
            }
        }

        hasValidBlockingColliders =
            blockingColliders.Count > 0 &&
            blockingLayerMask != 0;

        if (room1Root == null ||
            foundRoom1WallCount != Room1WallPaths.Length)
        {
            Debug.LogWarning(
                "[PlayerBodyCollisionSolver] Could not find all expected " +
                "Room 1 wall roots. Check their names and hierarchy paths.",
                this);
        }

        if (!foundShelf)
        {
            Debug.LogWarning(
                "[PlayerBodyCollisionSolver] Could not find the exact " +
                "Room 1 Shelf_Set/Shelf_Apt_01 target.",
                this);
        }

        if (room2Root == null ||
            foundRoom2WallCount != Room2WallPaths.Length)
        {
            Debug.LogWarning(
                "[PlayerBodyCollisionSolver] Could not find all expected " +
                "Room 2 wall roots. Check their names and hierarchy paths.",
                this);
        }

        if (!hasValidBlockingColliders)
        {
            Debug.LogWarning(
                "[PlayerBodyCollisionSolver] No valid target colliders were " +
                "cached for the requested walls and Shelf_Apt_01.",
                this);
        }
        else
        {
            Debug.Log(
                "[PlayerBodyCollisionSolver] Cached " +
                blockingColliders.Count +
                " collider(s) for the requested Room 1/Room 2 walls " +
                "and Shelf_Apt_01.",
                this);
        }
    }

    private Transform FindSceneRoot(Scene scene, string objectName)
    {
        if (!scene.IsValid() || !scene.isLoaded)
        {
            return null;
        }

        GameObject[] sceneRoots = scene.GetRootGameObjects();

        for (int i = 0; i < sceneRoots.Length; i++)
        {
            GameObject sceneRoot = sceneRoots[i];

            if (sceneRoot == null)
            {
                continue;
            }

            // Include inactive children in case a room is inactive
            // at the time the player initializes.
            Transform[] descendants =
                sceneRoot.GetComponentsInChildren<Transform>(true);

            for (int j = 0; j < descendants.Length; j++)
            {
                Transform candidate = descendants[j];

                if (candidate == null)
                {
                    continue;
                }

                if (string.Equals(
                    candidate.name,
                    objectName,
                    System.StringComparison.OrdinalIgnoreCase))
                {
                    return candidate;
                }
            }
        }

        return null;
    }

    private void AddBlockingColliderRoot(Transform blockerRoot)
    {
        if (blockerRoot == null)
        {
            return;
        }

        Collider[] colliders =
            blockerRoot.GetComponentsInChildren<Collider>(true);

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider currentCollider = colliders[i];

            if (currentCollider == null)
            {
                continue;
            }

            if (blockingColliders.Add(currentCollider))
            {
                // Include only layers used by the selected target colliders.
                // HashSet filtering prevents unrelated same-layer objects
                // from being treated as blockers.
                blockingLayerMask |=
                    1 << currentCollider.gameObject.layer;
            }
        }
    }
}
