using System;
using UnityEngine;

namespace Player
{
    public class AudioController : MonoBehaviour
    {
        [SerializeField] private AudioData audioData;

        public static event Action<AudioClip, AudioSource> OnSoundPlayed;

        public void PlayWalkSFX()
        {
            AudioClip[] sfxs = audioData.walkSFXs;

            OnSoundPlayed?.Invoke(sfxs[UnityEngine.Random.Range(0, sfxs.Length)], null);
        }

        public void PlayJumpSFX()
        {
            OnSoundPlayed?.Invoke(audioData.jumpSFX, null);
        }

        public void PlayLandSFX()
        {
            OnSoundPlayed?.Invoke(audioData.landSFX, null);
        }

        public void PlayDashSFX()
        {
            OnSoundPlayed?.Invoke(audioData.dashSFX, null);
        }
    }
}