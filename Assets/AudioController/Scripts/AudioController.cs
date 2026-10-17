using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        GameObject[] list = GameObject.FindGameObjectsWithTag("DontDestroy");

        foreach (GameObject obj in list)
        {
            DontDestroyOnLoad(obj);
        }
    }

    public void PlayMusic(AudioClip clip, bool loop)
    {
        if (musicSource == null) return;

        musicSource.DOFade(0f, fadeDuration);
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.DOFade(1f, fadeDuration);
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    public void PlaySFX(AudioClip clip, bool loops = false)
    {
        if (sfxSource == null) return;

        sfxSource.PlayOneShot(clip);
    }

    public void StopSFX()
    {
        sfxSource.Stop();
    }


}
