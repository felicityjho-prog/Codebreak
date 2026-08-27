using UnityEngine;

public class AutoDoor : MonoBehaviour
{
    public Transform doorPivot;
    public float openAngle = -90f;
    public float speed = 2f;
    public bool openRight = true;

    public Collider doorCollider;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    private bool isOpen = false;

    void Start()
    {
        closedRotation = doorPivot.localRotation;

        float direction = openRight ? 1f : -1f;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle * direction, 0);
    }

    void Update()
    {
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isOpen = true;

            // 🔥 IMPORTANT: disable agad
            if (doorCollider != null)
                doorCollider.enabled = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isOpen = false;

            // ibalik collider pag nakaalis na
            if (doorCollider != null)
                doorCollider.enabled = true;
        }
    }
}