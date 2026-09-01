using UnityEngine;

namespace Ostryzhnyi.AdaptiveGrid.CodeBase.Field.Grid.Data
{
    [CreateAssetMenu(fileName = "GridSettings", menuName = "ScriptableObjects/GridSettings")]
    public class GridSettings : ScriptableObject
    {
        public float SpacingFactor = -0.39f;
        public int GridSize = 12;
    }
}