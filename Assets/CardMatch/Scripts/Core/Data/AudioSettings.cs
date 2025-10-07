using UnityEngine;

namespace CardMatch.Audio
{
    [CreateAssetMenu(fileName = "AudioSettings", menuName = "CardMatch/Audio Settings")]
    public class AudioSettings : ScriptableObject
    {
        public AudioClip cardFlipSound;
        public AudioClip cardMatchSound;
        public AudioClip cardMismatchSound;
        public AudioClip gameOverSound;
        [Range(0f, 1f)]
        public float volume = 1f;
    }
}
