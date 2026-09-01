using UnityEngine;
using Ostryzhnyi.AdaptiveGrid.CodeBase.Field.Grid;
using Ostryzhnyi.AdaptiveGrid.CodeBase.Field.Grid.Data;
using Ostryzhnyi.DI;

namespace Ostryzhnyi.AdaptiveGrid.CodeBase.Sample
{
    public class EntrySamplePoint : MonoBehaviour
    {
        [Inject] private FieldGrid _fieldGrid;
        [Inject] private GridSettings _gridSettings;

        private void Start()
        {
            Debug.Log("ENTRY SAMPLE POINT IS RUNNING!");

            int[,] grid = new int[_gridSettings.GridSize, _gridSettings.GridSize];

            for (int i = 0; i < _gridSettings.GridSize; i++)
            {
                for (int j = 0; j < _gridSettings.GridSize; j++)
                {
                    grid[i, j] = 1;
                }
            }

            Debug.Log("GRID CREATED: " + _gridSettings.GridSize + "x" + _gridSettings.GridSize);

            _fieldGrid.SpawnGrid(grid);

            Debug.Log("SPAWNGRID CALLED!");
        }
    }
}