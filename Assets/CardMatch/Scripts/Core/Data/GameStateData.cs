using System;
using System.Collections.Generic;

namespace CardMatch.Data
{
    [Serializable]
    public class GameStateData
    {
        public int currentScore;
        public int matchesCount;
        public int attemptsCount;
        public List<CardData> cards;
    }

    [Serializable]
    public class CardData
    {
        public int id;
        public int typeId;
        public int state;
    }
}