
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

    private List<Color> originalTextColors = new List<Color>();

    private void Awake()
    {
        CacheOriginalTextColors();
    }

    private void Start()
    {
        UpdateObjectiveCount();
    }

    // ==========================================
    // SAVE ORIGINAL TEXT COLORS
    // ==========================================

    private void CacheOriginalTextColors()
    {
        originalTextColors.Clear();

        if (items == null)
            return;

        foreach (ChecklistItem item in items)
        {
            if (item != null && item.uiText != null)
            {
                originalTextColors.Add(item.uiText.color);
            }
            else
            {
                originalTextColors.Add(Color.white);
            }
        }
    }

    // ==========================================
    // COLLECT CHECKLIST ITEM
    // ==========================================

    public void CollectItem(string objectName)
    {
        if (items == null)
            return;

        foreach (ChecklistItem item in items)
        {
            if (item != null &&
                item.itemName == objectName &&
                !item.isCollected)
            {
                item.isCollected = true;

                // Make the collected item text green.
                if (item.uiText != null)
                {
                    item.uiText.color = Color.green;
                }

                // Show this item's checkmark.
                if (item.checkmark != null)
                {
                    item.checkmark.SetActive(true);
                }

                UpdateObjectiveCount();

                Debug.Log(item.itemName + " collected!");
                return;
            }
        }

        Debug.LogWarning("Checklist item not found: " + objectName);
    }

    // ==========================================
    // RESET CHECKLIST FOR ROOM 1 RESTART
    // ==========================================

    public void ResetChecklist()
    {
        if (items == null)
            return;

        // Re-cache colors if the list changed.
        if (originalTextColors.Count != items.Count)
        {
            CacheOriginalTextColors();
        }

        for (int i = 0; i < items.Count; i++)
        {
            ChecklistItem item = items[i];

            if (item == null)
                continue;

            // Reset collected state.
            item.isCollected = false;

            // Restore original objective text color.
            if (item.uiText != null)
            {
                if (i < originalTextColors.Count)
                {
                    item.uiText.color = originalTextColors[i];
                }
                else
                {
                    item.uiText.color = Color.white;
                }
            }

            // Hide the checkmark.
            if (item.checkmark != null)
            {
                item.checkmark.SetActive(false);
            }
        }

        // Return objective counter to 0 / total.
        UpdateObjectiveCount();

        Debug.Log("ROOM 1 CHECKLIST RESET!");
    }

    // ==========================================
    // UPDATE OBJECTIVE COUNTER
    // ==========================================

    private void UpdateObjectiveCount()
    {
        if (objectiveCount == null)
            return;

        int collectedCount = 0;

        if (items != null)
        {
            foreach (ChecklistItem item in items)
            {
                if (item != null && item.isCollected)
                {
                    collectedCount++;
                }
            }

            objectiveCount.text =
                collectedCount + " / " + items.Count;
        }
        else
        {
            objectiveCount.text = "0 / 0";
        }
    }
}
