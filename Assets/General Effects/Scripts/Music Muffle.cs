using System;
using DG.Tweening;
using UnityEngine;

namespace GeneralEffects
{
    public class MusicMuffle : MonoBehaviour
    {
        [SerializeField] private DefaultData defaultData;

        private AudioLowPassFilter lowPassFilter;

        private Sequence muffleSequence;

        private void Start()
        {
            GameObject musicObject = GameObject.FindWithTag("MusicSource");
            if (musicObject == null)
            {
                Debug.LogError("[Music Muffle] No music source in scene!"); ;

                return;
            }

            lowPassFilter = musicObject.GetComponent<AudioLowPassFilter>();
        }

        public void PlayMuffle
        (
            float? cutoffFrequency = null,
            float? mufflingDuration = null,
            Ease? mufflingEase = null,
            float? muffledDuration = null,
            float? demuffilgDuration = null,
            Ease? demufflingEase = null
        )
        {
            if (lowPassFilter == null)
                return;

            muffleSequence?.Kill();

            muffleSequence = DOTween.Sequence();
            muffleSequence.SetUpdate(true);

            muffleSequence.Append
            (
                DOTween.To(
                    () => lowPassFilter.cutoffFrequency,
                    x => lowPassFilter.cutoffFrequency = x,
                    cutoffFrequency ?? defaultData.defaultCutoffFrequency,
                    mufflingDuration ?? defaultData.defaultMufflingDuration
                )
                .SetEase(mufflingEase ?? defaultData.defaultMufflingEase)
            );

            muffleSequence.AppendInterval(muffledDuration ?? 0);

            muffleSequence.OnComplete(() =>
            {
                //Se uma duração foi passada, o demuffle deve obrigatoriamente vir quando essa duração acabar, pois
                //fica implícito que o sistema que invocou o efeito quer que ele rode por esse tempo e já termine por conta própria
                if (muffledDuration != null)
                    PlayDemuffle(demuffilgDuration, demufflingEase);
            });
        }

        public void PlayDemuffle
        (
            float? demufflingDuration = null,
            Ease? demufflingEase = null
        )
        {
            if (lowPassFilter == null)
                return;

            muffleSequence?.Kill();

            muffleSequence = DOTween.Sequence();
            muffleSequence.SetUpdate(true);

            muffleSequence.Append
            (
                DOTween.To(
                    () => lowPassFilter.cutoffFrequency,
                    x => lowPassFilter.cutoffFrequency = x,
                    22000f,
                    demufflingDuration ?? defaultData.defaultDemufflingDuration
                )
                .SetEase(demufflingEase ?? defaultData.defaultDemufflingEase)
            );
        }
    }
}