using CardMatch.Card;
using CardMatch.Data;
using CardMatch.Grid;
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
            Container.Bind<GridView>()
                .FromComponentInHierarchy()
                .AsSingle();
            
            Container.Bind<RectTransform>()
                .WithId("GridContainer")
                .FromInstance(gridContainer)
                .AsSingle();
            
            Container.BindFactory<int, int, CardModel, CardModel.Factory>();

            Container.BindFactory<CardModel, Sprite, CardPresenter, CardPresenter.Factory>()
                .FromComponentInNewPrefab(cardPresenterPrefab)
                .UnderTransform(gridContainer);
            
            Container.Bind<ICardGenerationService>().To<CardGenerationService>().AsSingle();
        }
    }
}