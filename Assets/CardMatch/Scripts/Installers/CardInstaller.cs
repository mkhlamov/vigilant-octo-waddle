using CardMatch.Card;
using Zenject;

namespace CardMatch.Installers
{
    public class CardInstaller : Installer<CardInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CardModel>().AsSingle();
        }
    }
}