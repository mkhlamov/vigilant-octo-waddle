using Zenject;

namespace CardMatch.Bootstrap
{
    public class GameplayStarter : IInitializable
    {
        private GameManager gameManager;
        
        public GameplayStarter(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        //Wait till UI is properly calculated
        public void Initialize()
        {
            gameManager.Initialize();
        }
    }
}