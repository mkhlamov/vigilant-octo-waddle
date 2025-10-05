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
        private CardModel.Factory cardModelFactory;
        private CardPresenter.Factory cardPresenterFactory;

        private readonly List<CardPresenter> cards = new();
        private readonly List<CardModel> cardModels = new();

        [Inject]
        private void Construct(GridConfig gridConfig,
            LevelSettings levelSettings,
            [Inject(Id = "GridContainer")] RectTransform gridContainer,
            CardModel.Factory cardModelFactory,
            CardPresenter.Factory cardPresenterFactory)
        {
            this.gridConfig = gridConfig;
            this.levelSettings = levelSettings;
            this.gridContainer = gridContainer;
            this.cardModelFactory = cardModelFactory;
            this.cardPresenterFactory = cardPresenterFactory;
        }
        
        private void Start()
        {
            //TODO move to bootstrapper
            Generate();
        }

        private void Generate()
        {
            Clear();
            CreateCardData();
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

        //TODO refactor
        private void CreateCardData()
        {
            var totalCards = gridConfig.TotalCards;
            var pairsNeeded = totalCards / 2;

            for (var i = 0; i < pairsNeeded; i++)
            {
                var typeId = i % levelSettings.cardSprites.Length;
                
                cardModels.Add(cardModelFactory.Create(i * 2, typeId));
                cardModels.Add(cardModelFactory.Create(i * 2 + 1, typeId));
            }

            ShuffleCards();
        }
        
        private void ShuffleCards()
        {
            for (var i = 0; i < cardModels.Count; i++)
            {
                var randomIndex = Random.Range(i, cardModels.Count);
                (cardModels[i], cardModels[randomIndex]) = (cardModels[randomIndex], cardModels[i]);
            }
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
            var cardSize = CalculateCardSize(containerSize);

            for (var i = 0; i < cards.Count; i++)
            {
                var row = i / gridConfig.columns;
                var col = i % gridConfig.columns;

                Vector2 position = CalculateCardPosition(row, col, cardSize);

                var cardRect = cards[i].GetComponent<RectTransform>();
                cards[i].SetSize(cardSize);
                cards[i].SetPosition(position);
                cardRect.sizeDelta = cardSize;
                cardRect.anchoredPosition = position;
            }
        }

        private Vector2 CalculateCardSize(Vector2 containerSize)
        {
            var availableWidth = containerSize.x - gridConfig.gridPadding.x * 2 -
                                 gridConfig.cardSpacing * (gridConfig.columns - 1);
            var availableHeight = containerSize.y - gridConfig.gridPadding.y * 2 -
                                  gridConfig.cardSpacing * (gridConfig.rows - 1);

            var calculatedSize = new Vector2(availableWidth / gridConfig.columns, availableHeight / gridConfig.rows);

            var aspectRatio = gridConfig.cardSize.x / gridConfig.cardSize.y;
            calculatedSize = calculatedSize.x / aspectRatio <= calculatedSize.y
                ? new Vector2(calculatedSize.x, calculatedSize.x / aspectRatio)
                : new Vector2(calculatedSize.y * aspectRatio, calculatedSize.y);

            return calculatedSize;
        }
        
        private Vector2 CalculateCardPosition(int row, int col, Vector2 cardSize)
        {
            var totalWidth = gridConfig.columns * cardSize.x + (gridConfig.columns - 1) * gridConfig.cardSpacing;
            var totalHeight = gridConfig.rows * cardSize.y + (gridConfig.rows - 1) * gridConfig.cardSpacing;
        
            var startX = -totalWidth / 2 + cardSize.x / 2;
            var startY = totalHeight / 2 - cardSize.y / 2;
        
            var x = startX + col * (cardSize.x + gridConfig.cardSpacing);
            var y = startY - row * (cardSize.y + gridConfig.cardSpacing);
        
            return new Vector2(x, y);
        }
    }
}