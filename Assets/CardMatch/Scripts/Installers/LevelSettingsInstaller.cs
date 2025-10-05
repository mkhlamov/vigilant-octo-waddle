using CardMatch.Data;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace CardMatch.Installers
{
    [CreateAssetMenu(fileName = "LevelSettingsInstaller", menuName = "CardMatch/LevelSettingsInstaller")]
    public class LevelSettingsInstaller : ScriptableObjectInstaller<LevelSettingsInstaller>
    {
        [SerializeField]
        private GridConfig gridConfig;
        
        public override void InstallBindings()
        {
            Container.BindInstance(gridConfig).AsSingle();
        }
    }
}