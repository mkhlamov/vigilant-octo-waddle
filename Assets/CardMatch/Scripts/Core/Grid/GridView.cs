using System.Collections.Generic;
using CardMatch.Card;
using CardMatch.Data;
using UnityEngine;
using Zenject;

namespace CardMatch.Grid
{
    public class GridView : MonoBehaviour
    {
        private GridConfig gridConfig;
        private LevelSettings levelSettings;
        private RectTransform gridContainer;
        private CardPresenter.Factory cardPresenterFactory;
        private ICardGenerationService cardGenerationService;
        private GridLayoutCalculator gridLayoutCalculator;

        private readonly List<CardPresenter> cards = new();
        private List<CardModel> cardModels = new();

        [Inject]
        private void Construct(GridConfig gridConfig,
            LevelSettings levelSettings,
            [Inject(Id = "GridContainer")] RectTransform gridContainer,
            CardModel.Factory cardModelFactory,
            CardPresenter.Factory cardPresenterFactory,
            ICardGenerationService cardGenerationService,
            GridLayoutCalculator gridLayoutCalculator)
        {
            this.gridConfig = gridConfig;
            this.levelSettings = levelSettings;
            this.gridContainer = gridContainer;
            this.cardPresenterFactory = cardPresenterFactory;
            this.cardGenerationService = cardGenerationService;
            this.gridLayoutCalculator = gridLayoutCalculator;
        }
        
        private void Start()
        {
            //TODO move to bootstrapper
            Generate();
        }

        private void Generate()
        {
            Clear();
            cardModels = cardGenerationService.GenerateCards(gridConfig.TotalCards, levelSettings.cardSprites.Length);
            CreateCardPresenters();

            PositionCards();
        }

        private void Clear()
        {
            foreach (var card in cards)
            {
                if (card)
                {
                    DestroyImmediate(card.gameObject);
                }
            }

            cards.Clear();
            cardModels.Clear();
        }

        private void CreateCardPresenters()
        {
            foreach (var cardModel in cardModels)
            {
                var card = cardPresenterFactory.Create(cardModel, levelSettings.cardSprites[cardModel.TypeId]);
                cards.Add(card);
            }
        }

        private void PositionCards()
        {
            var containerSize = gridContainer.rect.size;
            var cardSize = gridLayoutCalculator.CalculateCardSize(containerSize);

            for (var i = 0; i < cards.Count; i++)
            {
                var gridPosition = gridLayoutCalculator.CalculateGridPosition(i);
                var worldPosition = gridLayoutCalculator.CalculateCardPosition(gridPosition.row, gridPosition.col, cardSize);

                SetupCardTransform(cards[i], cardSize, worldPosition);
            }
        }
        
        private void SetupCardTransform(CardPresenter card, Vector2 cardSize, Vector2 position)
        {
            var cardRect = card.GetComponent<RectTransform>();
            card.SetSize(cardSize);
            card.SetPosition(position);
            cardRect.sizeDelta = cardSize;
            cardRect.anchoredPosition = position;
        }
    }
}