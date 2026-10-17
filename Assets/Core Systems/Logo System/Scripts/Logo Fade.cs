using System;
using System.Collections;
using UnityEngine;

namespace LogoSystem
{
    public class LogoFade : LogoBase
    {
        [SerializeField] private float fadeTime = 1f;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override IEnumerator PlayRoutine(Action onFinish)
        {
            Color color = image.color;

            // fade in
            for (float t = 0; t < fadeTime; t += Time.deltaTime)
            {
                color.a = t / fadeTime;
                image.color = color;

                yield return null;
            }

            yield return new WaitForSeconds(duration);

            // fade out
            for (float t = 0; t < fadeTime; t += Time.deltaTime)
            {
                color.a = 1 - (t / fadeTime);
                image.color = color;

                yield return null;
            }

            onFinish?.Invoke();
        }
    }
}