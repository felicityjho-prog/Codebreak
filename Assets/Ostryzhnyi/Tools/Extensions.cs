using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ostryzhnyi.Tools
{
    public static class Extensions
    {
        private const double EPSILON = float.Epsilon;
        private const double EPSILON_SQR = EPSILON * EPSILON;
        private const double EPSILON_VIEWING_VECTOR = 0.01f;
        
        public static void Shuffle<T>(this IList<T> list)  
        {  
            System.Random rng = new System.Random(); 
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = rng.Next(n + 1);  
                (list[k], list[n]) = (list[n], list[k]);
            }  
        }
        
        public static Type GetLinkedType(this Enum value, int pos = 0)
        {
            var da = (LinkedTypeAttribute[])(value.GetType().GetField(value.ToString()))
                .GetCustomAttributes(typeof(LinkedTypeAttribute), false);
            return da.Length > pos ? da[pos].Type : null;
        }
        
        public static string GetIdWithPlatform(string android, string ios)
        {
            return Application.platform == RuntimePlatform.Android
                   || Application.platform == RuntimePlatform.WindowsEditor
                ? android
                : ios;
        }

        #region Float

        public static bool FuzzyEquals0(this float v, double epsilon = EPSILON) => FuzzyEquals(v, 0, epsilon);
        public static bool FuzzyEquals(this float v, float b, double epsilon = EPSILON) => Math.Abs(v - b) < epsilon;
        public static bool FuzzyEquals(this Vector2 a, Vector2 b, double epsilon = EPSILON_VIEWING_VECTOR) => FuzzyEquals0(Vector2.SqrMagnitude(a - b), epsilon);
        public static bool FuzzyEquals0(this Vector2 a, double epsilon = EPSILON_VIEWING_VECTOR) => FuzzyEquals0(Vector2.SqrMagnitude(a), epsilon);
        public static bool FuzzyEquals(this Vector3 a, Vector3 b, double epsilon = EPSILON_VIEWING_VECTOR) => FuzzyEquals0(Vector3.SqrMagnitude(a - b), epsilon);
        public static bool FuzzyEquals0(this Vector3 a, double epsilon = EPSILON_VIEWING_VECTOR) => FuzzyEquals0(Vector3.SqrMagnitude(a), epsilon);
        /// <summary>
        /// Use to filter out Vector3's that wouldn't cause an issue when used with <see cref="Quaternion.LookRotation(Vector3)"/>
        /// </summary>
        public static bool IsValidViewingVector(this Vector3 a) => !FuzzyEquals0(Vector3.SqrMagnitude(a), EPSILON_VIEWING_VECTOR);

        #endregion

      

    }
}