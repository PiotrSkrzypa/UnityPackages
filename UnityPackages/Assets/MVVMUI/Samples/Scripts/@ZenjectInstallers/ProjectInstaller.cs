using PSkrzypa.EventBus;
using UnityEngine;
using Zenject;

namespace PSkrzypa.MVVMUI.Samples
{
    public class ProjectInstaller : MonoInstaller<ProjectInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<IMenuController>().To<MenuController>().AsSingle().NonLazy();
            Container.Bind<IThreadDispatcher>().To<MainThreadDispatcher>().AsSingle().NonLazy();
            Container.Bind<IEventBus>().To<EventBus.EventBus>().AsSingle().NonLazy();
        }
    }
}