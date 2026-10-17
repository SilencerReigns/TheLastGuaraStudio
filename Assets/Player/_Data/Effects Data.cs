using DG.Tweening;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "EffectsData", menuName = "Scriptable Objects/Player/EffectsData")]
    public class EffectsData : ScriptableObject
    {
        [Header("DASH")]
        [Header("After Images")]
        public float imageDuration;
        public float startingAlpha;
        public float delayBetweenImages;

        [Header("MOVEMENT")]
        public float runParticlesCooldown;
    }
}