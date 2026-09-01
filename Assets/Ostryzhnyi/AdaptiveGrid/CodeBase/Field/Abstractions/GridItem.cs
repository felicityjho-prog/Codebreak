using UnityEngine;

namespace Ostryzhnyi.AdaptiveGrid.CodeBase.Field.Abstractions
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class GridItem : MonoBehaviour
    {
        public SpriteRenderer Sprite;

        protected virtual void Awake()
        {
            Sprite = GetComponent<SpriteRenderer>();
        }
    }
}