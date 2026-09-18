using UnityEngine;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class ChecklistItem
{
    public string itemName;
    public TMP_Text uiText;
    public GameObject checkmark;
    public bool isCollected;
}

public class ChecklistManager : MonoBehaviour
{
    public List<ChecklistItem> items;

    [Header("Objective Counter")]
    public TMP_Text objectiveCount;

    void Start()
    {
        UpdateObjectiveCount();
    }

    public void CollectItem(string objectName)
    {
        foreach (ChecklistItem item in items)
        {
            if (item.itemName == objectName && !item.isCollected)
            {
                item.isCollected = true;

                // Make the text green
                if (item.uiText != null)
                {
                    item.uiText.color = Color.green;
                }

                // Show THIS item's checkmark
                if (item.checkmark != null)
                {
                    item.checkmark.SetActive(true);
                }

                // Update 0 / 12 counter
                UpdateObjectiveCount();

                Debug.Log(item.itemName + " collected!");
                return;
            }
        }

        Debug.LogWarning("Checklist item not found: " + objectName);
    }

    void UpdateObjectiveCount()
    {
        int collectedCount = 0;

        foreach (ChecklistItem item in items)
        {
            if (item.isCollected)
            {
                collectedCount++;
            }
        }

        if (objectiveCount != null)
        {
            objectiveCount.text = collectedCount + " / " + items.Count;
        }
    }
}