using System;
using System.Collections.Generic;
using System.Linq;
using CardMatch.Card;
using CardMatch.Data;
using CardMatch.Grid;
using CardMatch.UI;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace CardMatch
{
    public class CardManager : IDisposable
    {
        private readonly RectTransform gridContainer;
        private readonly CardPresenter.Factory cardPresenterFactory;
        private readonly GridLayoutCalculator gridLayoutCalculator;
        private readonly ContainerResizeDetector resizeDetector;
        private readonly LevelSettings levelSettings;

        private readonly List<CardPresenter> flippedCards = new();
        private readonly List<CardPresenter> allCards = new();
        private List<CardModel> cardModels = new();

        public event Action<CardPresenter> OnCardClicked;

        public CardManager(
            [Inject(Id = "GridContainer")] RectTransform gridContainer,
            CardPresenter.Factory cardPresenterFactory,
            GridLayoutCalculator gridLayoutCalculator,
            ContainerResizeDetector resizeDetector,
            LevelSettings levelSettings)
        {
            this.gridContainer = gridContainer;
            this.cardPresenterFactory = cardPresenterFactory;
            this.gridLayoutCalculator = gridLayoutCalculator;
            this.resizeDetector = resizeDetector;
            this.levelSettings = levelSettings;
            
            this.resizeDetector.OnContainerSizeChanged += OnContainerSizeChanged;
        }

        public void CreateCards(List<CardModel> models)
        {
            ClearCards();
            cardModels = new List<CardModel>(models);
            CreateCardPresenters();
            PositionCards();
        }

        public void ClearCards()
        {
            for (var i = 0; i < allCards.Count; i++)
            {
                var card = allCards[i];
                if (card)
                {
                    card.OnCardClicked -= OnCardClicked;
                    card.Dispose();
                    Object.Destroy(card.gameObject);
                }
            }

            allCards.Clear();
            cardModels.Clear();
            flippedCards.Clear();
        }

        public List<CardPresenter> GetFlippedCards()
        {
            return new List<CardPresenter>(flippedCards);
        }

        public void ClearFlippedCards()
        {
            flippedCards.Clear();
        }

        public void AddFlippedCard(CardPresenter card)
        {
            flippedCards.Add(card);
        }

        public bool AllCardsMatched()
        {
            return allCards.All(card => card.Model.State == CardState.Matched);
        }

        public void SetCardMatched(CardPresenter card)
        {
            card.SetMatched();
        }

        public void SetCardFaceDown(CardPresenter card)
        {
            _ = card.SetFaceDown();
        }

        public void RestoreFromState(List<CardModel> models)
        {
            ClearCards();
            cardModels = new List<CardModel>(models);
            CreateCardPresenters();
            PositionCards();
            
            for (var i = 0; i < allCards.Count; i++)
            {
                var card = allCards[i];
                if (card.Model.State == CardState.FaceUp)
                {
                    flippedCards.Add(card);
                }
            }
        }

        private void CreateCardPresenters()
        {
            foreach (var cardModel in cardModels)
            {
                var card = cardPresenterFactory.Create(cardModel, levelSettings.cardSprites[cardModel.TypeId]);
                card.OnCardClicked += OnCardClicked;
                allCards.Add(card);
            }
        }

        private void PositionCards()
        {
            var containerSize = gridContainer.rect.size;
            var cardSize = gridLayoutCalculator.CalculateCardSize(containerSize);

            for (var i = 0; i < allCards.Count; i++)
            {
                var gridPosition = gridLayoutCalculator.CalculateGridPosition(i);
                var worldPosition = gridLayoutCalculator.CalculateCardPosition(gridPosition.row, gridPosition.col, cardSize);

                SetupCardTransform(allCards[i], cardSize, worldPosition);
            }
        }

        private void SetupCardTransform(CardPresenter card, Vector2 cardSize, Vector2 position)
        {
            card.SetSize(cardSize);
            card.SetPosition(position);
        }

        private void OnContainerSizeChanged(Vector2 newSize)
        {
            if (allCards.Count > 0)
            {
                PositionCards();
            }
        }

        public void Dispose()
        {
            if (resizeDetector)
            {
                resizeDetector.OnContainerSizeChanged -= OnContainerSizeChanged;
            }
            
            foreach (var card in allCards)
            {
                if (card)
                {
                    card.OnCardClicked -= OnCardClicked;
                    card.Dispose();
                }
            }
            
            allCards.Clear();
            flippedCards.Clear();
        }
    }
}
