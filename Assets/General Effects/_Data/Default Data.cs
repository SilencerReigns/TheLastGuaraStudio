using DG.Tweening;
using UnityEngine;

namespace GeneralEffects
{
    [CreateAssetMenu(fileName = "Default Data", menuName = "Scriptable Objects/General Effects/Default Data")]
    public class DefaultData : ScriptableObject
    {
        [Header("Camera Shake")]
        public float defaultShaketime;
        public float defaultShakeForce;
        public Vector3 defaultShakeVelocity;

        [Header("Controller Rumble")]
        public float defaultLowFrequency;
        public float defaultHighFrequency;
        public float defaultRumbleDuration;
        
        [Header("Hit Stop")]
        public float defaultHitstopDuration;

        [Header("Music Fade")]
        public float defaultMusicFadeInDuration;
        public float defaultMusicFadeOutDuration;

        [Header("Music Muffle")]
        public float defaultCutoffFrequency;
        public float defaultMufflingDuration;
        public Ease defaultMufflingEase;
        public float defaultDemufflingDuration;
        public Ease defaultDemufflingEase;

        [Header("Sprite Flash")]
        public float defaultFlashDuration;

        [Header("Vignette Show")]
        public float defaultVignetteShowDuration;
        public Ease defaultVignetteShowEase;
        public float defaultVignetteShowIntensity;
        public float defaultVignetteHideDuration;
        public Ease defaultVignetteHideEase;
    }
}