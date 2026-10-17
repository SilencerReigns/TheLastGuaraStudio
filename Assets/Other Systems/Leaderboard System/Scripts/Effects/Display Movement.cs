using System;
using DG.Tweening;
using UnityEngine;

namespace LeaderboardSystem
{
    public class DisplayMovement : MonoBehaviour
    {
        [SerializeField] private GeneralData generalData;
        [SerializeField] private AudioController audioController;

        public void MoveDisplayIn(RectTransform display, Action OnFinish)
        {
            display.DOAnchorPosX(0, generalData.displayMovementDuration).SetEase(generalData.displayMovementEase)
                .OnComplete(() => OnFinish?.Invoke());
        }

        public void MoveDisplayOut(RectTransform display, Action OnFinish)
        {
            float displayWidth = display.rect.width;

            display.DOAnchorPosX(-displayWidth, generalData.displayMovementDuration).SetEase(generalData.displayMovementEase)
                .OnComplete(() => OnFinish?.Invoke());
        }
    }
}