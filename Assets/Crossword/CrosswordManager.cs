using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrosswordManager : MonoBehaviour
{
    [Header("Grid")]
    public GameObject crosswordCellPrefab;
    public int gridSize = 6;

    [Header("Colors")]
    public Color whiteCellColor = Color.white;
    public Color blockedCellColor = Color.black;

    // 0 = playable
    // 1 = blocked
    private int[,] crosswordPattern =
    {
        { 0, 0, 0, 1, 0, 0 },
        { 0, 1, 0, 0, 0, 1 },
        { 0, 0, 0, 0, 1, 0 },
        { 1, 0, 1, 0, 0, 0 },
        { 0, 0, 0, 0, 1, 0 },
        { 0, 1, 0, 1, 0, 0 }
    };

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                GameObject cell = Instantiate(
                    crosswordCellPrefab,
                    transform
                );

                Image cellImage = cell.GetComponent<Image>();

                TMP_InputField input =
                    cell.GetComponentInChildren<TMP_InputField>();

                bool blocked = crosswordPattern[row, col] == 1;

                if (blocked)
                {
                    // BLACK CELL
                    if (cellImage != null)
                        cellImage.color = blockedCellColor;

                    if (input != null)
                        input.gameObject.SetActive(false);
                }
                else
                {
                    // PLAYABLE CELL
                    if (cellImage != null)
                        cellImage.color = whiteCellColor;

                    if (input != null)
                        input.gameObject.SetActive(true);
                }
            }
        }
    }
}