using DG.Tweening;
using UnityEngine;

namespace LeaderboardSystem
{
    [CreateAssetMenu(fileName = "GeneralData", menuName = "Scriptable Objects/Leaderboard/GeneralData")]
    public class GeneralData : ScriptableObject
    {
        public string leaderboardId;
        public bool evaluatesResult;
        public bool loadNextScene;
        public string nextSceneName;

        [Header("Evaluation")]
        public int maxScore;
        public Sprite goodResultImage;
        public Sprite mediocreResultImage;
        public Sprite badResultImage;
        public string[] tierNames;
        public float[] tierThresholds;

        [Header("Effects")]
        [Header("Display Movement")]
        public float displayMovementDuration;
        public Ease displayMovementEase;

        [Header("Score Count")]
        public float scoreCountSpeed;
        public float scorePulseSize;
        public float scorePulseUpDuration;
        public float scorePulseDownDuration;

        [Header("Tier ShowUp")]
        public float tierShowDelay;
        public float tierInitialScale;
        public float tierShowUpDuration;
        public float tierShakeDuration;
        public float tierShakeStrength;
        public int tierShakeVibrato;
        public float tierShakeRandomness;
    }
}