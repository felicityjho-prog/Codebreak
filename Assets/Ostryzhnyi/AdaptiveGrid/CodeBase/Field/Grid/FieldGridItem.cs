using Ostryzhnyi.AdaptiveGrid.CodeBase.Field.Abstractions;
using Ostryzhnyi.AdaptiveGrid.CodeBase.Field.Bricks;
using UnityEngine;

namespace Ostryzhnyi.AdaptiveGrid.CodeBase.Field.Grid
{
    public class FieldGridItem : GridItem
    {
        public bool HasBrick => _brick != null;
        
        private Brick _brick;
        
        public void AttachBrick(Brick brick)
        {
            _brick = brick;
            brick.transform.SetParent(transform);
            brick.transform.localPosition = Vector3.zero;
        }
        
        public void DestroyBrick()
        {
            if(_brick == null)
                return;
            
            Destroy(_brick.gameObject);
            _brick = null;
        }
    }
}