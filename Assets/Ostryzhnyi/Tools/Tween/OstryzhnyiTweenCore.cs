using System;
using UnityEngine;

namespace Ostryzhnyi.Tools.Tween
{
    public class OstryzhnyiTweenCore : MonoBehaviour
    {
        private static OstryzhnyiTweenCore _core;

        private static bool _isQuit = false;
        
        public static OstryzhnyiTweenCore Core
        {
            get
            {
                if (_isQuit)
                    return null;
                
                if (_core == null)
                {
                    GameObject singletonObject = new GameObject(nameof(OstryzhnyiTweenCore));
                    _core = singletonObject.AddComponent<OstryzhnyiTweenCore>();

                    DontDestroyOnLoad(singletonObject);
                }
                return _core;
            }
        }

        private void OnApplicationQuit()
        {
            _isQuit = true;
        }
    }
}