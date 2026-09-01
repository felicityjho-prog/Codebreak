using System;
using Ostryzhnyi.AdaptiveGrid.CodeBase.DragAndDrop;
using Ostryzhnyi.AdaptiveGrid.CodeBase.Field.Abstractions;
using Ostryzhnyi.AdaptiveGrid.CodeBase.Field.Grid;
using UnityEngine;

namespace Ostryzhnyi.AdaptiveGrid.CodeBase.Field.Bricks
{
    [RequireComponent(typeof(DragAndDropObject))]
    public class Brick : GridItem
    {
        public event Action<Brick> OnDrop;
        public event Action<Brick> OnStartDrag;
        
        [SerializeField] private LayerMask _targetLayer;
        private BoxCollider _parentCollider;

        private DragAndDropObject _dragAndDropObject;

        protected override void Awake()
        {
            base.Awake();
            _dragAndDropObject = GetComponent<DragAndDropObject>();
            _dragAndDropObject.OnDrop += OnDropHandler;
            _dragAndDropObject.OnStartDrag += OnStartDragHandler;
        }

        private void OnDestroy()
        {
            _dragAndDropObject.OnDrop -= OnDropHandler;
            _dragAndDropObject.OnStartDrag -= OnStartDragHandler;
        }

        private void OnDropHandler()
        {
            OnDrop?.Invoke(this);
        }

        public bool CanAttach(out FieldGridItem gridItem)
        {
            Ray ray = new Ray(transform.position, Vector3.forward);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _targetLayer))
            {
                if(hit.collider.TryGetComponent<FieldGridItem>(out var element) && !element.HasBrick)
                {
                    gridItem = element;
                    return true;
                }
            }
            
            gridItem = null;
            return false;
        }

        public void ReturnToStartPosition()
        {
            _dragAndDropObject.ReturnToStartPosition();
        }

        public void SetDragState(bool state)
        {
            _dragAndDropObject.SetState(state);
        }

        private void OnStartDragHandler()
        {
            OnStartDrag?.Invoke(this);
        }
        
        public void ResizeToFitGrid(Vector3 blockSize)
        {
            transform.localScale = blockSize;
        }
    }
}