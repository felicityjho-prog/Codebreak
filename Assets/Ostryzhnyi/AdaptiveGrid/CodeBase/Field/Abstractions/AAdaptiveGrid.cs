using Ostryzhnyi.DI;
using UnityEngine;

namespace Ostryzhnyi.AdaptiveGrid.CodeBase.Field.Abstractions
{
    public abstract class AAdaptiveGrid : MonoBehaviour
    {
        [Inject] protected DIContainer _container;

        [SerializeField] private GridItem _squarePrefab;
        [SerializeField] protected int _gridSize = 12;

        // CHANGED: protected para magamit ng FieldGrid
        [SerializeField] protected Transform _field;

        [SerializeField, Range(0, 100)] private float _paddingTopPercent = 0f;
        [SerializeField, Range(0, 100)] private float _paddingBottomPercent = 0f;
        [SerializeField, Range(0, 100)] private float _paddingLeftPercent = 0f;
        [SerializeField, Range(0, 100)] private float _paddingRightPercent = 0f;

        public GridItem[,] GridItems { get; private set; }
        public int[,] Grid { get; private set; }

        protected abstract float Spacing { get; }

        private Camera _mainCamera;
        private float _screenWidth;
        private float _screenHeight;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        protected void SetCustomGrid(
            int[,] customGrid,
            float? defaultSquareSize = null,
            float? defaultRelativeSpacing = null)
        {
            Grid = customGrid;

            if (customGrid != null)
            {
                _gridSize = customGrid.GetLength(0);
            }

            CreateGrid(
                customGrid,
                defaultSquareSize,
                defaultRelativeSpacing
            );
        }

        private void CreateGrid(
            int[,] grid,
            float? defaultSquareSize = null,
            float? defaultRelativeSpacing = null)
        {
            if (_field == null)
            {
                Debug.LogError("FieldGrid ERROR: Field Transform is not assigned!");
                return;
            }

            if (_squarePrefab == null)
            {
                Debug.LogError("FieldGrid ERROR: Square Prefab is not assigned!");
                return;
            }

            if (grid == null)
            {
                Debug.LogError("FieldGrid ERROR: Grid is null!");
                return;
            }

            // Delete previous generated squares
            foreach (Transform child in _field)
            {
                Destroy(child.gameObject);
            }

            CalculateSizes(
                out var squareSize,
                out var relativeSpacing,
                out var gridWorldWidth,
                out var gridWorldHeight,
                out var screenBounds
            );

            if (defaultSquareSize != null)
                squareSize = defaultSquareSize.Value;

            if (defaultRelativeSpacing != null)
                relativeSpacing = defaultRelativeSpacing.Value;

            int minRow = grid.GetLength(0);
            int maxRow = -1;
            int minCol = grid.GetLength(1);
            int maxCol = -1;

            // Find occupied cells
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] == 1)
                    {
                        minRow = Mathf.Min(minRow, i);
                        maxRow = Mathf.Max(maxRow, i);
                        minCol = Mathf.Min(minCol, j);
                        maxCol = Mathf.Max(maxCol, j);
                    }
                }
            }

            if (maxRow == -1 || maxCol == -1)
            {
                Debug.LogWarning("No blocks to display.");
                return;
            }

            int rowsToRender = maxRow - minRow + 1;
            int colsToRender = maxCol - minCol + 1;

            float xOffset =
                (colsToRender % 2 == 0)
                    ? 0f
                    : (squareSize + relativeSpacing) / 2f;

            float yOffset =
                (rowsToRender % 2 == 0)
                    ? 0f
                    : (squareSize + relativeSpacing) / 2f;

            xOffset = (colsToRender % 2 == 1) ? 0 : xOffset;
            yOffset = (rowsToRender % 2 == 1) ? 0 : yOffset;

            GridItems = new GridItem[
                grid.GetLength(0),
                grid.GetLength(1)
            ];

            // Spawn squares
            for (int i = minRow; i <= maxRow; i++)
            {
                for (int j = minCol; j <= maxCol; j++)
                {
                    if (grid[i, j] == 1)
                    {
                        GridItem square = Instantiate(
                            _squarePrefab,
                            _field
                        );

                        _container.InjectGameObject(square.gameObject);

                        float xPos =
                            (j - (minCol + maxCol) / 2f)
                            * (squareSize + relativeSpacing)
                            + xOffset;

                        float yPos =
                            ((minRow + maxRow) / 2f - i)
                            * (squareSize + relativeSpacing)
                            + yOffset;

                        // IMPORTANT:
                        // Position relative to FIELD
                        square.transform.localPosition =
                            new Vector3(
                                xPos,
                                yPos,
                                0f
                            );

                        square.transform.localScale =
                            Vector3.one * squareSize;

                        square.name =
                            $"{square.name} [{i},{j}]";

                        GridItems[i, j] = square;
                    }
                }
            }

            SpawnBackGround(
                squareSize,
                relativeSpacing
            );

            SpawnedGrid();
        }

        protected virtual void CalculateSizes(
            out float squareSize,
            out float relativeSpacing,
            out float gridWorldWidth,
            out float gridWorldHeight,
            out Vector3 screenBounds)
        {
            if (_mainCamera == null)
                _mainCamera = Camera.main;

            screenBounds =
                _mainCamera.ScreenToWorldPoint(
                    new Vector3(
                        Screen.width,
                        Screen.height,
                        _mainCamera.transform.position.z
                    )
                );

            gridWorldWidth = screenBounds.x * 2;
            gridWorldHeight = screenBounds.y * 2;

            float paddingTop =
                gridWorldHeight *
                (_paddingTopPercent / 100f);

            float paddingBottom =
                gridWorldHeight *
                (_paddingBottomPercent / 100f);

            float paddingLeft =
                gridWorldWidth *
                (_paddingLeftPercent / 100f);

            float paddingRight =
                gridWorldWidth *
                (_paddingRightPercent / 100f);

            float availableWidth =
                gridWorldWidth -
                paddingLeft -
                paddingRight;

            float availableHeight =
                gridWorldHeight -
                paddingTop -
                paddingBottom;

            relativeSpacing =
                (_gridSize > 1)
                    ? Mathf.Min(
                        availableWidth,
                        availableHeight
                    ) * Spacing /
                    (_gridSize - 1)
                    : 0;

            squareSize =
                Mathf.Min(
                    (
                        availableWidth -
                        (_gridSize - 1) *
                        relativeSpacing
                    ) / _gridSize,

                    (
                        availableHeight -
                        (_gridSize - 1) *
                        relativeSpacing
                    ) / _gridSize
                );
        }

        protected virtual void SpawnBackGround(
            float squareSize,
            float relativeSpacing)
        {
        }

        protected virtual void SpawnedGrid()
        {
        }
    }
}