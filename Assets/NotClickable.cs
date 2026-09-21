using UnityEngine;

public class NotClickable : MonoBehaviour
{
    public InteractionMessage interactionMessage;

    [Header("Room Settings")]
    public bool isRoom1Object = true;

    private bool interactionEnabled = false;

    public void EnableInteraction()
    {
        if (isRoom1Object)
        {
            interactionEnabled = true;
        }
    }

    public void DisableInteraction()
    {
        interactionEnabled = false;
    }

    private void OnMouseDown()
    {
        if (!interactionEnabled)
            return;

        if (!isRoom1Object)
            return;

        if (GetComponent<ClickableObject>() != null)
            return;

        if (interactionMessage != null)
        {
            interactionMessage.ShowMessage();
        }
    }
}