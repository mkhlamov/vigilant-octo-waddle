using System.Collections.Generic;
using CardMatch.Card;
using CardMatch.Data;
using UnityEngine;

namespace CardMatch.Grid
{
    public class GridView : MonoBehaviour
    {
        //TODO move to settings installer
        [SerializeField]
        private GridConfig gridConfig;

        [SerializeField]
        private CardView cardPrefab;

        [SerializeField]
        private RectTransform gridContainer;

        private readonly List<CardView> cards = new();
        private readonly List<CardModel> cardModels = new();
        private readonly Color[] cardColors = { Color.blue, Color.red, Color.green };

        private void Start()
        {
            //TODO move to bootstrapper
            Generate();
        }

        private void Generate()
        {
            Clear();
            CreateCardData();
            CreateCardViews();
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
                var typeId = i % cardColors.Length;

                cardModels.Add(new CardModel(i, typeId));
                cardModels.Add(new CardModel(i + 1, typeId));
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

        private void CreateCardViews()
        {
            foreach (var cardModel in cardModels)
            {
                var cardView = Instantiate(cardPrefab, gridContainer);
                cardView.Initialize(cardColors[cardModel.TypeId]);
                cards.Add(cardView);
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