namespace CardMatch.Data
{
    public interface IGameStateStorage
    {
        bool HasSavedState(int levelIndex);
        void Save(int levelIndex, GameStateData state);
        GameStateData Load(int levelIndex);
        void Clear(int levelIndex);
    }
}