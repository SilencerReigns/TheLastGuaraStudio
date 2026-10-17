using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GeneralEffects
{
    public class VignetteShow : MonoBehaviour
    {
        [SerializeField] private DefaultData defaultData;

        private Volume volume;
        private Vignette vignette;

        private Sequence vignetteSequence;

        private void Start()
        {
            GameObject volumeObject = GameObject.FindWithTag("GlobalVolume");
            if (volumeObject == null)
            {
                Debug.LogError("[Vignette Flash] No volume in scene!");
                return;
            }

            volume = volumeObject.GetComponent<Volume>();

            if (!volume.profile.TryGet(out vignette))
                Debug.LogError("[Vignette Flash] Volume doesn't have a vignette module!");
        }

        public void PlayShowVignette
        (
            float? showDuration = null,
            Ease? showEase = null,
            float? showIntensity = null,
            float? stayDuration = null,
            float? hideDuration = null,
            Ease? hideEase = null
        )
        {
            if (volume == null || vignette == null)
                return;

            vignetteSequence?.Kill();

            vignetteSequence = DOTween.Sequence();

            vignetteSequence.Append
            (
                DOTween.To(
                    () => vignette.intensity.value,
                    x => vignette.intensity.value = x,
                    showIntensity ?? defaultData.defaultVignetteShowIntensity,
                    showDuration ?? defaultData.defaultVignetteShowDuration
                )
                .SetEase(showEase ?? defaultData.defaultVignetteShowEase)
            );

            vignetteSequence.AppendInterval(stayDuration ?? 0);

            vignetteSequence.OnComplete(() =>
            {
                //Se uma duração foi passada, o hide deve obrigatoriamente vir quando essa duração acabar, pois
                //fica implícito que o sistema que invocou o efeito quer que ele rode por esse tempo e já termine por conta própria
                if (stayDuration != null)
                    PlayHideVignette(hideDuration, hideEase);
            });
        }

        public void PlayHideVignette
        (
            float? hideDuration = null,
            Ease? hideEase = null
        )
        {
            if (volume == null || vignette == null)
                return;

            vignetteSequence?.Kill();

            vignetteSequence = DOTween.Sequence();

            vignetteSequence.Append
            (
                DOTween.To(
                    () => vignette.intensity.value,
                    x => vignette.intensity.value = x,
                    0,
                    hideDuration ?? defaultData.defaultVignetteHideDuration
                )
                .SetEase(hideEase ?? defaultData.defaultVignetteHideEase)
            );
        }
    }
}