using UnityEngine;
using UnityEngine.Audio;

namespace AudioSystem
{
    public class MixerController : MonoBehaviour
    {
        [SerializeField] private AudioMixer mixer;

        private void Start()
        {
            ChangeVolume("masterVolume", PlayerPrefs.GetFloat("masterVolume", 1f));
            ChangeVolume("musicVolume", PlayerPrefs.GetFloat("musicVolume", 1f));
            ChangeVolume("effectsVolume", PlayerPrefs.GetFloat("effectsVolume", 1f));
        }

        private void OnEnable()
        {
            SliderSetup.OnVolumeChanged += ChangeVolume;
        }
        private void OnDisable()
        {
            SliderSetup.OnVolumeChanged -= ChangeVolume;
        }

        public void ChangeVolume(string paramName, float newVolume)
        {
            float volume = Mathf.Clamp(newVolume, 0.0001f, 1f);
            mixer.SetFloat(paramName, Mathf.Log10(volume) * 20);

            PlayerPrefs.SetFloat(paramName, volume);
            PlayerPrefs.Save();
        }
    }
}