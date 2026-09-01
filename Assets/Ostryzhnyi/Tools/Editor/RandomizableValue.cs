using UnityEngine;

namespace Ostryzhnyi.Tools.Editor
{
    [System.Serializable]
    public class RandomizableValue
    {
        public enum RandomMode { Constant, RandomBetweenTwoConstants }

        public RandomMode Mode = RandomMode.Constant;
        public float ConstantValue = 1f;
        public float MinValue = 0f;
        public float MaxValue = 1f;

        public float GetValue()
        {
            switch (Mode)
            {
                case RandomMode.Constant:
                    return ConstantValue;
                case RandomMode.RandomBetweenTwoConstants:
                    return Random.Range(MinValue, MaxValue);
                default:
                    return ConstantValue;
            }
        }
    }
}