using System;
using Zenject;

namespace CardMatch.Audio
{
    public class SoundEffectsManager : IInitializable, IDisposable
    {
        private readonly AudioManager audioManager;
        private readonly SignalBus signalBus;

        public SoundEffectsManager(AudioManager audioManager, SignalBus signalBus)
        {
            this.audioManager = audioManager;
            this.signalBus = signalBus;
        }

        public void Initialize()
        {
            signalBus.Subscribe<CardFlipSignal>(OnCardFlip);
            signalBus.Subscribe<CardMatchSignal>(OnCardMatch);
            signalBus.Subscribe<CardMismatchSignal>(OnCardMismatch);
            signalBus.Subscribe<GameOverSignal>(OnGameOver);
        }

        public void Dispose()
        {
            signalBus.Unsubscribe<CardFlipSignal>(OnCardFlip);
            signalBus.Unsubscribe<CardMatchSignal>(OnCardMatch);
            signalBus.Unsubscribe<CardMismatchSignal>(OnCardMismatch);
            signalBus.Unsubscribe<GameOverSignal>(OnGameOver);
        }

        private void OnCardFlip()
        {
            audioManager.PlayCardFlipSound();
        }

        private void OnCardMatch()
        {
            audioManager.PlayCardMatchSound();
        }

        private void OnCardMismatch()
        {
            audioManager.PlayCardMismatchSound();
        }

        private void OnGameOver()
        {
            audioManager.PlayGameOverSound();
        }
    }
}
