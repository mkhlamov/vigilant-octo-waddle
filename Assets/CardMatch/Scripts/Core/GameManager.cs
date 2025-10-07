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
        private readonly Levels.LevelManager levelManager;
        private readonly RectTransform gridContainer;
        private readonly CardPresenter.Factory cardPresenterFactory;
        private readonly ICardGenerationService cardGenerationService;
        private readonly GridLayoutCalculator gridLayoutCalculator;
        private readonly IGameStateStorage gameStateStorage;

        public event Action OnGameCompleted;

        private readonly List<CardPresenter> flippedCards = new();
        private readonly List<CardPresenter> allCards = new();
        private List<CardModel> cardModels = new();

        public GameManager(
            ScoreModel scoreModel,
            GridConfig gridConfig,
            LevelSettings levelSettings,
            Levels.LevelManager levelManager,
            [Inject(Id = "GridContainer")] RectTransform gridContainer,
            CardPresenter.Factory cardPresenterFactory,
            ICardGenerationService cardGenerationService,
            GridLayoutCalculator gridLayoutCalculator,
            IGameStateStorage gameStateStorage)
        {
            this.scoreModel = scoreModel;
            this.gridConfig = gridConfig;
            this.levelSettings = levelSettings;
            this.levelManager = levelManager;
            this.gridContainer = gridContainer;
            this.cardPresenterFactory = cardPresenterFactory;
            this.cardGenerationService = cardGenerationService;
            this.gridLayoutCalculator = gridLayoutCalculator;
            this.gameStateStorage = gameStateStorage;
        }

        public void Initialize()
        {
            scoreModel.Initialize();
            if (gameStateStorage.HasSavedState(levelManager.LevelIndex))
            {
                var state = gameStateStorage.Load(levelManager.LevelIndex);
                if (state != null)
                {
                    RestoreFromState(state);
                    return;
                }
            }
            GenerateCards();
        }

        private void GenerateCards()
        {
            ClearCards();
            cardModels = cardGenerationService.GenerateCards(gridConfig.TotalCards, levelSettings.cardSprites.Length);
            CreateCardPresenters();
            PositionCards();
            SaveState();
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
                SaveState();
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
                    OnGameCompleted?.Invoke();
                }
            }
            else
            {
                scoreModel.SubtractPenalty();
                _ = card1.SetFaceDown();
                _ = card2.SetFaceDown();
            }

            SaveState();
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

        public void SaveState()
        {
            var state = new GameStateData
            {
                currentScore = scoreModel.CurrentScore,
                matchesCount = scoreModel.MatchesCount,
                attemptsCount = scoreModel.AttemptsCount,
                cards = new List<CardData>()
            };

            foreach (var model in allCards.Select(t => t.Model))
            {
                state.cards.Add(new CardData
                {
                    id = model.ID,
                    typeId = model.TypeId,
                    state = (int)model.State
                });
            }

            gameStateStorage.Save(levelManager.LevelIndex, state);
        }

        private void RestoreFromState(GameStateData state)
        {
            ClearCards();
            scoreModel.Load(state.currentScore, state.matchesCount, state.attemptsCount);

            cardModels = new List<CardModel>();
            foreach (var cd in state.cards)
            {
                var model = new CardModel(cd.id, cd.typeId);
                model.State = (CardState)cd.state;
                cardModels.Add(model);
            }

            CreateCardPresenters();
            PositionCards();
            foreach (var card in allCards.Where(card => card.Model.State == CardState.FaceUp))
            {
                flippedCards.Add(card);
            }
        }
    }
}