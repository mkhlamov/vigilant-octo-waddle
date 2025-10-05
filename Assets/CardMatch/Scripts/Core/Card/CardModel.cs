using Zenject;

namespace CardMatch.Card
{
    public class CardModel
    {
        public int ID;
        public int TypeId;
        public CardState State;
    
        public CardModel(int id, int typeId)
        {
            this.ID = id;
            this.TypeId = typeId;
            this.State = CardState.FaceDown;
        }
        
        public class Factory : PlaceholderFactory<int, int, CardModel>
        {
        }

    }
    
    public enum CardState
    {
        FaceDown,
        Flipping,
        FaceUp,
        Matched
    }
}