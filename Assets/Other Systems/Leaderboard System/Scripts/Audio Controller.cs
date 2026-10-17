using System;
using UnityEngine;

namespace LeaderboardSystem
{
    public class AudioController : MonoBehaviour
    {
        [SerializeField] private AudioData audioData;

        public static event Action<AudioClip, AudioSource> OnSoundPlayed;

        public void PlayDisplayMovementSFX()
        {
            OnSoundPlayed?.Invoke(audioData.displayMovementSFX, null);
        }

        public void PlayScoreCountSFX()
        {
            OnSoundPlayed?.Invoke(audioData.scoreCountSFX, null);
        }

        public void PlayTierShowSFX()
        {
            OnSoundPlayed?.Invoke(audioData.tierShowSFX, null);
        }
    }
}