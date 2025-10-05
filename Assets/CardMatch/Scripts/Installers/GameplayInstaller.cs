using CardMatch.Grid;
using UnityEngine;
using Zenject;

namespace CardMatch.Installers
{
    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField] private RectTransform gridContainer;
        
        public override void InstallBindings()
        {
            Container.Bind<GridView>()
                .FromComponentInHierarchy()
                .AsSingle();

            
            Container.Bind<RectTransform>()
                .WithId("GridContainer")
                .FromInstance(gridContainer)
                .AsSingle();
        }
    }
}