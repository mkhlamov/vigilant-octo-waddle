using CardMatch.Bootstrap;
using CardMatch.Card;
using CardMatch.Core;
using CardMatch.Data;
using CardMatch.Grid;
using CardMatch.Score;
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
            
            Container.BindInterfacesAndSelfTo<GameplayStarter>().AsSingle().NonLazy();
        }
    }
}