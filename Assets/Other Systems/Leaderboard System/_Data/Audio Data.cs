using UnityEngine;

namespace LeaderboardSystem
{
    [CreateAssetMenu(fileName = "AudioData", menuName = "Scriptable Objects/Leaderboard/AudioData")]
    public class AudioData : ScriptableObject
    {
        public AudioClip displayMovementSFX;
        public AudioClip scoreCountSFX;
        public AudioClip tierShowSFX;
    }
}