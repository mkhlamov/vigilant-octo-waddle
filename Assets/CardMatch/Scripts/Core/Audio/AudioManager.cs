using UnityEngine;
using Zenject;

namespace CardMatch.Audio
{
    public class AudioManager : MonoBehaviour
    {
        private AudioSettings audioSettings;
        private AudioSource audioSource;

        [Inject]
        public void Construct(AudioSettings audioSettings)
        {
            this.audioSettings = audioSettings;
        }

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (!audioSource)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            audioSource.playOnAwake = false;
        }

        private void Start()
        {
            if (audioSettings)
            {
                audioSource.volume = audioSettings.volume;
            }
        }

        public void PlayCardFlipSound()
        {
            PlaySound(audioSettings.cardFlipSound);
        }

        public void PlayCardMatchSound()
        {
            PlaySound(audioSettings.cardMatchSound);
        }

        public void PlayCardMismatchSound()
        {
            PlaySound(audioSettings.cardMismatchSound);
        }

        public void PlayGameOverSound()
        {
            PlaySound(audioSettings.gameOverSound);
        }

        private void PlaySound(AudioClip clip)
        {
            if (audioSettings && clip && audioSource)
            {
                audioSource.PlayOneShot(clip, audioSettings.volume);
            }
        }
    }
}
