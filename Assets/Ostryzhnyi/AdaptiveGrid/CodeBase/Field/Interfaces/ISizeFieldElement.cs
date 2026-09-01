using System;

namespace Ostryzhnyi.AdaptiveGrid.CodeBase.Field.Interfaces
{
    public interface ISizeFieldElement
    {
        public event Action<float> OnSizeChanged;
        
        public float Size {get; set;}
        public float RelativeSpacing {get; set;}
    }
}