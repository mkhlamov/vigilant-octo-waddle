using System.Collections.Generic;

namespace CardMatch.Card
{
    public interface ICardGenerationService
    {
        List<CardModel> GenerateCards(int totalCards, int availableCardTypes);
    }
}