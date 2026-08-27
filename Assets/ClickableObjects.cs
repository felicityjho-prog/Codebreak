using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    public string itemName;
    public ChecklistManager checklistManager;

    void OnMouseDown()
    {
        // kapag na-click ang object
        checklistManager.CollectItem(itemName);

        // optional: mawala ang object after click
        gameObject.SetActive(false);
    }
}