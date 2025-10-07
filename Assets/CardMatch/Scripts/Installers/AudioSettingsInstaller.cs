using CardMatch.Audio;
using UnityEngine;
using Zenject;
using AudioSettings = CardMatch.Audio.AudioSettings;

namespace CardMatch.Installers
{
    public class AudioSettingsInstaller : MonoInstaller
    {
        [SerializeField]
        private AudioSettings audioSettings;
        
        [SerializeField]
        private AudioManager audioManager;

        public override void InstallBindings()
        {
            Container.Bind<AudioSettings>().FromInstance(audioSettings).AsSingle();
            Container.Bind<AudioManager>().FromInstance(audioManager).AsSingle();
            Container.BindInterfacesAndSelfTo<SoundEffectsManager>().AsSingle().NonLazy();
        }
    }
}
