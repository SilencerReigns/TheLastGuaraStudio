using System.Collections;
using UnityEngine;

namespace GeneralEffects
{
    public class MusicFade : MonoBehaviour
    {
        [SerializeField] private DefaultData defaultData;

        private AudioSource musicSource;
        private Coroutine coroutine;

        private void Start()
        {
            GameObject musicObject = GameObject.FindWithTag("MusicSource");
            if (musicObject == null)
            {
                Debug.LogError("[Music Fade] No music source in scene!"); ;

                return;
            }

            musicSource = musicObject.GetComponent<AudioSource>();
        }

        public void PlayFadeIn(float? duration = null)
        {
            if (musicSource == null)
                return;

            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(Routine(musicSource.volume, 1, duration ?? defaultData.defaultMusicFadeInDuration));
        }

        public void PlayFadeOut(float? duration = null)
        {
            if (musicSource == null)
                return;

            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(Routine(musicSource.volume, 0, duration ?? defaultData.defaultMusicFadeOutDuration));
        }

        private IEnumerator Routine(float startValue, float endValue, float duration)
        {
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                musicSource.volume = Mathf.Lerp(startValue, endValue, elapsedTime / duration);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            musicSource.volume = endValue;

            coroutine = null;
        }
    }
}