using Abstractions;
using Behaviours;
using Behaviours.FolkLift;
using Behaviours.UI;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class MainInstaller : MonoInstaller
    {
        [SerializeField] private MainScreen _mainScreen;
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameManager>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("GameManager").AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<MainScreen>().FromInstance(_mainScreen).AsSingle();
            
            Container.Bind<IForkLift>().To<ForkLiftBase>().AsSingle();
            Container.Bind<ILevel>().To<Level>().AsSingle();
        }
    }
}