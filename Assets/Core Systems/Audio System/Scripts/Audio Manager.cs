using UnityEngine;
using UnityEngine.Audio;

namespace AudioSystem
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [SerializeField] private AudioSource defaultMusicSource;
        [SerializeField] private AudioSource defaultSFXSource;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        public void PlayMusic(AudioClip music, AudioSource audioSource)
        {
            AudioSource usedSource;
            usedSource = audioSource ? audioSource : defaultMusicSource;

            usedSource.outputAudioMixerGroup = defaultMusicSource.outputAudioMixerGroup;

            if (usedSource.clip != music)
            {
                usedSource.clip = music;
                usedSource.loop = true;

                usedSource.Play();
            }
        }
        public void StopMusic(AudioSource audioSource)
        {
            AudioSource usedSource;
            usedSource = audioSource ? audioSource : defaultMusicSource;

            usedSource.Stop();
        }

        public void PlayContinuousSFX(AudioClip sfx, AudioSource audioSource)
        {
            AudioSource usedSource;
            usedSource = audioSource ? audioSource : defaultSFXSource;

            usedSource.outputAudioMixerGroup = defaultSFXSource.outputAudioMixerGroup;

            usedSource.clip = sfx;
            usedSource.loop = true;

            usedSource.Play();
        }
        public void StopContinuousSFX(AudioSource audioSource)
        {
            AudioSource usedSource;
            usedSource = audioSource ? audioSource : defaultSFXSource;

            usedSource.Stop();
        }

        public void PlaySFX(AudioClip sfx, AudioSource audioSource)
        {
            AudioSource usedSource;
            usedSource = audioSource ? audioSource : defaultSFXSource;

            usedSource.outputAudioMixerGroup = defaultSFXSource.outputAudioMixerGroup;

            usedSource.PlayOneShot(sfx);
        }
    }
}