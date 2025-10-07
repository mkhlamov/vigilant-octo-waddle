using System;
using System.Collections.Generic;
using System.Linq;
using CardMatch.Card;
using CardMatch.Data;
using CardMatch.Grid;
using CardMatch.Score;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace CardMatch
{
    public class GameManager : IDisposable
    {
        private readonly ScoreModel scoreModel;
        private readonly GridConfig gridConfig;
        private readonly LevelSettings levelSettings;
        private readonly RectTransform gridContainer;
        private readonly CardPresenter.Factory cardPresenterFactory;
        private readonly ICardGenerationService cardGenerationService;
        private readonly GridLayoutCalculator gridLayoutCalculator;

        private readonly List<CardPresenter> flippedCards = new();
        private readonly List<CardPresenter> allCards = new();
        private List<CardModel> cardModels = new();

        public GameManager(
            ScoreModel scoreModel,
            GridConfig gridConfig,
            LevelSettings levelSettings,
            [Inject(Id = "GridContainer")] RectTransform gridContainer,
            CardPresenter.Factory cardPresenterFactory,
            ICardGenerationService cardGenerationService,
            GridLayoutCalculator gridLayoutCalculator)
        {
            this.scoreModel = scoreModel;
            this.gridConfig = gridConfig;
            this.levelSettings = levelSettings;
            this.gridContainer = gridContainer;
            this.cardPresenterFactory = cardPresenterFactory;
            this.cardGenerationService = cardGenerationService;
            this.gridLayoutCalculator = gridLayoutCalculator;
        }

        public void Initialize()
        {
            scoreModel.Initialize();
            GenerateCards();
        }

        private void GenerateCards()
        {
            ClearCards();
            cardModels = cardGenerationService.GenerateCards(gridConfig.TotalCards, levelSettings.cardSprites.Length);
            CreateCardPresenters();
            PositionCards();
        }

        private void ClearCards()
        {
            foreach (var card in allCards)
            {
                if (card)
                {
                    card.OnCardClicked -= OnCardClicked;
                    card.Dispose();
                    Object.DestroyImmediate(card.gameObject);
                }
            }

            allCards.Clear();
            cardModels.Clear();
            flippedCards.Clear();
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

        private void OnCardClicked(CardPresenter cardPresenter)
        {
            flippedCards.Add(cardPresenter);

            if (flippedCards.Count == 2)
            {
                CheckForMatch();
            }
        }

        private void CheckForMatch()
        {
            scoreModel.AddAttempt();

            var card1 = flippedCards[0];
            var card2 = flippedCards[1];

            if (card1.Model.TypeId == card2.Model.TypeId)
            {
                scoreModel.AddMatch();
                
                card1.SetMatched();
                card2.SetMatched();
                
                if (IsGameCompleted())
                {
                    scoreModel.CompleteGame();
                }
            }
            else
            {
                scoreModel.SubtractPenalty();
                _ = card1.SetFaceDown();
                _ = card2.SetFaceDown();
            }

            flippedCards.Clear();
        }

        private bool IsGameCompleted()
        {
            return allCards.All(card => card.Model.State == CardState.Matched);
        }

        public void Dispose()
        {
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