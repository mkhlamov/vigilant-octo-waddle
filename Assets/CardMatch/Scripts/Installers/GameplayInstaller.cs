using CardMatch.Bootstrap;
using CardMatch.Card;
using CardMatch.Core.Levels;
using CardMatch.Data;
using CardMatch.Grid;
using CardMatch.Levels;
using CardMatch.Score;
using CardMatch.UI.Score;
using UnityEngine;
using Zenject;

namespace CardMatch.Installers
{
    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField] 
        private RectTransform gridContainer;

        [SerializeField]
        private GameObject cardPresenterPrefab;

        [SerializeField]
        private LevelSettings[] availableLevels;
        
        public override void InstallBindings()
        {
            Container.Bind<GameManager>().AsSingle();
            Container.Bind<ScoreModel>().AsSingle();
            
            Container.Bind<RectTransform>()
                .WithId("GridContainer")
                .FromInstance(gridContainer)
                .AsSingle();
            
            Container.BindFactory<int, int, CardModel, CardModel.Factory>();

            Container.BindFactory<CardModel, Sprite, CardPresenter, CardPresenter.Factory>()
                .FromComponentInNewPrefab(cardPresenterPrefab)
                .UnderTransform(gridContainer);
            
            Container.Bind<ICardGenerationService>().To<CardGenerationService>().AsSingle();
            Container.Bind<GridLayoutCalculator>().AsSingle();
            Container.Bind<IScoreSaver>().To<PlayerPrefsScoreSaver>().AsSingle();
            Container.Bind<IGameStateStorage>().To<PlayerPrefsGameStateStorage>().AsSingle();
            
            var levelManager = new LevelManager(availableLevels);
            Container.Bind<LevelManager>().FromInstance(levelManager).AsSingle();
            Container.Bind<LevelSettings>().FromInstance(levelManager.LevelSettings).AsSingle();
            Container.Rebind<GridConfig>().FromInstance(levelManager.LevelSettings.gridConfig).AsSingle();
            
            Container.BindInterfacesAndSelfTo<GameCompletionHandler>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameplayStarter>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameStateLifecycle>().AsSingle().NonLazy();
            
            // Bind UI components
            Container.BindInterfacesAndSelfTo<GameScreenPresenter>().FromComponentsInHierarchy().AsSingle().NonLazy();

            InstallSignals();
        }

        private void InstallSignals()
        {
            SignalBusInstaller.Install(Container);
            
            Container.DeclareSignal<CardFlipSignal>();
            Container.DeclareSignal<CardMatchSignal>();
            Container.DeclareSignal<CardMismatchSignal>();
            Container.DeclareSignal<GameOverSignal>();
        }
    }
}