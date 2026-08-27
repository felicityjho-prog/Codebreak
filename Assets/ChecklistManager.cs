using UnityEngine;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class ChecklistItem
{
    public string itemName;
    public TMP_Text uiText;
    public bool isCollected;
}

public class ChecklistManager : MonoBehaviour
{
    public List<ChecklistItem> items;

    public void CollectItem(string objectName)
    {
        foreach (ChecklistItem item in items)
        {
            if (item.itemName == objectName && !item.isCollected)
            {
                item.isCollected = true;

                item.uiText.color = Color.green;
                item.uiText.text = "✔ " + item.itemName;

                Debug.Log(item.itemName + " collected!");
            }
        }
    }
}