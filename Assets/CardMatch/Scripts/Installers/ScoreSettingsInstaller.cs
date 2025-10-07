using CardMatch.Core;
using UnityEngine;
using Zenject;

namespace CardMatch.Installers
{
    [CreateAssetMenu(fileName = "ScoreSettingsInstaller", menuName = "CardMatch/ScoreSettingsInstaller")]
    public class ScoreSettingsInstaller : ScriptableObjectInstaller<ScoreSettingsInstaller>
    {
        [SerializeField]
        private ScoreSettings scoreSettings;
        
        public override void InstallBindings()
        {
            Container.BindInstance(scoreSettings).AsSingle();
        }
    }
}