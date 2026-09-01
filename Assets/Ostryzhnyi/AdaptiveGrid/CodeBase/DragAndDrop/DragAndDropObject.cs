using System;
using UnityEngine;

namespace Ostryzhnyi.AdaptiveGrid.CodeBase.DragAndDrop
{
    [RequireComponent(typeof(Collider))]
    public class DragAndDropObject: MonoBehaviour
    {
        public event Action OnDrop;
        public event Action OnStartDrag;
        
        private Vector3 _startPosition;
        private bool _isDragging = false;
        private Camera _mainCamera;
        private Transform _baseParent;
        private Vector3 _diffPositions;

        private bool _isEnable = true;

        [SerializeField] private LayerMask _targetLayer; 

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void OnMouseDown()
        {
            if(!_isEnable)
                return;
            
            _isDragging = true;
            _startPosition = transform.position; 
            _baseParent = transform.parent;
            transform.parent = null;
            _diffPositions = transform.position - _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            OnStartDrag?.Invoke();
        }

        private void OnMouseDrag()
        {
            if(!_isEnable)
                return;
            
            if (_isDragging)
            {
                Vector3 mousePos = Input.mousePosition;
                float zCoord = _mainCamera.WorldToScreenPoint(transform.position).z;
                Vector3 worldPos = _mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, zCoord));
                _diffPositions = new Vector3(_diffPositions.x, _diffPositions.y, 0);
                worldPos = new Vector3(worldPos.x, worldPos.y, -.2f);
                transform.position = worldPos + _diffPositions;
            }
        }

        private void OnMouseUp()
        {
            if(!_isEnable)
                return;
            
            _isDragging = false;

            OnDrop?.Invoke();
        }
        
        
        public void SetState(bool isEnable)
        {
            _isEnable = isEnable;
        }

        public void ReturnToStartPosition()
        {
            transform.position = _startPosition;
            transform.parent = _baseParent;
        }
    }
}