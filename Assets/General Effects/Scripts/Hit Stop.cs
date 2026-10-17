using System.Collections;
using UnityEngine;

namespace GeneralEffects
{
    public class HitStop : MonoBehaviour
    {
        [SerializeField] private DefaultData defaultData;

        private Coroutine coroutine;

        public void PlayHitStop(float? duration = null)
        {
            Time.timeScale = 0;

            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(Routine(duration ?? defaultData.defaultHitstopDuration));
        }

        private IEnumerator Routine(float duration)
        {
            yield return new WaitForSecondsRealtime(duration);

            Time.timeScale = 1;

            coroutine = null;
        }
    }
}