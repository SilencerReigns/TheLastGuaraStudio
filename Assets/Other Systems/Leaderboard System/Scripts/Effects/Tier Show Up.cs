using System;
using DG.Tweening;
using UnityEngine;

namespace LeaderboardSystem
{
    public class TierShowUp : MonoBehaviour
    {
        [SerializeField] private GeneralData generalData;
        [SerializeField] private AudioController audioController;

        [SerializeField] private RectTransform tierText;

        public void ShowTier(Action OnScaleFinish)
        {
            Sequence sequence = DOTween.Sequence();

            sequence.AppendInterval(generalData.tierShowDelay);

            sequence.Append(tierText.DOScale(generalData.tierInitialScale, 0)
                .OnComplete(() => tierText.gameObject.SetActive(true)));

            sequence.Append
            (
                tierText.DOScale(1, generalData.tierShowUpDuration)
                    .OnComplete(() =>
                    {
                        audioController.PlayTierShowSFX();
                        OnScaleFinish?.Invoke();
                    })
            );

            sequence.Append
            (
                tierText.DOShakeAnchorPos
                (
                    generalData.tierShakeDuration,
                    generalData.tierShakeStrength,
                    generalData.tierShakeVibrato,
                    generalData.tierShakeRandomness
                )
            );
        }
    }
}