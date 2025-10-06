using System.Collections.Generic;
using UnityEngine;

namespace CardMatch.Card
{
    public class CardGenerationService : ICardGenerationService
    {
        private readonly CardModel.Factory cardModelFactory;

        public CardGenerationService(CardModel.Factory cardModelFactory)
        {
            this.cardModelFactory = cardModelFactory;
        }

        public List<CardModel> GenerateCards(int totalCards, int availableCardTypes)
        {
            ValidateInput(totalCards, availableCardTypes);
            
            var cardModels = new List<CardModel>();
            var pairsNeeded = totalCards / 2;

            for (var i = 0; i < pairsNeeded; i++)
            {
                var typeId = i % availableCardTypes;
                cardModels.Add(cardModelFactory.Create(i * 2, typeId));
                cardModels.Add(cardModelFactory.Create(i * 2 + 1, typeId));
            }

            ShuffleCards(cardModels);
            return cardModels;
        }

        private void ValidateInput(int totalCards, int availableCardTypes)
        {
            if (totalCards % 2 != 0)
            {
                Debug.LogError($"Total cards must be even for matching pairs. Got: {totalCards}");
            }
            
            if (availableCardTypes <= 0)
            {
                Debug.LogError($"Must have at least one card type available. Got: {availableCardTypes}");
            }
        }

        private void ShuffleCards(List<CardModel> cardModels)
        {
            for (var i = 0; i < cardModels.Count; i++)
            {
                var randomIndex = Random.Range(i, cardModels.Count);
                (cardModels[i], cardModels[randomIndex]) = (cardModels[randomIndex], cardModels[i]);
            }
        }
    }
}