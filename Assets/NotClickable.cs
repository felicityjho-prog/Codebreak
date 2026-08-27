using UnityEngine;

public class NotCollectible : MonoBehaviour
{
    public InteractionMessage messageManager;

    void OnMouseDown()
    {
        messageManager.ShowMessage();
    }
}