using System;
using System.Collections;
using UnityEngine;

namespace Ostryzhnyi.Tools.Tween
{
    public static class OstryzhnyiTween
    {
        #region ToColor
        
        public static Tween ToColor(this SpriteRenderer spriteRenderer, Color toColor, float duration, Action onComplete = null)
        {
            var coroutine = OstryzhnyiTweenCore.Core?.StartCoroutine(ToColorCoroutine(spriteRenderer, toColor, duration, newColor =>
            {
                spriteRenderer.color = newColor;
                onComplete?.Invoke();
            }));

            return new Tween(coroutine);
        }

        private static IEnumerator ToColorCoroutine(SpriteRenderer spriteRenderer, Color toColor, float duration, Action<Color> onColorUpdate)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                Color currentColor = Color.Lerp(spriteRenderer.color, toColor, elapsed / duration);
                spriteRenderer.color = currentColor;
                onColorUpdate?.Invoke(currentColor);
                yield return null;
            }

            onColorUpdate?.Invoke(toColor);
        }

        #endregion

        #region ToColorCycle

        public static Tween ToColorCycle(this SpriteRenderer spriteRenderer, Color toColor, float duration)
        {
            var coroutine = OstryzhnyiTweenCore.Core.StartCoroutine(ToColorCycleCoroutine(spriteRenderer, toColor, duration));

            return new Tween(coroutine);
        }
        private static IEnumerator ToColorCycleCoroutine(SpriteRenderer spriteRenderer, Color toColor, float duration)
        {
            var baseColor = spriteRenderer.color;
            
            while(OstryzhnyiTweenCore.Core != null)
            {
                float elapsed = 0f;
                spriteRenderer.color = baseColor;
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    Color currentColor = Color.Lerp(baseColor, toColor, elapsed / duration);
                    spriteRenderer.color = currentColor;
                
                    yield return new WaitForEndOfFrame();
                }
                
                elapsed = 0f;
                
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    Color currentColor = Color.Lerp(toColor, baseColor, elapsed / duration);
                    spriteRenderer.color = currentColor;
                
                    yield return new WaitForEndOfFrame();
                }
                spriteRenderer.color = toColor;
            }
        }

        #endregion
    }
}