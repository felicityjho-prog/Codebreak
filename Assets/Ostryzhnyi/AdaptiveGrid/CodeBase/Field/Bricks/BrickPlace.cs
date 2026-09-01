using Ostryzhnyi.AdaptiveGrid.CodeBase.Field.Interfaces;
using Ostryzhnyi.DI;
using UnityEngine;

namespace Ostryzhnyi.AdaptiveGrid.CodeBase.Field.Bricks
{
    public class BrickPlace : MonoBehaviour
    {
        [Inject] private ISizeFieldElement _sizeFieldElement;
        [Inject] private DIContainer _diContainer;
        
        [SerializeField] private Brick _brickPrefab = default;
        
        private Brick _currentBrick;

        private readonly Vector3 SizeMutiplier = new Vector3(1.2f, 1.2f, 1.2f);
        
        private void Start()
        {
            _sizeFieldElement.OnSizeChanged += Respawn;
            _sizeFieldElement.OnSizeChanged += Resize;
        }
        
        private void OnDestroy()
        {
            _sizeFieldElement.OnSizeChanged -= Respawn;
            _sizeFieldElement.OnSizeChanged -= Resize;
        }

        private void OnDrop(Brick brick)
        {
            if (brick.CanAttach(out var gridItem))
            {
                gridItem.AttachBrick(brick);
                brick.OnDrop -= OnDrop;
                brick.SetDragState(false);
                Respawn(_sizeFieldElement.Size);
                return;
            }

            BackToSpawn(brick);
        }

        private void SpawnBrick()
        {
            var brick = Instantiate(_brickPrefab, transform);
            _diContainer.InjectDependencies(brick);
            brick.OnDrop += OnDrop;
            brick.OnStartDrag += OnOnStartDrag;

            brick.ResizeToFitGrid(_sizeFieldElement.Size * Vector3.one);
        }

        private void OnOnStartDrag(Brick brick)
        {
            brick.transform.parent = null;
            brick.ResizeToFitGrid(_sizeFieldElement.Size * Vector3.one);
        }

        private void Respawn(float size)
        {
            SpawnBrick();
        }

        private void BackToSpawn(Brick brick)
        {
            brick.ReturnToStartPosition();
            brick.ResizeToFitGrid(_sizeFieldElement.Size * Vector3.one);
        }
        
        private void Resize(float sizeBrick)
        {
            transform.localScale = SizeMutiplier * sizeBrick;
        }

    }
}
