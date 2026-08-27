using UnityEngine;

public class Collectible : MonoBehaviour
{
    private bool collected = false;

    private void OnMouseDown()
    {
        if (!collected)
        {
            collected = true;

            FindObjectOfType<TaskManager>().CollectObject();

            gameObject.SetActive(false);
        }
    }
}