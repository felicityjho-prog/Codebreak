using UnityEngine;

public class InstructionPanelPositionLocker : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 lockedPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // Save the current position
        lockedPosition = rectTransform.anchoredPosition;
    }

    private void LateUpdate()
    {
        if (rectTransform == null)
            return;

        // Keep the panel locked to its original position
        rectTransform.anchoredPosition = lockedPosition;
    }
}