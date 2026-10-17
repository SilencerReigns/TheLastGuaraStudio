using System;
using System.Collections;
using UnityEngine;

namespace LeaderboardSystem
{
    public class EffectsController : MonoBehaviour
    {
        [SerializeField] private DisplayMovement displayMovement;
        [SerializeField] private ScoreCount scoreCount;
        [SerializeField] private TierShowUp tierShowUp;

        public void ApplyDisplayEffects(RectTransform displayIn, RectTransform displayOut, Action OnFinish)
        {
            displayMovement.MoveDisplayIn(displayIn, OnFinish);
            displayMovement.MoveDisplayOut(displayOut, OnFinish);
        }

        public void ApplyStartEffects()
        {
            StartCoroutine(EffectsRoutine());
        }

        private IEnumerator EffectsRoutine()
        {
            bool countFinished = false;
            scoreCount.CountScore(() => countFinished = true);

            yield return new WaitUntil(() => countFinished);

            bool tierScaleFinished = false;
            tierShowUp.ShowTier(() => tierScaleFinished = true);

            yield return new WaitUntil(() => tierScaleFinished);
        }
    }
}