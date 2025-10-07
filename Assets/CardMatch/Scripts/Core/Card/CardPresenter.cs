using System;
using System.Threading.Tasks;
using CardMatch.Audio;
using CardMatch.Data;
using UnityEngine;
using Zenject;

namespace CardMatch.Card
{
    public class CardPresenter : MonoBehaviour, IDisposable
    {
        [SerializeField]
        private CardView view;

        [Inject]
        private LevelSettings levelSettings;

        [Inject]
        private SignalBus signalBus;

        private CardModel model;

        public event Action<CardPresenter> OnCardClicked;

        public CardModel Model => model;
        public CardView View => view;

        [Inject]
        public void Construct(CardModel model, Sprite cardSprite)
        {
            this.model = model;

            view.Initialize(cardSprite, levelSettings.cardBackSprite);
            view.ResetVisuals();
            view.Button.onClick.AddListener(OnCardClick);
            UpdateView();
        }

        private async void OnCardClick()
        {
            if (CanFlip())
            {
                model.State = CardState.Flipping;
                signalBus.Fire<CardFlipSignal>();
                await view.FlipCard(true);
                model.State = CardState.FaceUp;
                UpdateView();

                OnCardClicked?.Invoke(this);
            }
        }

        private bool CanFlip()
        {
            return model.State == CardState.FaceDown;
        }

        public void SetMatched()
        {
            model.State = CardState.Matched;
            UpdateView();
        }

        public async Task SetFaceDown()
        {
            model.State = CardState.Flipping;
            signalBus.Fire<CardFlipSignal>();
            await view.FlipCard(false);
            model.State = CardState.FaceDown;
            UpdateView();
        }

        public void Dispose()
        {
            if (view && view.Button)
            {
                view.Button.onClick.RemoveListener(OnCardClick);
            }
        }

        public void SetSize(Vector2 cardSize)
        {
            view.SetSize(cardSize);
        }

        public void SetPosition(Vector2 position)
        {
            view.SetPosition(position);
        }

        private void UpdateView()
        {
            switch (model.State)
            {
                case CardState.FaceDown:
                    view.ShowFaceDown();
                    break;
                case CardState.FaceUp:
                    view.ShowFaceUp();
                    break;
                case CardState.Matched:
                    view.ShowMatched();
                    break;
                case CardState.Flipping:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public class Factory : PlaceholderFactory<CardModel, Sprite, CardPresenter>
        {
        }
    }
}