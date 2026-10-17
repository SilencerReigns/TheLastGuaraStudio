using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "AudioData", menuName = "Scriptable Objects/Player/AudioData")]
    public class AudioData : ScriptableObject
    {
        public AudioClip[] walkSFXs;
        public AudioClip jumpSFX;
        public AudioClip landSFX;
        public AudioClip dashSFX;
    }
}