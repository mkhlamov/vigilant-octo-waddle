using System;
using CardMatch.Data;
using Cysharp.Threading.Tasks;
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
        
        private CardModel model;
        
        public event Action<CardPresenter> OnCardClicked;
        
        public CardModel Model => model;
        public CardView View => view;

        [Inject]
        public void Construct(CardModel model, Sprite cardSprite)
        {
            this.model = model;
            
            view.Initialize(cardSprite, levelSettings.cardBackSprite);
            view.Button.onClick.AddListener(OnCardClick);
            UpdateView();
        }
        
        private async void OnCardClick()
        {
            if (CanFlip())
            {
                OnCardClicked?.Invoke(this);
                
                model.State = CardState.Flipping;
                await view.FlipCard(true);
                model.State = CardState.FaceUp;
                UpdateView();
                
                await FlipBackAfterDelay();
            }
        }
        
        private async UniTask FlipBackAfterDelay()
        {
            await UniTask.Delay(500);
            
            if (model.State == CardState.FaceUp)
            {
                model.State = CardState.Flipping;
                await view.FlipCard(false);
                model.State = CardState.FaceDown;
                UpdateView();
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