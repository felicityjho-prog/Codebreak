using System;
using Ostryzhnyi.AdaptiveGrid.CodeBase.Field.Abstractions;
using Ostryzhnyi.AdaptiveGrid.CodeBase.Field.Grid.Data;
using Ostryzhnyi.AdaptiveGrid.CodeBase.Field.Interfaces;
using Ostryzhnyi.DI;
using UnityEngine;

namespace Ostryzhnyi.AdaptiveGrid.CodeBase.Field.Grid
{
    public class FieldGrid : AAdaptiveGrid, ISizeFieldElement
    {
        [Inject] private GridSettings _settings;

        public event Action<float> OnSizeChanged;

        public float Size { get; set; }
        protected override float Spacing => _settings.SpacingFactor;

        public float RelativeSpacing { get; set; }

        [SerializeField] private GameObject _backgroundPrefab;
        [SerializeField] private float _backgroundPadding = 1.1f;

        private GameObject _background;

        public void SpawnGrid(int[,] grid)
        {
            SetCustomGrid(grid);
        }

        protected override void SpawnBackGround(float squareSize, float relativeSpacing)
        {
            Size = squareSize;
            RelativeSpacing = relativeSpacing;
            OnSizeChanged?.Invoke(Size);

            if (_backgroundPrefab != null)
            {
                _background = Instantiate(_backgroundPrefab, transform);
                
                _container.InjectGameObject(_background);
            }

            float backgroundWidth = _gridSize * squareSize + (_gridSize - 1) * relativeSpacing;
            float backgroundHeight = _gridSize * squareSize + (_gridSize - 1) * relativeSpacing;

            if (_background != null)
            {
                _background.transform.localScale = new Vector3(backgroundWidth * _backgroundPadding,
                    backgroundHeight * _backgroundPadding, 1);
                _background.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            }
        }

        protected override void SpawnedGrid()
        {
        }
    }
}
