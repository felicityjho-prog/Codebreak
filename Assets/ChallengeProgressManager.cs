using UnityEngine;

public class ChallengeProgressManager : MonoBehaviour
{
    public static ChallengeProgressManager Instance;

    [Header("Challenge Progress")]
    [Tooltip("1 = Table 1, 2 = Table 2, 3 = Table 3")]
    public int currentUnlockedTable = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // ==========================================
    // CHECK IF TABLE IS UNLOCKED
    // ==========================================

    public bool IsTableUnlocked(int tableNumber)
    {
        return tableNumber <= currentUnlockedTable;
    }

    // ==========================================
    // CHECK IF TABLE IS COMPLETED
    // ==========================================

    public bool IsTableCompleted(int tableNumber)
    {
        return tableNumber < currentUnlockedTable;
    }

    // ==========================================
    // COMPLETE TABLE
    // ==========================================

    public void CompleteTable(int tableNumber)
    {
        Debug.Log("TABLE " + tableNumber + " COMPLETED!");

        if (tableNumber == currentUnlockedTable)
        {
            currentUnlockedTable++;

            Debug.Log(
                "TABLE " + currentUnlockedTable + " UNLOCKED!"
            );
        }
    }

    // ==========================================
    // GET CURRENT UNLOCKED TABLE
    // ==========================================

    public int GetCurrentUnlockedTable()
    {
        return currentUnlockedTable;
    }
}