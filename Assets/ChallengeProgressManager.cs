using UnityEngine;

public class ChallengeProgressManager : MonoBehaviour
{
    public static ChallengeProgressManager Instance;

    [Header("Challenge Progress")]
    [Tooltip("1 = Table 1, 2 = Table 2, 3 = Table 3")]
    public int currentUnlockedTable = 1;

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
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
        Debug.Log(
            "TABLE " +
            tableNumber +
            " COMPLETED!"
        );

        if (tableNumber == currentUnlockedTable)
        {
            currentUnlockedTable++;

            Debug.Log(
                "TABLE " +
                currentUnlockedTable +
                " UNLOCKED!"
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

    // ==========================================
    // RESET ROOM 2 PROGRESS
    // ==========================================
    //
    // IMPORTANT:
    // This does NOT reset Room 1.
    //
    // It only makes Room 2 start again
    // from Table 1.
    //
    // Table 1 = unlocked
    // Table 2 = locked
    // Table 3 = locked
    // ==========================================

    public void ResetRoom2Progress()
    {
        currentUnlockedTable = 1;

        Debug.Log(
            "================================="
        );

        Debug.Log(
            "ROOM 2 PROGRESS RESET"
        );

        Debug.Log(
            "TABLE 1 UNLOCKED"
        );

        Debug.Log(
            "TABLE 2 LOCKED"
        );

        Debug.Log(
            "TABLE 3 LOCKED"
        );

        Debug.Log(
            "================================="
        );
    }
}