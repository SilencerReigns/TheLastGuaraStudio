using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace LeaderboardSystem
{
    public class ScoreCount : MonoBehaviour
    {
        [SerializeField] private GeneralData generalData;
        [SerializeField] private AudioController audioController;
     
        [SerializeField] private TextMeshProUGUI scoreText;

        public void CountScore(Action OnFinish)
        {
            int counter = 0;
            int maxValue = PlayerPrefs.GetInt("Score", 0);

            Sequence sequence = DOTween.Sequence();

            sequence.Append(
                DOTween.To
                (
                    () => counter,
                    value =>
                    {
                        counter = value;

                        scoreText.text = value.ToString();
                        audioController.PlayScoreCountSFX();
                    },
                    maxValue,
                    maxValue / generalData.scoreCountSpeed
                ).SetEase(Ease.Linear)
            );

            sequence.Append(scoreText.rectTransform.DOScale(generalData.scorePulseSize, generalData.scorePulseUpDuration));
            sequence.Append(scoreText.rectTransform.DOScale(1, generalData.scorePulseDownDuration));

            sequence.OnComplete(() => OnFinish?.Invoke());
        }
    }
}