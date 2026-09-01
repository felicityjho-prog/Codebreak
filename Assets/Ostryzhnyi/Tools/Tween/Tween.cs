using UnityEngine;

namespace Ostryzhnyi.Tools.Tween
{
    public class Tween
    {
        private Coroutine _coroutine;

        public Tween(Coroutine coroutine)
        {
            _coroutine = coroutine;
        }

        public void Stop()
        {
            OstryzhnyiTweenCore.Core?.StopCoroutine(_coroutine);
        }
    }
}