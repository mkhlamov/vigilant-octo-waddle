using System;
using System.Collections.Generic;
using System.Linq;
using CardMatch.Audio;
using CardMatch.Card;
using CardMatch.Data;
using CardMatch.Grid;
using CardMatch.Score;
using CardMatch.UI;
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
        private readonly ICardGenerationService cardGenerationService;
        private readonly IGameStateStorage gameStateStorage;
        private readonly SignalBus signalBus;
        private readonly CardManager cardManager;

        public event Action OnGameCompleted;

        private List<CardModel> cardModels = new();

        public GameManager(
            ScoreModel scoreModel,
            GridConfig gridConfig,
            LevelSettings levelSettings,
            Levels.LevelManager levelManager,
            ICardGenerationService cardGenerationService,
            IGameStateStorage gameStateStorage,
            SignalBus signalBus,
            CardManager cardManager)
        {
            this.scoreModel = scoreModel;
            this.gridConfig = gridConfig;
            this.levelSettings = levelSettings;
            this.levelManager = levelManager;
            this.cardGenerationService = cardGenerationService;
            this.gameStateStorage = gameStateStorage;
            this.signalBus = signalBus;
            this.cardManager = cardManager;
            
            this.cardManager.OnCardClicked += OnCardClicked;
        }

        public void Initialize()
        {
            scoreModel.Initialize(levelManager.LevelIndex);
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
            cardModels = cardGenerationService.GenerateCards(gridConfig.TotalCards, levelSettings.cardSprites.Length);
            cardManager.CreateCards(cardModels);
            SaveState();
        }


        private void OnCardClicked(CardPresenter cardPresenter)
        {
            cardManager.AddFlippedCard(cardPresenter);

            if (cardManager.GetFlippedCards().Count == 2)
            {
                CheckForMatch();
            }
        }

        private void CheckForMatch()
        {
            scoreModel.AddAttempt();

            var flippedCards = cardManager.GetFlippedCards();
            var card1 = flippedCards[0];
            var card2 = flippedCards[1];

            if (card1.Model.TypeId == card2.Model.TypeId)
            {
                scoreModel.AddMatch();
                signalBus.Fire<CardMatchSignal>();
                
                cardManager.SetCardMatched(card1);
                cardManager.SetCardMatched(card2);
                SaveState();
                
                if (IsGameCompleted())
                {
                    scoreModel.CompleteGame(levelManager.LevelIndex);
                    signalBus.Fire<GameOverSignal>();
                    OnGameCompleted?.Invoke();
                }
            }
            else
            {
                scoreModel.SubtractPenalty();
                signalBus.Fire<CardMismatchSignal>();
                cardManager.SetCardFaceDown(card1);
                cardManager.SetCardFaceDown(card2);
                SaveState();
            }
            
            cardManager.ClearFlippedCards();
        }

        private bool IsGameCompleted()
        {
            return cardManager.AllCardsMatched();
        }


        public void Dispose()
        {
            if (cardManager != null)
            {
                cardManager.OnCardClicked -= OnCardClicked;
                cardManager.Dispose();
            }
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

            for (var i = 0; i < cardModels.Count; i++)
            {
                var model = cardModels[i];
                state.cards.Add(new CardData { id = model.ID, typeId = model.TypeId, state = (int)model.State });
            }

            gameStateStorage.Save(levelManager.LevelIndex, state);
        }

        private void RestoreFromState(GameStateData state)
        {
            scoreModel.Load(state.currentScore, state.matchesCount, state.attemptsCount);

            cardModels = new List<CardModel>();
            for (var i = 0; i < state.cards.Count; i++)
            {
                var cd = state.cards[i];
                var model = new CardModel(cd.id, cd.typeId);
                model.State = (CardState)cd.state;
                cardModels.Add(model);
            }

            cardManager.RestoreFromState(cardModels);
        }
    }
}